using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;

namespace Motion_Joystick
{
    [Activity(Label = "@string/app_name", Theme = "@style/Maui.SplashTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public bool _is_S_Version = Build.VERSION.SdkInt >= BuildVersionCodes.S;
        public bool _is_R_Version = Build.VERSION.SdkInt >= BuildVersionCodes.R;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            UiModeManager? uiModeManager = GetUiModeManager();
            if (uiModeManager != null)
            {
                if (_is_S_Version)
                {
                    uiModeManager.SetApplicationNightMode((int)UiNightMode.No);
                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                }
                else
                {
                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                }
            }

            base.OnCreate(savedInstanceState);

            try
            {
                EnterImmersiveMode();
            }
            catch (Exception ex)
            {
                Android.Util.Log.Warn("MainActivity", $"Erro ao entrar no modo imersivo: {ex}");
            }
        }

        private UiModeManager? GetUiModeManager()
        {
            return GetSystemService(UiModeService) as UiModeManager;
        }

        private void EnterImmersiveMode()
        {
            if (Window != null)
            {
                if (_is_S_Version)
                {
                    // Fix for CA1416: Add runtime check for API level
                    Window.SetDecorFitsSystemWindows(false);
                }

                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window.DecorView.SystemUiFlags = //Use Insets COntroller instead
                    SystemUiFlags.LayoutStable |
                    SystemUiFlags.LayoutHideNavigation |
                    SystemUiFlags.LayoutFullscreen |
                    SystemUiFlags.HideNavigation |
                    SystemUiFlags.Fullscreen |
                    SystemUiFlags.ImmersiveSticky;

                Window.AddFlags(WindowManagerFlags.KeepScreenOn);

                Window.SetNavigationBarColor(Android.Graphics.Color.Transparent); //Update to .NET MAUI 9.0
                Window.SetStatusBarColor(Android.Graphics.Color.Transparent); //...
            }
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus)
                try
                {
                    EnterImmersiveMode();
                }
                catch (Exception ex)
                {
                    Android.Util.Log.Warn("MainActivity", $"Erro ao entrar no modo imersivo: {ex}");
                }
        }

        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();

                if (MauiProgram._gcLoopCts != null)
                {
                    MauiProgram._gcLoopCts.Cancel();
                }
            }
            catch {}
        }
    }
}
