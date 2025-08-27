using Motion_Joystick.Routes.Templates.Controllers;

namespace Motion_Joystick
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = AppTheme.Light;
            }

            Routing.RegisterRoute("Template2", typeof(Template2));
            Routing.RegisterRoute("template1", typeof(template1));
            Routing.RegisterRoute("Template3", typeof(Template3));
            Routing.RegisterRoute("Template4", typeof(Template4));
            Routing.RegisterRoute("Template5", typeof(Template5));
            Routing.RegisterRoute("Template6", typeof(Template6));
            Routing.RegisterRoute("Template7", typeof(Template7));
            Routing.RegisterRoute("Template8", typeof(Template8));
            Routing.RegisterRoute("Template9", typeof(Template9));
            Routing.RegisterRoute("Template10", typeof(Template10));
            Routing.RegisterRoute("Template11", typeof(Template11));
            Routing.RegisterRoute("Template12", typeof(Template11));

            this.Navigating += OnShellNavigating;
        }

        private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
        {
            if (e.Source == ShellNavigationSource.Pop && Current.CurrentState.Location.OriginalString != "//MainPage")
            {
                e.Cancel();

                await Navigation.PopAsync();
            }
        }
    }
}