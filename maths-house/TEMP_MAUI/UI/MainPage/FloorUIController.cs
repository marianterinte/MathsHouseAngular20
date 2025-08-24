using GCMS.MathHouse.BL;

namespace GCMS.MathHouse.UI.MainPage
{
    public class FloorUIController
    {
        private readonly GameStateService _gameStateService;
        private readonly Dictionary<string, (ImageButton Button, Image Animal)> _floorControls;
        private readonly HashSet<VisualElement> _animatedElements = [];

        public FloorUIController(GameStateService gameStateService)
        {
            _gameStateService = gameStateService;
            _floorControls = new Dictionary<string, (ImageButton, Image)>();
        }

        public void InitializeFloorControls(
            ImageButton groundFloorButton, Image groundFloorAnimal,
            ImageButton firstFloorLeftButton, Image firstFloorLeftAnimal,
            ImageButton firstFloorRightButton, Image firstFloorRightAnimal,
            ImageButton secondFloorLeftButton, Image secondFloorLeftAnimal,
            ImageButton secondFloorRightButton, Image secondFloorRightAnimal,
            ImageButton thirdFloorLeftButton, Image thirdFloorLeftAnimal,
            ImageButton thirdFloorRightButton, Image thirdFloorRightAnimal,
            ImageButton topFloorButton, Image topFloorAnimal)
        {
            _floorControls.Clear();
            _floorControls.Add("GroundFloor", (groundFloorButton, groundFloorAnimal));
            _floorControls.Add("FirstFloorLeft", (firstFloorLeftButton, firstFloorLeftAnimal));
            _floorControls.Add("FirstFloorRight", (firstFloorRightButton, firstFloorRightAnimal));
            _floorControls.Add("SecondFloorLeft", (secondFloorLeftButton, secondFloorLeftAnimal));
            _floorControls.Add("SecondFloorRight", (secondFloorRightButton, secondFloorRightAnimal));
            _floorControls.Add("ThirdFloorLeft", (thirdFloorLeftButton, thirdFloorLeftAnimal));
            _floorControls.Add("ThirdFloorRight", (thirdFloorRightButton, thirdFloorRightAnimal));
            _floorControls.Add("TopFloor", (topFloorButton, topFloorAnimal));
        }

        public void UpdateFloorUI(string floorId)
        {
            try
            {
                if (!_floorControls.TryGetValue(floorId, out var controls))
                {
                    return;
                }

                var floor = _gameStateService.GetFloor(floorId);
                if (floor == null)
                {
                    return;
                }

                var (button, animalImage) = controls;
                var showFloor = ShouldShowFloor(floorId);

                button.IsVisible = showFloor;

                if (!showFloor)
                {
                    if (animalImage != null)
                    {
                        animalImage.IsVisible = false;
                    }
                    return;
                }

                switch (floor.Status)
                {
                    case FloorStatus.Resolved:
                        HandleResolvedFloor(button, animalImage, floor);
                        break;
                    case FloorStatus.Unlocked:
                        HandleUnlockedFloor(button, animalImage);
                        break;
                    case FloorStatus.Locked:
                        HandleLockedFloor(button, animalImage);
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void HandleResolvedFloor(ImageButton button, Image animalImage, Floor floor)
        {
            button.IsVisible = false;
            if (animalImage != null)
            {
                animalImage.Source = floor.Animal.Image;
                animalImage.IsVisible = true;
            }
        }

        private void HandleUnlockedFloor(ImageButton button, Image animalImage)
        {
            button.Source = "access_granted.png";
            button.IsVisible = true;
            if (animalImage != null)
            {
                animalImage.IsVisible = false;
            }
            AnimateLockFade(button);
        }

        private void HandleLockedFloor(ImageButton button, Image animalImage)
        {
            button.Source = "lock.png";
            button.Opacity = 0.9;
            button.IsVisible = true;
            if (animalImage != null)
            {
                animalImage.IsVisible = false;
            }
        }

        private bool ShouldShowFloor(string floorId)
        {
            var floor = _gameStateService.GetFloor(floorId);
            if (floor == null) return false;

            if (floor.Status != FloorStatus.Locked)
                return true;

            if (floorId.StartsWith("FirstFloor"))
                return true;

            var floorOrder = new List<string>
            {
                "GroundFloor",
                "FirstFloorLeft",
                "FirstFloorRight",
                "SecondFloorLeft",
                "SecondFloorRight",
                "ThirdFloorLeft",
                "ThirdFloorRight",
                "TopFloor"
            };

            var index = floorOrder.IndexOf(floorId);
            if (index <= 0) return true;

            var prevKey = floorOrder[index - 1];
            return _gameStateService.GetStatus(prevKey) == FloorStatus.Resolved;
        }

        public void AnimateLockFade(VisualElement element)
        {
            if (_animatedElements.Contains(element))
                return;

            _animatedElements.Add(element);

            var animation = new Animation
            {
                { 0, 0.5, new Animation(v => element.Opacity = v, 0.9, 0) },
                { 0.5, 1, new Animation(v => element.Opacity = v, 0, 0.9) }
            };

            animation.Commit(element, "FadeLoop", length: 1600, repeat: () => true);
        }

        public async Task RefreshResolvedAnimals()
        {
            var resolvedFloors = _gameStateService.Floors.Values
                .Where(f => f.Status == FloorStatus.Resolved)
                .ToList();

            foreach (var floor in resolvedFloors)
            {
                if (_floorControls.TryGetValue(floor.Id, out var controls))
                {
                    var (button, animalImage) = controls;

                    if (animalImage != null)
                    {
                        button.IsVisible = false;
                        animalImage.Source = floor.Animal.Image;
                        animalImage.IsVisible = true;

                        await Task.Delay(10);
                        animalImage.InvalidateMeasure();
                    }
                }
            }
        }

        public void UpdateAllFloors()
        {
            foreach (var floorId in _gameStateService.Floors.Keys)
            {
                UpdateFloorUI(floorId);
            }
        }

        public void AnimateUnlockedFloors()
        {
            var unlockedFloors = _gameStateService.Floors.Values
                .Where(f => f.Status == FloorStatus.Unlocked)
                .ToList();

            foreach (var floor in unlockedFloors)
            {
                if (floor.Id != "TopFloor")
                {
                    if (_floorControls.TryGetValue(floor.Id, out var controls))
                    {
                        AnimateLockFade(controls.Button);
                    }
                }
            }
        }
    }
}

