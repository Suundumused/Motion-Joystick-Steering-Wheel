namespace Motion_Joystick.Source.Joystick.KeysAxes.Axes;

using Microsoft.Maui.Controls;
using Motion_Joystick.Source.Joystick.Map_Keys;

public partial class RightDirectional : ContentView
{
    private double centerX, centerY, radius;
    private double[] final_pack_value = new double[] { 0.0, 0.0 };

    private bool _size_changed = false;

    public event EventHandler<(double X, double Y)>? JoystickMoved;

    public RightDirectional()
    {
        InitializeComponent();

        Custom_SetSize();
    }

    private async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:

                Controls.VibrateActive();
                break;

            case GestureStatus.Running:

                double x = e.TotalX;
                double y = e.TotalY;
                double magnitude = Math.Sqrt(x * x + y * y);

                // Aplica a movimentação da imagem
                if (magnitude <= radius + 2.5) // Corrected '^' to '*'
                {
                    AnalogImage.TranslationX = x;
                    AnalogImage.TranslationY = y;
                }
                else
                {
                    double scale = (radius + 2.5) / magnitude;
                    AnalogImage.TranslationX = x * scale;
                    AnalogImage.TranslationY = y * scale;
                }

                double finalXValue = AnalogImage.TranslationX / radius;
                double finalYValue = AnalogImage.TranslationY / radius;

                if (finalXValue > 1)
                {
                    finalXValue = 1.0;
                }
                else if (finalXValue < -1)
                {
                    finalXValue = -1.0;
                }

                if (finalYValue > 1)
                {
                    finalYValue = 1.0;
                }
                else if (finalYValue < -1)
                {
                    finalYValue = -1.0;
                }

                final_pack_value[0] = finalXValue;
                final_pack_value[1] = finalYValue;

                break;

            case GestureStatus.Completed:

                await AnalogImage.TranslateTo(0, 0, 100);

                final_pack_value[0] = 0.0;
                final_pack_value[1] = 0.0;

                Controls.VibrateDisable();
                break;

            case GestureStatus.Canceled:

                await AnalogImage.TranslateTo(0, 0, 100);

                final_pack_value[0] = 0.0;
                final_pack_value[1] = 0.0;
                break;
        }

        await Controls.Send_axes("rj", final_pack_value);
    }

    private void Custom_SetSize()
    {
        double a_size = 133 * MauiProgram.main_density_preference;
        double b_size = 89 * MauiProgram.main_density_preference;

        if (a_size < 89)
        {
            JoystickArea.WidthRequest = 89;
            JoystickArea.HeightRequest = 89;
        }
        else
        {
            JoystickArea.WidthRequest = a_size;
            JoystickArea.HeightRequest = a_size;
        }

        if (b_size < 67)
        {
            AnalogImage.WidthRequest = 67;
            AnalogImage.HeightRequest = 67;
        }
        else
        {
            AnalogImage.WidthRequest = b_size;
            AnalogImage.HeightRequest = b_size;
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
                radius = JoystickArea.Width / 2;
            }
        }
    }
}