namespace Motion_Joystick.Source.Joystick.KeysAxis.Binaries;

public partial class ExitButton : ContentView
{
	public ExitButton()
	{
		InitializeComponent();

        Custom_SetSize();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void Custom_SetSize()
    {
        double b_size = 75 * MauiProgram.main_density_preference;

        if (b_size < 67) 
        {
            Button0.WidthRequest = 67;
            Button0.HeightRequest = 67;
        }
        else 
        {
            Button0.WidthRequest = b_size;
            Button0.HeightRequest = b_size;
        }

        Button0.CornerRadius = (int)(b_size);
    }
}