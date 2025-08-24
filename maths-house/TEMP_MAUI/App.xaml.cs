using GCMS.MathHouse.BL;
using GCMS.MathHouse.Localization;

namespace GCMS.MathHouse
{
    public partial class App : Application
    {
        private readonly GameStateService _gameStateService;
        private AppShell _appShell;

        public App(GameStateService gameStateService)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await LocalizationService.InitAsync();
            });
            
            _gameStateService = gameStateService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            _appShell = new AppShell(_gameStateService);
            return new Window(_appShell);
        }

        protected override async void OnStart()
        {
            base.OnStart();
            
            // Handle app startup logic through AppShell
            if (_appShell != null)
            {
                await _appShell.HandleAppStartup();
            }
        }
    }
}