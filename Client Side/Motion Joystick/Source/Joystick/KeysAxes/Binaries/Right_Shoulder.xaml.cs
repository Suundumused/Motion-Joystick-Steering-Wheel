using Motion_Joystick.Source.Joystick.Map_Keys;

namespace Motion_Joystick.Source.Joystick.KeysAxis.Binaries;

public partial class Right_Shoulder : ContentView //area de colisão
{
	public Right_Shoulder()
	{
		InitializeComponent();

        Custom_SetSize();
	}

    private async void Button_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("2");
    }

    private async void Button_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("2");
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