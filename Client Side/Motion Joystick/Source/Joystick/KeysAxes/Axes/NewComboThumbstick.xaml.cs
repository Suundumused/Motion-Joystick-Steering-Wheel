using Motion_Joystick.Source.Joystick.Map_Keys;

namespace Motion_Joystick.Source.Joystick.KeysAxes.Axes;

public partial class NewComboThumbstick : ContentView
{
    private bool _size_changed = false;

    private double JoystickArea_Width { get; set; }
    private double halfWidth { get; set; }
    private double halfWidth_2 { get; set; }
    private double halfWidth__2 { get; set; }

    private double _start_offset { get; set; } = 0.0;
    private double last_gesture_value { get; set; } = 0.0;
    private double last_double_final_x_value {get; set;} = 0.0;

    private bool last_gesture_state { get; set; } = false;
    private bool first_press { get; set; } = false;

    public NewComboThumbstick()
	{
		InitializeComponent();
    }

    private async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                first_press = true;

                if (!last_gesture_state)
                {
                    _start_offset = 0.0;
                }
                else
                {
                    if (last_gesture_value > 0.5)
                    {
                        if (last_gesture_value > halfWidth_2)
                        {
                            _start_offset = halfWidth_2;
                        }
                        else if (last_gesture_value < halfWidth__2)
                        {
                            _start_offset = halfWidth__2;
                        }
                        else
                        {
                            _start_offset = last_gesture_value;
                        }
                    }
                    else
                    {
                        _start_offset = 0.0;
                    }
                }

                Controls.VibrateActive();
                break;

            case GestureStatus.Running:
                last_gesture_value = e.TotalX;

                if (first_press) 
                {
                    first_press = false;

                    if (last_gesture_value < 0)
                    {
                        _start_offset = _start_offset + 10;
                    }
                    else
                    {
                        _start_offset = _start_offset - 10;
                    }
                }

                double private_offset = _start_offset + last_gesture_value;

                if (private_offset > halfWidth_2)
                {
                    AnalogImage.TranslationX = halfWidth_2;
                }
                else if (private_offset < halfWidth__2)
                {
                    AnalogImage.TranslationX = halfWidth__2;
                }
                else
                {
                    AnalogImage.TranslationX = private_offset;
                }

                double finalXValue = (AnalogImage.TranslationX + halfWidth) / JoystickArea_Width;

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

                await Controls.Send_axes("17", last_double_final_x_value);
                break;

            case GestureStatus.Completed:
                if (!MauiProgram.settingsAcessor.ConstantAcceleration)
                {
                    last_gesture_state = false;

                    await AnalogImage.TranslateTo(0, 0, 100);

                    last_double_final_x_value = 0.5;

                    await Controls.Send_axes("17", last_double_final_x_value, true);
                }
                else if (last_double_final_x_value < 0.5)
                {
                    last_gesture_state = false;

                    await AnalogImage.TranslateTo(0, 0, 100);

                    last_double_final_x_value = 0.5;

                    await Controls.Send_axes("17", last_double_final_x_value, true);
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

                    last_double_final_x_value = 0.5;
                    
                    await Controls.Send_axes("17", last_double_final_x_value, true);
                }
                else if (last_double_final_x_value < 0.5)
                {
                    last_gesture_state = false;

                    await AnalogImage.TranslateTo(0, 0, 100);

                    last_double_final_x_value = 0.5;

                    await Controls.Send_axes("17", last_double_final_x_value, true);
                }
                else
                {
                    last_gesture_state = true;
                }
                break;
        }
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

                JoystickArea_Width = JoystickArea.Width / MauiProgram.settingsAcessor.TriggerSensibility;

                halfWidth = JoystickArea_Width / 2;
                halfWidth_2 = halfWidth + 12;
                halfWidth__2 = halfWidth * -1 - 12;
            }
        }
    }
}