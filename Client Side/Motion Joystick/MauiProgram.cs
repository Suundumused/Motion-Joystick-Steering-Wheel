using Microsoft.Extensions.Logging;
using Motion_Joystick.Source.IO.DataCache;
using Motion_Joystick.Source.Joystick.Map_Keys;
using Motion_Joystick.Source.Network;

using System.Diagnostics;
using CommunityToolkit.Maui;


namespace Motion_Joystick
{
    public static class MauiProgram
    {
        public static CancellationTokenSource? _gcLoopCts;

        public static SettingsCache settingsAcessor = new SettingsCache();
        public static Socket_Connection? socketConnection;

        public static Controls joyControls = new Controls();

        public static readonly double density = DeviceDisplay.MainDisplayInfo.Density;
        public static readonly double dpi = density * 160;

        public static readonly double main_density_preference = DeviceDisplay.MainDisplayInfo.Density / 2.75;

        public static MauiApp CreateMauiApp()
        {
            Console.WriteLine($"Densidade: {density}");

            MauiAppBuilder? builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //_ = Diag(); // Explicitly discard the task to avoid CS4014 warning

            //_gcLoopCts = new CancellationTokenSource();
            //_ = ForceGarbageCollectionLoop(_gcLoopCts.Token);

            #if DEBUG
                builder.Logging.AddDebug();
            #endif
                return builder.Build();
        }

        private static async Task Diag()
        {
            while (true)
            {
                long memoriaUsada = Process.GetCurrentProcess().PrivateMemorySize64;
                long memoriaTrabalhada = Process.GetCurrentProcess().WorkingSet64;

                Console.WriteLine($"Used memory (Private): {memoriaUsada / (1024 * 1024)} MB");
                Console.WriteLine($"Workload memory (Working Set): {memoriaTrabalhada / (1024 * 1024)} MB");

                await Task.Delay(1000);
            }
        }

        private static async Task ForceGarbageCollectionLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Run(() =>
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                });

                await Task.Delay(TimeSpan.FromSeconds(90), token); // Adjust interval as needed
            }
        }
    }
}
