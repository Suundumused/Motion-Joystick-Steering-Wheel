using Motion_Joystick.Source.Joystick.Map_Keys;

namespace Motion_Joystick.Source.Joystick.KeysAxes.Binaries;

public partial class DownUp_Only : ContentView
{
    private bool _size_allocated { get; set; } = false;

    public DownUp_Only()
    {
        InitializeComponent();
    }

    private async void Down_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("7");
    }

    private async void Down_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("7");
    }

    private async void Up_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("6");
    }

    private async void Up_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("6");
    }
}