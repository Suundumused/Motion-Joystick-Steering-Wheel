using Motion_Joystick.Source.Joystick.Map_Keys;

namespace Motion_Joystick.Source.Joystick.KeysAxes.Binaries;

public partial class Pause_Select : ContentView
{
    public Pause_Select()
    {
        InitializeComponent();

        Custom_SetSize();
    }

    private async void Button_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("11");
    }

    private async void Button_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("11");
    }

    private async void Button_Pressed_1(object sender, EventArgs e)
    {
        await Controls.Press_button("12");
    }

    private async void Button_Released_1(object sender, EventArgs e)
    {
        await Controls.Release_button("12");
    }

    private void Custom_SetSize()
    {
        double y_size = 75 * MauiProgram.main_density_preference;

        int corner_size = (int)y_size;

        double font_size = 12 * MauiProgram.main_density_preference;

        if (y_size < 67)
        {
            ButtonA.WidthRequest = 67;
            ButtonB.WidthRequest = 67;
            ButtonA.HeightRequest = 67;
            ButtonB.HeightRequest = 67;
        }
        else
        {
            ButtonA.WidthRequest = y_size;
            ButtonB.WidthRequest = y_size;
            ButtonA.HeightRequest = y_size;
            ButtonB.HeightRequest = y_size;
        }

        if (font_size < 12)
        {
            ButtonA.FontSize = 12;
            ButtonB.FontSize = 12;
        }
        else
        {
            ButtonA.FontSize = font_size - 2;
            ButtonB.FontSize = font_size - 2;
        }

        if (corner_size < 33)
        {
            ButtonA.CornerRadius = 33;
            ButtonB.CornerRadius = 33;
        }
        else
        {
            ButtonA.CornerRadius = corner_size;
            ButtonB.CornerRadius = corner_size;
        }
    }
}