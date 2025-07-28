namespace Motion_Joystick.Routes.Templates.Controllers;

public partial class Template8 : ContentPage
{
    public Template8()
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

    public static class AppConstants
    {
        public static double AsInvertVJOY
        {
            get
            {
                // Ensure MauiProgram.settingsAcessor and InvertJoystick are accessible and properly defined.
                if (MauiProgram.settingsAcessor != null && MauiProgram.settingsAcessor.UpSideDown)
                {
                    return -1;
                }
                return 1;
            }
        }
    }
}
