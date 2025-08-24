// MauiProgram.cs
using CommunityToolkit.Maui;
using GCMS.MathHouse.BL;
using GCMS.MathHouse.UI.Common;
using GCMS.MathHouse.UI.MainPage;

namespace GCMS.MathHouse
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().UseMauiCommunityToolkitMediaElement().UseMauiCommunityToolkitMediaElement();

            // Services
            builder.Services.AddSingleton<GameStateService>();
            builder.Services.AddSingleton<GameLevelService>();
            builder.Services.AddSingleton<ResponsiveLayoutService>();
            builder.Services.AddSingleton<IDeviceScreenService, DeviceScreenService>();
            builder.Services.AddSingleton<IngredientQuestService>();
            builder.Services.AddSingleton<FreeMathGameFlowController>();
            builder.Services.AddSingleton<GameResetService>();
            builder.Services.AddSingleton<GameFlowController>(); // Add main GameFlowController

            // Layout Managers
            builder.Services.AddTransient<FreeMathLayoutManager>();

            // UI Components
            builder.Services.AddTransient<NumericKeyboard>();

            // Pages
            builder.Services.AddTransient<FreeMathPage>();
            builder.Services.AddTransient<FreeMathProblemPage>();
            builder.Services.AddTransient<AboutPage>(); // Add AboutPage for dependency injection

            var mauiApp = builder.Build();

            // 1️⃣ Intercept excepțiile CLR neprinse
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                HandleGlobalException("AppDomain.CurrentDomain.UnhandledException", ex);
            };
            // 2️⃣ Intercept Task-uri fără catch
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                HandleGlobalException("TaskScheduler.UnobservedTaskException", e.Exception);
                e.SetObserved(); // marchează ca „observed" ca să nu se oprească process-ul
            };
//#if WINDOWS
//            // 3️⃣ Pentru WinUI: excepții pe thread-ul UI
//            Application.Current.Windows[0].Dispatcher.UnhandledException += (sender, e) =>
//            {
//                HandleGlobalException("Dispatcher.UnhandledException", e.Exception);
//                e.Handled = true;
//            };
//#endif
#if ANDROID
            // 4️⃣ Pentru Android
            Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                HandleGlobalException("AndroidEnvironment.UnhandledExceptionRaiser", args.Exception);
                args.Handled = true;
            };
#endif
            return mauiApp;
        }



        static void HandleGlobalException(string source, Exception? ex)
        {
            // Aici poți loga excepția, trimite la un serviciu de telemetrie, afișa alertă etc.
            Console.WriteLine($"[{source}] {ex?.GetType().Name}: {ex?.Message}\n{ex?.StackTrace}");
            // Dacă vrei să afișezi un popup (trebuie să fii pe UI thread):
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Eroare neașteptată", $"{ex?.Message}", "Închide");
            });
        }
    }
}