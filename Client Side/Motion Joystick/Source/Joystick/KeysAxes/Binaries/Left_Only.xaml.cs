using Motion_Joystick.Source.Joystick.Map_Keys;

namespace Motion_Joystick.Source.Joystick.KeysAxis.Binaries;

public partial class Left_Only : ContentView
{
    private bool _size_allocated { get; set; } = false;

	public Left_Only()
	{
		InitializeComponent();
	}

    private async void Left_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("4");
    }

    private async void Left_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("4");
    }
}