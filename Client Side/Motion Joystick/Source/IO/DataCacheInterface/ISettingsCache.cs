namespace Motion_Joystick.Source.IO.DataCacheInterface
{
    internal interface ISettingsCache
    {
        internal double RefreshRate { get; set; }
        internal double Sensibility { get; set; }
        internal double TriggerSensibility { get; set; }

        internal bool UpSideDown { get; set; }
        internal bool ConstantAcceleration { get; set; }
        internal bool PowerSavingMode { get; set; }

        internal int ServerFieldA { get; set; }
        internal int ServerFieldB { get; set; }
        internal int ServerFieldC { get; set; }
        internal int ServerFieldD { get; set; }

        internal int ServerPort { get; set; }
    }
}