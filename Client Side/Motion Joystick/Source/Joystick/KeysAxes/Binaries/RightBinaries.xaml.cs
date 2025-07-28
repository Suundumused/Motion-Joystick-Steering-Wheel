using Motion_Joystick.Source.Joystick.Map_Keys;

namespace Motion_Joystick.Source.Joystick.KeysAxis.Binaries;


public partial class RightBinaries : ContentView
{
	public RightBinaries()
	{
		InitializeComponent();

        Custom_SetSize();
	}

    private async void A_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("16");
    }

    private async void A_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("16");
    }

    private async void X_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("13");
    }

    private async void X_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("13");
    }

    private async void Y_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("15");
    }

    private async void Y_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("15");
    }

    private async void B_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("14");
    }

    private async void B_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("14");
    }

    private void Custom_SetSize()
    {
        int a_size_radius = 56;

        double a_size = 77 * MauiProgram.main_density_preference;
        double b_size = 25 * MauiProgram.main_density_preference;

        if (a_size < 56) 
        {
            a_size_radius = 56;

            A.WidthRequest = 56;
            A.HeightRequest = 56;

            X.WidthRequest = 56;
            X.HeightRequest = 56;

            Y.WidthRequest = 56;
            Y.HeightRequest = 56;

            B.WidthRequest = 56;
            B.HeightRequest = 56;
        }
        else 
        {
            a_size_radius = (int)a_size;

            A.WidthRequest = a_size;
            A.HeightRequest = a_size;

            X.WidthRequest = a_size;
            X.HeightRequest = a_size;

            Y.WidthRequest = a_size;
            Y.HeightRequest = a_size;

            B.WidthRequest = a_size;
            B.HeightRequest = a_size;
        }

        if (b_size < 16) 
        {
            A.FontSize = 16;
            X.FontSize = 16;
            Y.FontSize = 16;
            B.FontSize = 16;
        }
        else 
        {
            A.FontSize = b_size;
            X.FontSize = b_size;
            Y.FontSize = b_size;
            B.FontSize = b_size;
        }

        A.CornerRadius = a_size_radius;
        X.CornerRadius = a_size_radius;
        Y.CornerRadius = a_size_radius;
        B.CornerRadius = a_size_radius;
    }
}