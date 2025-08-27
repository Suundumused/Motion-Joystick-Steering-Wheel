using Motion_Joystick.Source.Joystick.Map_Keys;

namespace Motion_Joystick.Source.Joystick.KeysAxes.Binaries;

public partial class AX_Only : ContentView
{
    private bool _size_allocated { get; set; } = false;

    public AX_Only()
    {
        InitializeComponent();
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
}