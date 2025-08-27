using Motion_Joystick.Source.Joystick.Map_Keys;

namespace Motion_Joystick.Source.Joystick.KeysAxes.Axes;

public partial class RT_Thumbstick : ContentView
{
    private bool _size_changed = false;

    private double JoystickArea_width { get; set; }
    private double _width { get; set; }
    private double width_2 { get; set; }

    private double _start_offset { get; set; } = 0.0;
    private double last_gesture_value { get; set; } = 0.0;
    private double last_double_final_x_value { get; set; } = 0.0;

    private bool last_gesture_state { get; set; } = false;

    public RT_Thumbstick()
    {
        InitializeComponent();
    }

    private async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                if (!last_gesture_state)
                {
                    _start_offset = -10.0;
                }
                else
                {
                    if (last_gesture_value > 0.24)
                    {
                        if (last_gesture_value > width_2)
                        {
                            _start_offset = width_2;
                        }
                        else
                        {
                            _start_offset = last_gesture_value;
                        }
                    }
                    else
                    {
                        _start_offset = -10.0;
                    }
                }

                Controls.VibrateActive();
                break;

            case GestureStatus.Running:
                last_gesture_value = e.TotalX;

                double private_offset = _start_offset + last_gesture_value;

                if (private_offset > width_2)
                {
                    AnalogImage.TranslationX = width_2;
                }
                else if (private_offset < -12)
                {
                    AnalogImage.TranslationX = -12;
                }
                else
                {
                    AnalogImage.TranslationX = private_offset;
                }

                double finalXValue = AnalogImage.TranslationX / _width;

                if (finalXValue > 1)
                {
                    last_double_final_x_value = 1.0;
                }
                else if (finalXValue < 0)
                {
                    last_double_final_x_value = 0;
                }
                else
                {
                    last_double_final_x_value = finalXValue;
                }

                await Controls.Send_axes("rt", last_double_final_x_value);
                break;

            case GestureStatus.Completed:
                if (!MauiProgram.settingsAcessor.ConstantAcceleration)
                {
                    last_gesture_state = false;

                    await AnalogImage.TranslateTo(0, 0, 100);

                    last_double_final_x_value = 0.0;

                    await Controls.Send_axes("rt", last_double_final_x_value, true);
                }
                else if (last_double_final_x_value < 0.24)
                {
                    last_gesture_state = false;

                    await AnalogImage.TranslateTo(0, 0, 100);

                    last_double_final_x_value = 0.0;

                    await Controls.Send_axes("rt", last_double_final_x_value, true);
                }
                else
                {
                    last_gesture_state = true;
                }

                Controls.VibrateDisable();
                break;

            case GestureStatus.Canceled:
                if (!MauiProgram.settingsAcessor.ConstantAcceleration)
                {
                    last_gesture_state = false;

                    await AnalogImage.TranslateTo(0, 0, 100);

                    last_double_final_x_value = 0.0;

                    await Controls.Send_axes("rt", last_double_final_x_value, true);
                }
                else if (last_double_final_x_value < 0.24)
                {
                    last_gesture_state = false;

                    await AnalogImage.TranslateTo(0, 0, 100);

                    last_double_final_x_value = 0.0;

                    await Controls.Send_axes("rt", last_double_final_x_value, true);
                }
                else
                {
                    last_gesture_state = true;
                }

                break;
        }
    }

    public async Task ForceReset()
    {
        last_gesture_state = false;

        await AnalogImage.TranslateTo(0, 0, 100);

        last_double_final_x_value = 0.0;

        await Controls.Send_axes("rt", last_double_final_x_value, true);
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (!_size_changed)
        {
            if (width > 0)
            {
                _size_changed = true;

                AnalogImage.HeightRequest = MainTable.Height * 2 / 4;
                AnalogImage.WidthRequest = AnalogImage.HeightRequest;

                JoystickArea_width = JoystickArea.Width / MauiProgram.settingsAcessor.TriggerSensibility;

                _width = JoystickArea_width / 1.5;
                width_2 = _width + 12;
            }
        }
    }
}