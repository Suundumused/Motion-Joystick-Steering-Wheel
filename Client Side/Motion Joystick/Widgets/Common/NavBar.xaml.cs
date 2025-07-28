using Motion_Joystick.Routes.Settings;
using Motion_Joystick.Routes.Templates;
using Motion_Joystick.Routes.Info;

using Motion_Joystick.Routes.AsConnection;

namespace Motion_Joystick.Widgets.Common;

public partial class NavBar : ContentView
{
    private bool _hasAppeared = false;

    private Page? currentPage = null;

    public NavBar()
    {
        InitializeComponent();
        this.SizeChanged += OnSizeChanged;
    }

    private void OnInit()
    {
        IReadOnlyList<Page>? navigationStack = Navigation.NavigationStack;
        if (navigationStack != null && navigationStack.Count > 0)
        {
            currentPage = navigationStack[^1]; // Use indexer to access the last element directly

            Console.WriteLine($"Current page: {currentPage}");
        }
        else if (Application.Current?.Windows.FirstOrDefault()?.Page is NavigationPage navigationPage) // Updated to use Windows[0].Page
        {
            currentPage = navigationPage.CurrentPage;
        }

        if (currentPage is About)
        {
            infobtn.IsVisible = false;
        }
        else if (currentPage is MySettings)
        {
            settingsbtn.IsVisible = false;
        }
        else if (currentPage is TemplatesList)
        {
            templatesbtn.IsVisible = false;
        }
        else if (currentPage is SetupConnection)
        {
            return;
        }
        else
        {
            templatesbtn.IsVisible = false;
        }
    }

    private async void OnFabClicked(object sender, EventArgs e)
    {
        if (Application.Current != null) // Ensure Application.Current is not null
        {
            if (currentPage is MainPage)
            {
                Application.Current.Quit();
            }
            else
            {
                await Navigation.PopAsync();
            }
        }
        else
        {
            Console.WriteLine("Application.Current is null. Unable to proceed.");
        }
    }

    private async void SettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MySettings());
    }

    private async void Templates_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TemplatesList());
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new About());
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        if (!_hasAppeared && this.Width > 0 && this.Height > 0)
        {
            _hasAppeared = true;
            OnInit();
        }
    }
}
