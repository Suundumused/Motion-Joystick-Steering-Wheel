using Motion_Joystick.Source.Joystick.Map_Keys;

namespace Motion_Joystick.Source.Joystick.KeysAxes.Binaries;

public partial class Right_Only : ContentView
{
    private bool _size_allocated { get; set; } = false;

    public Right_Only()
    {
        InitializeComponent();
    }

    private async void Right_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("5");
    }

    private async void Right_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("5");
    }
}