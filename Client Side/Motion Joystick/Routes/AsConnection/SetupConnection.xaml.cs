using Motion_Joystick.Resources.Languages;
using Motion_Joystick.Source.Network;
using System.Net;

namespace Motion_Joystick.Routes.AsConnection;

public partial class SetupConnection : ContentPage
{
    private bool _size_changed = false;

    public SetupConnection()
    {
        InitializeComponent();
        Logo.HeightRequest = 185 * MauiProgram.main_density_preference;

        IpBox1.Text = MauiProgram.settingsAcessor.ServerFieldA.ToString();
        IpBox2.Text = MauiProgram.settingsAcessor.ServerFieldB.ToString();
        IpBox3.Text = MauiProgram.settingsAcessor.ServerFieldC.ToString();
        IpBox4.Text = MauiProgram.settingsAcessor.ServerFieldD.ToString();

        PortEntry.Text = MauiProgram.settingsAcessor.ServerPort.ToString();

        if (MauiProgram.socketConnection != null)
        {
            UpdateConnectButton(true);
            MauiProgram.socketConnection._connectButton = ConnectBTN;
            MauiProgram.socketConnection._pingLabel = PingLabel;
        }
        else
        {
            UpdateConnectButton(false);
        }

    }
    public Button ConnectButton => ConnectBTN;

    private void UpdateConnectButton(bool isConnected)
    {
        if (ConnectButton != null && PingLabel != null)
        {
            if (isConnected)
            {
                ConnectButton.Text = AppResources.Disconnect;
                ConnectButton.BackgroundColor = Colors.Red;
            }
            else
            {
                ConnectButton.Text = AppResources.Connect;
                ConnectButton.BackgroundColor = Colors.LightGreen;
                PingLabel.Text = "";
            }
        }
    }

    private async Task ScrollToEnd()
    {
        await scroll_view.ScrollToAsync(ConnectBTN, ScrollToPosition.End, true);
    }

    private void OnIpPartChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry && entry.Text?.Length == entry.MaxLength)
        {
            if (entry == IpBox1)
            {
                IpBox2.Focus();
                MauiProgram.settingsAcessor.ServerFieldA = int.Parse(IpBox1.Text);
            }
            else if (entry == IpBox2)
            {
                IpBox3.Focus();
                MauiProgram.settingsAcessor.ServerFieldB = int.Parse(IpBox2.Text);
            }
            else if (entry == IpBox3)
            {
                IpBox4.Focus();
                MauiProgram.settingsAcessor.ServerFieldC = int.Parse(IpBox3.Text);
            }
            else if (entry == IpBox4)
            {
                PortEntry.Focus();
                MauiProgram.settingsAcessor.ServerFieldD = int.Parse(IpBox4.Text);
            }
            else if (entry == PortEntry)
            {
                PortEntry.Unfocus();
                MauiProgram.settingsAcessor.ServerPort = int.Parse(PortEntry.Text);
            }
        }
    }

    private async void OnConnectClicked(object sender, EventArgs e)
    {
        if (MauiProgram.socketConnection != null)
        {
            MauiProgram.socketConnection.Stop();
            MauiProgram.socketConnection = null;

            UpdateConnectButton(false);
        }
        else
        {
            string ip = $"{IpBox1.Text}.{IpBox2.Text}.{IpBox3.Text}.{IpBox4.Text}";
            string portText = PortEntry.Text;

            if (!IPAddress.TryParse(ip, out _))
            {
                if (Application.Current?.Windows.FirstOrDefault()?.Page != null)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert(AppResources.Error, AppResources.Invalid_IP_address, "OK");
                    });
                }
                return;
            }

            if (!int.TryParse(portText, out int port) || port < 1 || port > 65535)
            {
                if (Application.Current?.Windows.FirstOrDefault()?.Page != null)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert(AppResources.Error, AppResources.Invalid_port, "OK");
                    });
                }
                return;
            }

            MauiProgram.settingsAcessor.ServerFieldC = int.Parse(IpBox3.Text);
            MauiProgram.settingsAcessor.ServerFieldD = int.Parse(IpBox4.Text);
            MauiProgram.settingsAcessor.ServerPort = int.Parse(PortEntry.Text);

            ConnectBTN.IsEnabled = false;

            MauiProgram.socketConnection = new Socket_Connection(ip, port);
            await MauiProgram.socketConnection.StartAsync(ConnectBTN, PingLabel);
        }
    }

    private async void OnFabClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (!_size_changed)
        {
            if (width > 0)
            {
                _size_changed = true;

                await Task.Delay(1000);

                if (!this.IsLoaded) return;
                await ScrollToEnd();
            }
        }
    }
}