using Motion_Joystick.Source.Joystick.Map_Keys;

namespace Motion_Joystick.Source.Joystick.KeysAxes.Binaries;

public partial class LeftBinaries : ContentView
{
    private bool _size_changed { get; set; } = false;

    public LeftBinaries()
    {
        InitializeComponent();

        Custom_SetSize();
    }

    private async void Down_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("7");
    }

    private async void Down_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("7");
    }

    private async void Left_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("4");
    }

    private async void Left_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("4");
    }

    private async void Up_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("6");
    }

    private async void Up_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("6");
    }

    private async void Right_Pressed(object sender, EventArgs e)
    {
        await Controls.Press_button("5");
    }

    private async void Right_Released(object sender, EventArgs e)
    {
        await Controls.Release_button("5");
    }

    private void MainTable_SizeChanged(object sender, EventArgs e)
    {
        if (!_size_changed)
        {
            _size_changed = true;

            MyEllipse.WidthRequest = MainTable.Width;
            MyEllipse.HeightRequest = MainTable.Height;
        }
    }

    private void Custom_SetSize()
    {
        double size_a = 77 * MauiProgram.main_density_preference;
        double size_b = 25 * MauiProgram.main_density_preference;

        if (size_a < 56)
        {
            Down.WidthRequest = 56;
            Down.HeightRequest = 56;

            Left.WidthRequest = 56;
            Left.HeightRequest = 56;

            Right.WidthRequest = 56;
            Right.HeightRequest = 56;

            Up.WidthRequest = 56;
            Up.HeightRequest = 56;
        }
        else
        {
            Down.WidthRequest = size_a;
            Down.HeightRequest = size_a;

            Left.WidthRequest = size_a;
            Left.HeightRequest = size_a;

            Right.WidthRequest = size_a;
            Right.HeightRequest = size_a;

            Up.WidthRequest = size_a;
            Up.HeightRequest = size_a;
        }

        if (size_b < 16)
        {
            Down.FontSize = 16;
            Up.FontSize = 16;
            Left.FontSize = 16;
            Right.FontSize = 16;
        }
        else
        {
            Down.FontSize = size_b;
            Up.FontSize = size_b;
            Left.FontSize = size_b;
            Right.FontSize = size_b;
        }
    }
}