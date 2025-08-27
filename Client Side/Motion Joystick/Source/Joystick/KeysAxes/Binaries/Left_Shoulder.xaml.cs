using Motion_Joystick.Source.Joystick.Map_Keys;

namespace Motion_Joystick.Source.Joystick.KeysAxes.Binaries;

public partial class Left_Shoulder : ContentView //área de colisão
{
    public Left_Shoulder()
    {
        InitializeComponent();

        Custom_SetSize();
    }

    private async void Button_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("3");
    }

    private async void Button_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("3");
    }

    private void Custom_SetSize()
    {
        double as_size = 156 * MauiProgram.main_density_preference;

        if (as_size < 127)
        {
            Pannel.WidthRequest = 127;
            Pannel.HeightRequest = 127;
        }
        else
        {
            Pannel.WidthRequest = as_size;
            Pannel.HeightRequest = as_size;
        }
    }
}