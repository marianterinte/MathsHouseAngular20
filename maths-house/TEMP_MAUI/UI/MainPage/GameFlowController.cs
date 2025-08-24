using GCMS.MathHouse.BL;
using GCMS.MathHouse.Localization;
using System.Text.RegularExpressions;

namespace GCMS.MathHouse.UI.MainPage
{
    public class GameFlowController
    {
        private readonly GameStateService _gameStateService;
        private readonly GameLevelService _gameLevelService;

        public event Action<string> OnFloorUIUpdateRequested;
        public event Action OnRefreshAnimalsRequested;

        public GameFlowController(GameStateService gameStateService, GameLevelService gameLevelService)
        {
            _gameStateService = gameStateService;
            _gameLevelService = gameLevelService;
        }

        public async Task<bool> HandleFloorClick(string floorId)
        {
            var floor = _gameStateService.GetFloor(floorId);
            if (floor == null) return false;

            if (floor.Status == FloorStatus.Locked)
            {
                string message = LocalizationService.Translate("main_page_level_locked_title");
                string description = LocalizationService.Translate("main_page_level_locked_message");
                await Application.Current.MainPage.DisplayAlert(message, description, "OK");
                return false;
            }

            _gameLevelService.Initialize(
                floor.OperationType,
                floor.QuestionsCount,
                success => OnFloorCompleted(floorId, success));

            await Shell.Current.GoToAsync("//GameLevelPage");
            return true;
        }

        public async Task CheckAndUnlockNextFloors(string currentPrefix)
        {
            if (!_gameStateService.ShouldUnlockNextFloor(currentPrefix))
                return;

            var (nextLeft, nextRight) = GetNextFloorIds(currentPrefix);

            if (!string.IsNullOrEmpty(nextLeft))
            {
                await _gameStateService.SetStatusAsync(nextLeft, FloorStatus.Unlocked);
            }
            if (!string.IsNullOrEmpty(nextRight))
            {
                await _gameStateService.SetStatusAsync(nextRight, FloorStatus.Unlocked);
            }
        }

        private (string nextLeft, string nextRight) GetNextFloorIds(string currentPrefix)
        {
            return currentPrefix switch
            {
                "GroundFloor" => ("FirstFloorLeft", "FirstFloorRight"),
                "FirstFloor" => ("SecondFloorLeft", "SecondFloorRight"),
                "SecondFloor" => ("ThirdFloorLeft", "ThirdFloorRight"),
                "ThirdFloor" => ("TopFloor", "TopFloor"),
                _ => ("", "")
            };
        }

        private async void OnFloorCompleted(string floorId, bool success)
        {
            await _gameStateService.SetStatusAsync(floorId, FloorStatus.Resolved);
            _gameStateService.HasPendingUiUpdate = true;

            
            if (success)
            {
                await ProcessSuccessfulFloorCompletion(floorId);
            }
            if(floorId == "TopFloor")
            {
                await Shell.Current.GoToAsync("//FreeMathPage");
            }
            else
            {
                await Shell.Current.GoToAsync("//MainPage");
            }

            // Replace Task.Delay with proper main thread coordination
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                OnFloorUIUpdateRequested?.Invoke(floorId);
                OnRefreshAnimalsRequested?.Invoke();
            });
        }

        private async Task ProcessSuccessfulFloorCompletion(string floorId)
        {
            var floor = _gameStateService.GetFloor(floorId);
            if (floor == null) return;

            if (floor.Id == "TopFloor")
            {
                await UpdateEnding();
            }
            else
            {
                await ProcessRegularFloorCompletion(floor, floorId);
            }
        }

        private async Task ProcessRegularFloorCompletion(Floor floor, string floorId)
        {
            var animalName = floor.Animal.Name;
            var msg = string.Format(LocalizationService.Translate("main_page_animal_saved_message"), animalName);
            MessagingCenter.Send<object, string>(this, "FloorCompleted", msg);

            var prefix = Regex.Replace(floorId, "(Left|Right)$", "");
            await CheckAndUnlockNextFloors(prefix);
            await _gameStateService.SaveAsync();
        }

        // Make this public so it can be called from FreeMathPage when wizard quest is completed
        public async Task UpdateEnding()
        {
            string endMessage = LocalizationService.Translate("main_page_end_message");
            MessagingCenter.Send(Application.Current.MainPage, "FloorCompleted", endMessage);
            await _gameStateService.SetStatusAsync("TopFloor", FloorStatus.Resolved);
        }

        // Additional method specifically for completing the entire game from FreeMathPage
        public async Task CompleteGame()
        {
            await UpdateEnding();
        }
    }
}
