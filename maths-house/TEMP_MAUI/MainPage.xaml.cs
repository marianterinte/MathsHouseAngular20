using GCMS.MathHouse.BL;
using GCMS.MathHouse.Localization;
using GCMS.MathHouse.UI.Common;
using GCMS.MathHouse.UI.MainPage;

namespace GCMS.MathHouse;

public partial class MainPage : ContentPage
{
    private readonly GameStateService _gameStateService;
    private readonly FloorUIController _floorUIController;
    private readonly LayoutManager _layoutManager;
    private readonly GameFlowController _gameFlowController;
    private readonly PopupManager _popupManager;
    private readonly TypewriterAnimationService _typewriterService;
    private readonly LayoutCoordinationService _layoutCoordinationService;
    private readonly AudioManager _audioManager;

    private bool _popupAlreadyShown = false;
    private bool _isOnFloorMessage = false;

    private string _endMessage;
    private string _startMessage;

    public MainPage(GameStateService gameStateService, GameLevelService gameLevelService,
                   ResponsiveLayoutService layoutService, GameFlowController gameFlowController)
    {
        _gameStateService = gameStateService;
        _floorUIController = new FloorUIController(gameStateService);
        _layoutManager = new LayoutManager(layoutService);
        _gameFlowController = gameFlowController;
        _popupManager = new PopupManager();
        _typewriterService = new TypewriterAnimationService();
        _layoutCoordinationService = new LayoutCoordinationService();
        _audioManager = new AudioManager();

        InitializeComponent();
        InitializeFloorControls();
        SetupMessageSubscriptions();
    }

    private async Task InitializeAsync()
    {
        await LocalizationService.InitAsync();
        await _layoutManager.InitializeAsync();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            AdaptLayout();
        });
    }

    private void InitializeFloorControls()
    {
        _floorUIController.InitializeFloorControls(
            GroundFloorButton, GroundFloorAnimal,
            FirstFloorLeftButton, FirstFloorLeftAnimal,
            FirstFloorRightButton, FirstFloorRightAnimal,
            SecondFloorLeftButton, SecondFloorLeftAnimal,
            SecondFloorRightButton, SecondFloorRightAnimal,
            ThirdFloorLeftButton, ThirdFloorLeftAnimal,
            ThirdFloorRightButton, ThirdFloorRightAnimal,
            TopFloorButton, TopFloorAnimal);
    }

    private void SetupMessageSubscriptions()
    {
        MessagingCenter.Subscribe<object, string>(this, "FloorCompleted", async (sender, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // Replace Task.Delay with proper layout coordination
                GameMessageLabel.Text = msg;
                GameMessageLabel.InvalidateMeasure();
                await _typewriterService.StartTypewriterAnimation(GameMessageLabel, msg, 50);
                _isOnFloorMessage = true;
            });
        });

        MessagingCenter.Subscribe<AppShell>(this, "GameReset", async (sender) =>
        {
            await HandleGameReset();
        });

        _gameFlowController.OnFloorUIUpdateRequested += (floorId) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(500); // Ensure UI is ready
                _floorUIController.UpdateFloorUI(floorId);
            });
        };

        _gameFlowController.OnRefreshAnimalsRequested += async () =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await _floorUIController.RefreshResolvedAnimals();
            });
        };
    }

    private async Task HandleGameReset()
    {
        PufPufHelperImage.Source = "puf_at_door.png";
        _floorUIController.UpdateAllFloors();

        // Always play animation again on reset
        await _layoutCoordinationService.EnsureLabelLayoutCalculated(GameMessageLabel, _startMessage);
        await _typewriterService.StartTypewriterAnimation(GameMessageLabel, _startMessage, 50);

        HouseImage.Source = "math_house.png";
        _popupAlreadyShown = false;
        _isOnFloorMessage = false;
    }

    private void AdaptLayout()
    {
        var animalImages = new[]
        {
            TopFloorAnimal, ThirdFloorLeftAnimal, ThirdFloorRightAnimal,
            SecondFloorLeftAnimal, SecondFloorRightAnimal,
            FirstFloorLeftAnimal, FirstFloorRightAnimal,
            GroundFloorAnimal
        };

        var animalButtons = new[]
        {
            TopFloorButton, ThirdFloorLeftButton, ThirdFloorRightButton,
            SecondFloorLeftButton, SecondFloorRightButton,
            FirstFloorLeftButton, FirstFloorRightButton,
            GroundFloorButton
        };

        _layoutManager.AdaptLayout(animalImages, animalButtons, TopFloorButton, HouseGrid, GameMessageLabel, HouseImage);
    }

    protected override async void OnAppearing()
    {
        try
        {
            base.OnAppearing();

            // Start playing MainPage music
            await _audioManager.PlayMainPageMusic(audioElement);

            await InitializeAsync();
            await InitializeTranslations();

            _floorUIController.UpdateAllFloors();

            if (_gameStateService.HasPendingUiUpdate)
            {
                _gameStateService.HasPendingUiUpdate = false;
                _floorUIController.AnimateUnlockedFloors();
            }

            await CheckEndingGameUI();
            await _floorUIController.RefreshResolvedAnimals();

            await SplashImage.FadeTo(0, 400);
            SplashImage.IsVisible = false;
        }
        catch (Exception ex)
        {
            // Log error silently
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Stop any ongoing typing animation
        _typewriterService?.StopAnimation();
        // Pause audio when leaving MainPage
        StopMusicSafely();
    }

    private void StopMusicSafely()
    {
        try
        {
            _ = _audioManager.StopAudio(audioElement);
        }
        catch (Exception ex)
        {
            // Silent fail - audio stopping error
        }
    }

    private async Task InitializeTranslations()
    {
        if (!_isOnFloorMessage)
        {
            await LocalizationService.InitAsync();
            _endMessage = LocalizationService.Translate("main_page_end_message");
            _startMessage = LocalizationService.Translate("main_page_start_message");
        }
    }

    private async Task CheckEndingGameUI()
    {
        if (_gameStateService.GameIsResolved())
        {
            HouseImage.Source = "math_house_unlockedtop.png";

            // Use LayoutCoordinationService instead of Task.Delay
            await _layoutCoordinationService.EnsureLabelLayoutCalculated(GameMessageLabel, _endMessage);
            await _typewriterService.StartTypewriterAnimation(GameMessageLabel, _endMessage, 50);

            PufPufHelperImage.Source = "puf_succeded.png";
            if (!_popupAlreadyShown)
            {
                await _popupManager.ShowFinalPopup();
                _popupAlreadyShown = true;
            }
        }
        else if (!_isOnFloorMessage)
        {
            HouseImage.Source = "math_house.png";

            // Use LayoutCoordinationService instead of Task.Delay
            await _layoutCoordinationService.EnsureLabelLayoutCalculated(GameMessageLabel, _startMessage);
            await _typewriterService.StartTypewriterAnimation(GameMessageLabel, _startMessage, 50);

            PufPufHelperImage.Source = "puf_wondering.png";
        }
        else
        {
            HouseImage.Source = "math_house.png";
            PufPufHelperImage.Source = "puf_succeded.png";
        }
    }

    private async Task HandleFloorClick(string floorId)
    {
        _isOnFloorMessage = false;
        var success = await _gameFlowController.HandleFloorClick(floorId);
    }

    private async void OnAnimalTapped(object sender, EventArgs e)
    {
        if (sender is Image image && !string.IsNullOrWhiteSpace(image.ClassId))
        {
            var videoManager = new VideoManager();
            await videoManager.PlayAnimalVideo(image.ClassId);
        }
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AboutPage");
    }

    private async void OnWizardRescueClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//FreeMathPage");
    }

    private async void OnGroundFloorClicked(object sender, EventArgs e) => await HandleFloorClick("GroundFloor");
    private async void OnFirstFloorLeftClicked(object sender, EventArgs e) => await HandleFloorClick("FirstFloorLeft");
    private async void OnFirstFloorRightClicked(object sender, EventArgs e) => await HandleFloorClick("FirstFloorRight");
    private async void OnSecondFloorLeftClicked(object sender, EventArgs e) => await HandleFloorClick("SecondFloorLeft");
    private async void OnSecondFloorRightClicked(object sender, EventArgs e) => await HandleFloorClick("SecondFloorRight");
    private async void OnThirdFloorLeftClicked(object sender, EventArgs e) => await HandleFloorClick("ThirdFloorLeft");
    private async void OnThirdFloorRightClicked(object sender, EventArgs e) => await HandleFloorClick("ThirdFloorRight");
    private async void OnTopFloorClicked(object sender, EventArgs e) => await HandleFloorClick("TopFloor");
}