using Motion_Joystick.Routes.Templates.Controllers;

namespace Motion_Joystick.Routes.Templates;

public partial class TemplatesList : ContentPage
{
    public TemplatesList()
    {
        InitializeComponent();
    }

    private async void template1_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new template1());
    }

    private async void template2_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Template2());
    }

    private async void template3_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Template3());
    }

    private async void template4_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Template4());
    }

    private async void template5_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Template5());
    }

    private async void template6_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Template6());
    }

    private async void template7_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Template7());
    }

    private async void template8_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Template8());
    }

    private async void template9_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Template9());
    }

    private async void template10_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Template10());
    }

    private async void template11_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Template11());
    }

    private async void template12_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Template12());
    }

    private void OnFabClicked(object sender, EventArgs e)
    {
        if (Application.Current != null && Application.Current.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new MainPage());
        }
    }
}