namespace Motion_Joystick.Routes.Templates.Controllers;

public partial class Template2 : ContentPage
{
    public Template2()
    {
        InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);

        if (MauiProgram.settingsAcessor.UpSideDown)
        {
            Parent.ScaleX = -1;
            Parent.ScaleY = -1;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Navigation.RemovePage(this);
    }

    protected override bool OnBackButtonPressed()
    {
        return true; // suppress back button
    }
}