using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using MessagePack;
using Motion_Joystick.Resources.Languages;
using Motion_Joystick.Source.Joystick.Map_Keys;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;


namespace Motion_Joystick.Source.Network
{
    public class Socket_Connection
    {
        private readonly string _host;
        private readonly int _port;
        private readonly int _maxRetries = 3;

        private Socket _udpSocket;
        private IPEndPoint _remoteEndPoint;

        private CancellationTokenSource _cts;

        private List<MyMessage> dataQueue = new List<MyMessage>();
        private Task processDataQueueTask = Task.CompletedTask;

        private Stopwatch timer = new Stopwatch();

        private int ping_reach_id = -1;
        public long ping { get; set; } = 0;

        public int _refresh_rate = (int)Math.Floor(MauiProgram.settingsAcessor.RefreshRate);

        public Button? _connectButton;
        public Label? _pingLabel;

        public Socket_Connection(string host, int port)
        {
            _host = host;
            _port = port;
            _remoteEndPoint = new IPEndPoint(IPAddress.Parse(_host), _port);
        }

        public async Task StartAsync(Button connectButton, Label PingLabel)
        {
            _connectButton = connectButton;
            _pingLabel = PingLabel;

            _cts = new CancellationTokenSource();
            _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //_udpSocket.Bind(new IPEndPoint(IPAddress.Parse(_host), _port));

            _ = Controls.Send_config("rr", MauiProgram.settingsAcessor.RefreshRate / 1000);

            await RunCommunicationLoopAsync(_cts.Token);
        }

        private static void ShowAlertAsync(string title, string message, string buttonText)
        {
            Toast.Make($"{title}: {message}", ToastDuration.Long).Show();
        }

        private void UpdateConnectButton(bool isConnected)
        {
            if (_connectButton != null && _pingLabel != null)
            {
                _connectButton.Text = isConnected ? AppResources.Disconnect : AppResources.Connect;
                _connectButton.BackgroundColor = isConnected ? Colors.Red : Colors.LightGreen;
                if (!isConnected) _pingLabel.Text = "";
            }
        }

        private async Task RunCommunicationLoopAsync(CancellationToken token)
        {
            if (!MauiProgram.joyControls.StartAccelerometer()) 
            {
                if (Application.Current?.Windows.FirstOrDefault()?.Page is Page currentPage)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await currentPage.DisplayAlert(AppResources.Error, AppResources.gyro_not_supported, "OK");
                    });
                }

                Stop();
                _cts?.Dispose();

                return;
            }

            ShowAlertAsync("-->", $"{AppResources.Connected_to} {_host}:{_port}", "OK");
            UpdateConnectButton(true);

            if (_connectButton is not null)
            {
                _connectButton.IsEnabled = true;
            }

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await SendPingAsync(token);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Communication loop canceled.");
                    return;
                }
                catch (Exception ex)
                {
                    ShowAlertAsync(AppResources.Error, AppResources.Failed_tocon, "OK");
                    Stop();
                    
                    Console.WriteLine($"Send error: {ex.Message}");
                    return;
                }
            }

            _cts?.Dispose();
        }

        private async Task SendPingAsync(CancellationToken token)
        {
            await Task.Delay(1000, token);

            if (_udpSocket == null)
                throw new InvalidOperationException(AppResources.Not_Con);

            token.ThrowIfCancellationRequested();

            if (ping_reach_id == -1) 
            {
                ping_reach_id = 5;

                timer.Start();
                await SendData(new MyMessage { Command = "p", ID = "", Value = ping_reach_id, Reliable = true });
            }
        }

        private void StopPingAsync(MyMessage as_data) 
        {
            if (as_data.Value is not int || as_data.Value != ping_reach_id)
            {
                return;
            }

            timer.Stop();
            ping = timer.ElapsedMilliseconds;
            timer.Reset();

            if (_pingLabel != null)
            {
                _pingLabel.Text = $"Ping: {ping} ms";
                _pingLabel.TextColor = ping switch
                {
                    > 12 => Colors.OrangeRed,
                    > 5 => Colors.Yellow,
                    _ => Colors.LightGreen
                };
            }

            ping_reach_id = -1;
        }

        private void StopPingForce() 
        {
            timer.Stop();
            ping = timer.ElapsedMilliseconds;

            if (_pingLabel != null)
            {
                _pingLabel.Text = $"Ping: {ping} ms";
                _pingLabel.TextColor = ping switch
                {
                    > 12 => Colors.OrangeRed,
                    > 5 => Colors.Yellow,
                    _ => Colors.LightGreen
                };
            }

            ping_reach_id = -1;
        }

        public async Task SendData(MyMessage data) 
        {
            try
            {
                if (!data.Reliable)
                {
                    await _udpSocket.SendToAsync(new ArraySegment<byte>(MessagePackSerializer.Serialize(data)), SocketFlags.None, _remoteEndPoint);
                    return;
                }

                dataQueue.Add(data);

                if (processDataQueueTask.IsCompleted)
                {
                    processDataQueueTask = ProcessDataQueueAsync(_cts.Token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error catch 2 sending data: {ex.Message}");
            }
        }

        private async Task ProcessDataQueueAsync(CancellationToken token)
        {
            int attempts;

            while (dataQueue.Count > 0 && !token.IsCancellationRequested)
            {
                attempts = 0;
                MyMessage data = dataQueue[0];

                try
                {
                    await _udpSocket.SendToAsync(new ArraySegment<byte>(MessagePackSerializer.Serialize(data)), SocketFlags.None, _remoteEndPoint);

                    while (attempts < _maxRetries && !token.IsCancellationRequested)
                    {
                        byte[] buffer = new byte[1024];

                        using CancellationTokenSource? timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                        timeoutCts.CancelAfter(250);

                        try
                        {
                            SocketReceiveFromResult result = await _udpSocket.ReceiveFromAsync(
                                new ArraySegment<byte>(buffer), SocketFlags.None, _remoteEndPoint, timeoutCts.Token);

                            if (result.ReceivedBytes == 1 && buffer[0] == 0x01) //ack
                            {
                                StopPingAsync(data);
                                break;
                            }
                            else 
                            {
                                Console.WriteLine($"Unexpected response");
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            Console.WriteLine($"Send (timeout).");
                        }

                        attempts++;
                        if (attempts == _maxRetries) 
                        {
                            StopPingForce();
                        }
                        Console.WriteLine($"Attempt {attempts} failed, retrying...");
                    }

                    dataQueue.RemoveAt(0);
                }
                catch (Exception ex)
                {
                    StopPingForce();
                    dataQueue.RemoveAt(0);

                    Console.WriteLine($"Error sending data: {ex.Message}");
                }
            }
            processDataQueueTask = Task.CompletedTask;
        }

        public void Stop()
        {
            _cts?.Cancel();
            timer.Stop();

            try { _udpSocket?.Dispose(); } catch { }

            MauiProgram.joyControls.StopAccelerometer();
            MauiProgram.socketConnection = null;

            UpdateConnectButton(false);

            if (_connectButton is not null)
            {
                _connectButton.IsEnabled = true;
            }

            GC.SuppressFinalize(obj: this);
        }
    }
}
