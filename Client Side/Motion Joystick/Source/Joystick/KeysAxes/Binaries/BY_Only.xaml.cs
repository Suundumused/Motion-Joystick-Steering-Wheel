using Motion_Joystick.Source.Joystick.Map_Keys;

namespace Motion_Joystick.Source.Joystick.KeysAxes.Binaries;

public partial class BY_Only : ContentView
{
    private bool _size_allocated { get; set; } = false;

    public BY_Only()
    {
        InitializeComponent();
    }

    private async void B_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("14");
    }

    private async void B_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("14");
    }

    private async void Y_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("15");
    }

    private async void Y_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("15");
    }
}