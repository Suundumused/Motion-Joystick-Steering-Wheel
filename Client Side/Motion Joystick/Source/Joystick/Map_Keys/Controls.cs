using Motion_Joystick.Source.Joystick.Map_Keys_Interface;
using MessagePack;
using KeyAttribute = MessagePack.KeyAttribute;


namespace Motion_Joystick.Source.Joystick.Map_Keys
{
    public class Controls : I_Controls
    {
        private CancellationTokenSource? _cancellationTokenSource;

        public bool _running { get; private set; } = false;

        private double _lastAccelerationX = 0;
        private double _lastAccelerationY = 0;
        private const double NoiseThreshold = 1.0;

        private readonly double minSmoothing = 0.05;  
        private readonly double maxSmoothing = 0.1;

        private double[] final_pack_value = new double[] { 0.0, 0.0 };

        private AccelerometerData _latestData;

        public bool StartAccelerometer()
        {
            try
            {
                if (Accelerometer.Default.IsSupported)
                {
                    SensorSpeed _sensorSpeed;

                    if (MauiProgram.settingsAcessor.RefreshRate < 8.4) 
                    {
                        if (MauiProgram.settingsAcessor.PowerSavingMode)
                        {
                            _sensorSpeed = SensorSpeed.Game;
                        }
                        else 
                        {
                            _sensorSpeed = SensorSpeed.Fastest;
                        }
                    }
                    else if (MauiProgram.settingsAcessor.RefreshRate <= 12) 
                    {
                        if (MauiProgram.settingsAcessor.PowerSavingMode) 
                        {
                            _sensorSpeed = SensorSpeed.UI;
                        }
                        else 
                        {
                            _sensorSpeed = SensorSpeed.Game;
                        }
                    }
                    else
                    {
                        if (MauiProgram.settingsAcessor.PowerSavingMode) 
                        {
                            _sensorSpeed = SensorSpeed.UI;
                        }
                        else 
                        {
                            _sensorSpeed = SensorSpeed.Game;
                        }
                    }

                    if (Accelerometer.Default.IsMonitoring)
                    {
                        Accelerometer.Default.ReadingChanged -= Accelerometer_ReadingChanged;
                        Accelerometer.Default.Stop();
                    }

                    Accelerometer.Default.ReadingChanged += Accelerometer_ReadingChanged;
                    Accelerometer.Default.Start(_sensorSpeed);

                    _cancellationTokenSource = new CancellationTokenSource();
                    _ = StartManualHzLoop(_cancellationTokenSource.Token);

                    _running = true;
                    return true;
                }

                return false;
            }
            catch (FeatureNotSupportedException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void StopAccelerometer()
        {
            _running = false;

            try
            {
                _cancellationTokenSource?.Cancel();
            }catch{}
                
            try
            {
                Accelerometer.Default.ReadingChanged -= Accelerometer_ReadingChanged;
                Accelerometer.Default.Stop();
            }
            catch {}
        }

        private void Accelerometer_ReadingChanged(object? sender, AccelerometerChangedEventArgs e)
        {
            _latestData = e.Reading;
        }

        private async Task StartManualHzLoop(CancellationToken token)
        {
            int delay;

            if (MauiProgram.settingsAcessor.PowerSavingMode) 
            {
                delay = (int)(MauiProgram.settingsAcessor.RefreshRate * 2);
            }
            else 
            {
                delay = (int)MauiProgram.settingsAcessor.RefreshRate;
            }

            double smoothingBase = 144 / (1 / MauiProgram.settingsAcessor.RefreshRate * 1000);

            while (!token.IsCancellationRequested)
            {
                AccelerometerData reading = _latestData;

                if (_lastAccelerationX != reading.Acceleration.X && _lastAccelerationY != reading.Acceleration.Y) 
                {
                    double deltaX = Math.Abs(reading.Acceleration.X - _lastAccelerationX);
                    double deltaY = Math.Abs(reading.Acceleration.Y - _lastAccelerationY);

                    double x = reading.Acceleration.X;
                    double y = reading.Acceleration.Y;

                    if (x <= 1 && x >= -1 && y <= 1 && y >= -1)
                    {
                        if (deltaX > NoiseThreshold && deltaY > NoiseThreshold)
                        {
                            _lastAccelerationX = x;
                            _lastAccelerationY = y;

                            final_pack_value[0] = x;
                            final_pack_value[1] = y;
                        }
                        else
                        {
                            double smoothingFactor = Math.Min(Math.Max((minSmoothing + (Math.Max(deltaX, deltaY) / NoiseThreshold) * (maxSmoothing - minSmoothing)) * MauiProgram.settingsAcessor.Sensibility, minSmoothing), maxSmoothing) * smoothingBase;

                            _lastAccelerationX += (x - _lastAccelerationX) * smoothingFactor;
                            _lastAccelerationY += (y - _lastAccelerationY) * smoothingFactor;

                            final_pack_value[0] = _lastAccelerationX;
                            final_pack_value[1] = _lastAccelerationY;
                        }

                        await Send_axes("w", final_pack_value);
                    }
                }
                await Task.Delay(delay, token);
            }

            _cancellationTokenSource?.Dispose();
        }

        public static async Task Press_button(string id, bool reliable = true)
        {
            if (MauiProgram.socketConnection is not null)
            {
                await MauiProgram.socketConnection.SendData(new MyMessage { Command = "b", ID = id, Value = true, Reliable = reliable });
            }

            VibrateActive();
        }

        public static async Task Release_button(string id, bool reliable = true)
        {
            if (MauiProgram.socketConnection is not null)
            {
                await MauiProgram.socketConnection.SendData(new MyMessage { Command = "b", ID = id, Value = false, Reliable = reliable });
            }

            VibrateDisable();
        }

        public static async Task Send_axes(string id, dynamic value, bool reliable = false)
        {
            if (MauiProgram.socketConnection is not null)
            {
                await MauiProgram.socketConnection.SendData(new MyMessage { Command = "a", ID = id, Value = value, Reliable = reliable });
            }
        }

        public static async Task Send_config(string id, dynamic value, bool reliable = true)
        {
            if (MauiProgram.socketConnection is not null)
            {
                await MauiProgram.socketConnection.SendData(new MyMessage { Command = "c", ID = id, Value = value, Reliable = reliable });
            }
        }

        public static void VibrateActive()
        {
            if (!Vibration.Default.IsSupported) return;
            if (MauiProgram.settingsAcessor.Vibration) Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(1));
        }

        public static void VibrateDisable()
        {
            if (!Vibration.Default.IsSupported) return;
            if (MauiProgram.settingsAcessor.Vibration) Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(1));
        }
    }


    [MessagePackObject]
    public class MyMessage
    {
        [Key(0)]
        public required string Command { get; set; }

        [Key(1)]
        public required string ID { get; set; }

        [Key(2)]
        public required dynamic Value { get; set; }

        [Key(3)]
        public bool Reliable { get; set; }
    }
}