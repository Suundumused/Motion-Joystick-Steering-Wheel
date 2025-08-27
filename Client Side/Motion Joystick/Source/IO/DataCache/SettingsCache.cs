using Motion_Joystick.Source.IO.DataCacheInterface;
using Motion_Joystick.Source.Joystick.Map_Keys;

namespace Motion_Joystick.Source.IO.DataCache
{
    public class SettingsCache : ISettingsCache
    {
        private bool _constantAcceleration = Preferences.Get("ConstantAcceleration", false);

        public bool ConstantAcceleration
        {
            get => _constantAcceleration;
            set
            {
                _constantAcceleration = value;
                Preferences.Set("ConstantAcceleration", value);
            }
        }

        private double _refreshRate = Preferences.Get("RefreshRate", 16.666666666666666666666666666667);

        public double RefreshRate
        {
            get => _refreshRate;
            set
            {
                _refreshRate = value;
                Preferences.Set("RefreshRate", value);

                _ = Controls.Send_config("rr", value / 1000);
                if (MauiProgram.joyControls._running)
                {
                    MauiProgram.joyControls.StopAccelerometer();

                    Task.Delay((int)value * 2).Wait();

                    MauiProgram.joyControls.StartAccelerometer();
                }

                if (MauiProgram.socketConnection is not null)
                {
                    MauiProgram.socketConnection._refresh_rate = (int)Math.Floor(value);
                }
            }
        }

        private double _sensibility = Preferences.Get("Sensibility", 1.5);

        public double Sensibility
        {
            get => _sensibility;
            set
            {
                _sensibility = value;
                Preferences.Set("Sensibility", value);
            }
        }

        private double _triggerSensibility = Preferences.Get("TriggerSensibility", 1.5);

        public double TriggerSensibility
        {
            get => _triggerSensibility;
            set
            {
                _triggerSensibility = value;
                Preferences.Set("TriggerSensibility", value);
            }
        }

        private bool _UpSideDown = Preferences.Get("UpSideDown", false);

        public bool UpSideDown
        {
            get => _UpSideDown;
            set
            {
                _UpSideDown = value;
                Preferences.Set("UpSideDown", value);
            }
        }

        private bool _powerSavingMode = Preferences.Get("PowerSavingMode", false);

        public bool PowerSavingMode
        {
            get => _powerSavingMode;
            set
            {
                _powerSavingMode = value;
                Preferences.Set("PowerSavingMode", value);

                if (MauiProgram.joyControls._running)
                {
                    MauiProgram.joyControls.StopAccelerometer();

                    Task.Delay((int)_refreshRate * 2).Wait();

                    MauiProgram.joyControls.StartAccelerometer();
                }
            }
        }

        private bool _vibration = Preferences.Get("Vibration", false);

        public bool Vibration
        {
            get => _vibration;
            set
            {
                _vibration = value;
                Preferences.Set("Vibration", value);
            }
        }

        private int _serverFieldA = Preferences.Get("ServerFieldA", 192);

        public int ServerFieldA
        {
            get => _serverFieldA;
            set
            {
                _serverFieldA = value;
                Preferences.Set("ServerFieldA", value);
            }
        }

        private int _serverFieldB = Preferences.Get("ServerFieldB", 168);

        public int ServerFieldB
        {
            get => _serverFieldB;
            set
            {
                _serverFieldB = value;
                Preferences.Set("ServerFieldB", value);
            }
        }

        private int _serverFieldC = Preferences.Get("ServerFieldC", 1);

        public int ServerFieldC
        {
            get => _serverFieldC;
            set
            {
                _serverFieldC = value;
                Preferences.Set("ServerFieldC", value);
            }
        }

        private int _serverFieldD = Preferences.Get("ServerFieldD", 2);

        public int ServerFieldD
        {
            get => _serverFieldD;
            set
            {
                _serverFieldD = value;
                Preferences.Set("ServerFieldD", value);
            }
        }

        private int _serverPort = Preferences.Get("ServerPort", 3470);

        public int ServerPort
        {
            get => _serverPort;
            set
            {
                _serverPort = value;
                Preferences.Set("ServerPort", value);
            }
        }
    }
}