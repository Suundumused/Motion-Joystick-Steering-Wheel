#if ANDROID
    using Android.Content.Res;
    using Android.Graphics;
#endif
using Microsoft.Maui.Handlers;

using Motion_Joystick.Routes.AsConnection;
using Motion_Joystick.Routes.Templates;


namespace Motion_Joystick
{
    public partial class MainPage : ContentPage
    {
        private bool _size_changed = false;

        public MainPage()
        {
            InitializeComponent();

            Logo.HeightRequest = 185 * MauiProgram.main_density_preference;

            #if ANDROID
                EntryHandler.Mapper.AppendToMapping(nameof(Entry), (IEntryHandler handler, IEntry view) =>
                {
                    handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.LightSlateGray);
                });
            #endif
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            await Launcher.Default.OpenAsync("https://github.com/Suundumused/Motion-Joystick-Steering-Wheel/blob/main/Server%20Side/README.MD");
        }

        private async void OnOpenSetupClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SetupConnection());
        }

        private async void OnOpenTemplatesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TemplatesList());
        }

        private async Task ScrollToEnd()
        {
            await MainScrollView.ScrollToAsync(Setup, ScrollToPosition.End, true);
        }

        protected override async void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (!_size_changed)
            {
                if (width > 0)
                {
                    _size_changed = true;

                    await Task.Delay(1000);

                    if (!this.IsLoaded) return;
                    await ScrollToEnd();
                }
            }
        }
    }
}
