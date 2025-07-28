namespace Motion_Joystick.Routes.Settings;

public partial class MySettings : ContentPage
{
    CancellationTokenSource? _cts = null;

    public MySettings()
    {
        InitializeComponent();
        LoadSettings();
    }


    private void LoadSettings() //ConstantAcceleration
    {
        double refresh_value = 1 / (MauiProgram.settingsAcessor.RefreshRate / 1000);

        ValueSlider.Value = refresh_value;
        ValueLabel.Text = ((int)Math.Round(refresh_value)).ToString();

        ValueLabelSensibility.Text = MauiProgram.settingsAcessor.Sensibility.ToString("F2");
        ValueSliderSensibility.Value = MauiProgram.settingsAcessor.Sensibility;

        ValueLabelTriggerSensibility.Text = MauiProgram.settingsAcessor.TriggerSensibility.ToString("F2");
        ValueSliderTriggerSensibility.Value = MauiProgram.settingsAcessor.TriggerSensibility;

        InvertedJoystickSwitch.IsToggled = MauiProgram.settingsAcessor.UpSideDown;
        ConstantAcceleration.IsToggled = MauiProgram.settingsAcessor.ConstantAcceleration;
        PowerSavingMode.IsToggled = MauiProgram.settingsAcessor.PowerSavingMode;
        Vibration.IsToggled = MauiProgram.settingsAcessor.Vibration;
    }

    private void OnResetClicked(object sender, EventArgs e)
    {
        MauiProgram.settingsAcessor.RefreshRate = 16.666666666666666666666666666667;
        MauiProgram.settingsAcessor.Sensibility = 1.5;
        MauiProgram.settingsAcessor.TriggerSensibility = 1.5;

        MauiProgram.settingsAcessor.UpSideDown = false;
        MauiProgram.settingsAcessor.ConstantAcceleration = false;
        MauiProgram.settingsAcessor.PowerSavingMode = false;
        MauiProgram.settingsAcessor.Vibration = false;

        ValueLabel.Text = "60";
        ValueSlider.Value = 60;

        ValueLabelTriggerSensibility.Text = "1.50";
        ValueSliderTriggerSensibility.Value = 1.5;

        ValueLabelSensibility.Text = "1.50";
        ValueSliderSensibility.Value = 1.5;

        InvertedJoystickSwitch.IsToggled = false;
        ConstantAcceleration.IsToggled = false;
        PowerSavingMode.IsToggled = false;
        Vibration.IsToggled = false;

        MauiProgram.settingsAcessor.ServerFieldA = 192;
        MauiProgram.settingsAcessor.ServerFieldB = 168;
        MauiProgram.settingsAcessor.ServerFieldC = 1;
        MauiProgram.settingsAcessor.ServerFieldD = 2;

        MauiProgram.settingsAcessor.ServerPort = 3470;
    }


    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        ValueLabel.Text = ((int)Math.Round(e.NewValue)).ToString();

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        _ = Task.Delay(500, _cts.Token).ContinueWith(t =>
        {
            if (!t.IsCanceled)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Considera que o usuário parou de interagir com o slider
                    OnSliderReleased();
                });
            }
        });
    }

    private void OnSliderReleased()
    {
        double refresh_value = ValueSlider.Value;
        MauiProgram.settingsAcessor.RefreshRate =  1 / refresh_value * 1000;

        ValueLabel.Text = ((int)Math.Round(refresh_value)).ToString();
    }

    private void OnSensibilityValueChanged(object sender, ValueChangedEventArgs e)
    {
        ValueLabelSensibility.Text = e.NewValue.ToString("F2");

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        _ = Task.Delay(500, _cts.Token).ContinueWith(t =>
        {
            if (!t.IsCanceled)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Considera que o usuário parou de interagir com o slider
                    OnSliderSensibilityReleased();
                });
            }
        });
    }

    private void OnSliderSensibilityReleased()
    {
        MauiProgram.settingsAcessor.Sensibility = ValueSliderSensibility.Value;

        ValueLabelSensibility.Text = ValueSliderSensibility.Value.ToString("F2");
    }

    private void OnTriggerSensibilityValueChanged(object sender, ValueChangedEventArgs e)
    {
        ValueLabelTriggerSensibility.Text = e.NewValue.ToString("F2");

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        _ = Task.Delay(500, _cts.Token).ContinueWith(t =>
        {
            if (!t.IsCanceled)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Considera que o usuário parou de interagir com o slider
                    OnSliderTriggerSensibilityReleased();
                });
            }
        });
    }

    private void OnSliderTriggerSensibilityReleased()
    {
        MauiProgram.settingsAcessor.TriggerSensibility = ValueSliderTriggerSensibility.Value;

        ValueLabelTriggerSensibility.Text = ValueSliderTriggerSensibility.Value.ToString("F2");
    }

    private void OnUpSideDownToggled(object sender, ToggledEventArgs e)
    {
        MauiProgram.settingsAcessor.UpSideDown = InvertedJoystickSwitch.IsToggled;
    }

    private void OnConstantAccelerationToggled(object sender, ToggledEventArgs e)
    {
        MauiProgram.settingsAcessor.ConstantAcceleration = ConstantAcceleration.IsToggled;
    }

    private void OnPowerSavingModeToggled(object sender, ToggledEventArgs e)
    {
        MauiProgram.settingsAcessor.PowerSavingMode = PowerSavingMode.IsToggled;
    }

    private void OnVibrationToggled(object sender, ToggledEventArgs e)
    {
        MauiProgram.settingsAcessor.Vibration = Vibration.IsToggled;
    }

    private async void OnFabClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
