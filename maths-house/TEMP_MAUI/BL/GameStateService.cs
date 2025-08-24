using System.Text.Json;
using GCMS.MathHouse.Localization;

namespace GCMS.MathHouse.BL;

public class GameStateService
{
    private const string Key = "GameProgress";
    private GameProgress _state;
    public bool HasPendingUiUpdate { get; set; } = false;

    private Dictionary<string, Floor> _floors;
    public IReadOnlyDictionary<string, Floor> Floors => _floors;

    // Ingredient tracking for Wizard Rescue quest
    private readonly List<string> _ingredientIds = new()
    {
        "magic_wool", "magic_carrot", "magic_bone", "magic_feather", "magic_eucalyptus", "magic_leaf", "magic_bamboo"
    };
    private Dictionary<string, bool> _isIngredientCollected = new();
    private Dictionary<string, int> _collectedMagicNumbers = new();

    public IEnumerable<string> GetIngredientIds()
    {
        return _ingredientIds;
    }

    public bool IsIngredientCollected(string ingredientId)
    {
        return _isIngredientCollected.TryGetValue(ingredientId, out var collected) && collected;
    }

    public void MarkIngredientCollected(string ingredientId, int magicNumber)
    {
        _isIngredientCollected[ingredientId] = true;
        _collectedMagicNumbers[ingredientId] = magicNumber;
    }

    public int? GetCollectedMagicNumber(string ingredientId)
    {
        return _collectedMagicNumbers.TryGetValue(ingredientId, out var number) ? number : null;
    }

    // Method to check if user should be redirected to FreeMathPage on startup
    public bool ShouldRedirectToFreeMathPage()
    {
        // Check if TopFloor is resolved (meaning user completed part 1)
        var topFloorStatus = GetStatus("TopFloor");
        bool topfloorIsResolved = topFloorStatus == FloorStatus.Resolved;
        var freeMathProgress = PreferencesManager.GetFreeMathProgress();
        var freeMathIngredients = PreferencesManager.GetFreeMathCollectedIngredients();
        var ingredientsCount = PreferencesManager.GetCollectedIngredients().Count;
        bool hasReachedPart2 = PreferencesManager.GetHasReachedPart2();

        if (hasReachedPart2 && !topfloorIsResolved)
            return true;
        else
        if (hasReachedPart2 && topfloorIsResolved && ingredientsCount > 7)
            return false;
        else
        if (hasReachedPart2 && topfloorIsResolved && ingredientsCount < 7)
            return true;

        return false;
    }

    // Method for smart navigation to home - goes to correct page based on progress
    public async Task NavigateToHomePage()
    {
        await LoadAsync(); // Ensure latest state is loaded

        if (ShouldRedirectToFreeMathPage())
        {
            await Shell.Current.GoToAsync("//FreeMathPage");
        }
        else
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }

    public void InitializeGameStateService()
    {
        _floors = InitializeFloors();
    }
    private Dictionary<string, Floor> InitializeFloors()
    {
        return new Dictionary<string, Floor>
{
    {
        "GroundFloor", new Floor
        {
            Id = "GroundFloor",
            Name = "Parter",
            Status = FloorStatus.Unlocked,
            Animal = new QuestAnimal { Name = AnimalName("animal_names_bunny"), Image = "bunny.png", Video = "bunny.mp4" },
            OperationType = MathOperationType.AddTwoNumbersLessThan10,
            QuestionsCount = 7,
            NextFloorIds = new List<string> { "FirstFloorLeft", "FirstFloorRight" },
            Position = new Point(135, 500)
        }
    },
    {
        "FirstFloorLeft", new Floor
        {
            Id = "FirstFloorLeft",
            Name = "Etaj 1 Stânga",
            Status = FloorStatus.Locked,
            Animal = new QuestAnimal { Name = AnimalName("animal_names_cat"), Image = "cat.png", Video = "cat.mp4" },
            OperationType = MathOperationType.AddThreeNumbersLessThan10,
            QuestionsCount = 7,
            NextFloorIds = new List<string> { "SecondFloorLeft" },
            Position = new Point(87, 400)
        }
    },
    {
        "FirstFloorRight", new Floor
        {
            Id = "FirstFloorRight",
            Name = "Etaj 1 Dreapta",
            Status = FloorStatus.Locked,
          Animal = new QuestAnimal { Name = AnimalName("animal_names_dog"), Image = "dog.png", Video = "dog.mp4" },
            OperationType = MathOperationType.AddTwoNumbersLessThan20,
             QuestionsCount = 7,
            NextFloorIds = new List<string> { "SecondFloorRight" },
            Position = new Point(207, 400)
        }
    },
    {
        "SecondFloorLeft", new Floor
        {
            Id = "SecondFloorLeft",
            Name = "Etaj 2 Stânga",
            Status = FloorStatus.Locked,
           Animal = new QuestAnimal { Name = AnimalName("animal_names_parrot"), Image = "parrot.png", Video = "fox.mp4" },
            OperationType = MathOperationType.SubtractThreeNumbersLessThan10,
             QuestionsCount = 7,
            NextFloorIds = new List<string> { "ThirdFloorLeft" },
            Position = new Point(87, 300)
        }
    },
    {
        "SecondFloorRight", new Floor
        {
            Id = "SecondFloorRight",
            Name = "Etaj 2 Dreapta",
            Status = FloorStatus.Locked,
           Animal = new QuestAnimal { Name = AnimalName("animal_names_koala"), Image = "koala.png", Video = "koala.mp4" },
            OperationType = MathOperationType.SubtractTwoNumbersLessThan20,
             QuestionsCount = 7,
            NextFloorIds = new List<string> { "ThirdFloorRight" },
            Position = new Point(207, 300)
        }
    },
    {
        "ThirdFloorLeft", new Floor
        {
            Id = "ThirdFloorLeft",
            Name = "Etaj 3 Stânga",
            Status = FloorStatus.Locked,
            Animal = new QuestAnimal { Name = AnimalName("animal_names_owl"), Image = "owl.png", Video = "owl.mp4" },
            OperationType = MathOperationType.AddAndSubstract3NumbersLessThan10,
             QuestionsCount = 7,
            NextFloorIds = new List<string>() { "TopFloor" },
            Position = new Point(87, 200)
        }
    },
    {
        "ThirdFloorRight", new Floor
        {
            Id = "ThirdFloorRight",
            Name = "Etaj 3 Dreapta",
            Status = FloorStatus.Locked,
           Animal = new QuestAnimal { Name = AnimalName("animal_names_panda"), Image = "panda.png", Video = "panda.mp4" },
            OperationType = MathOperationType.AddAndSubstract2NumbersLessThan20,
             QuestionsCount = 7,
            NextFloorIds = new List<string>() { "TopFloor" },
            Position = new Point(207, 200)
        }
    },
            {
        "TopFloor", new Floor
        {
            Id = "TopFloor",
            Name = "Ultimul Etaj",
            Status = FloorStatus.Locked,
            Animal = new QuestAnimal { Name = AnimalName("animal_names_elephant"), Image = "elephant.png", Video = "elephant.mp4" },
            OperationType = MathOperationType.AddAndSubstract2NumbersLessThan20,
             QuestionsCount = 7,
            NextFloorIds = new List<string>(),
            Position = new Point(207, 200)
        }
    }
};
    }

    private static string AnimalName(string key) => LocalizationService.Translate(key);

    public async Task LoadAsync()
    {
        if (PreferencesManager.HasGameProgress())
        {
            var json = PreferencesManager.GetGameProgress();
            _state = JsonSerializer.Deserialize<GameProgress>(json) ?? new GameProgress();

            // Actualizează starea etajelor din datele salvate
            foreach (var (floorId, status) in _state.FloorStates)
            {
                if (_floors.ContainsKey(floorId))
                {
                    _floors[floorId].Status = status;
                }
            }
        }
        else
        {
            _state = new GameProgress();
            await SaveAsync();
        }
    }

    public Floor GetFloor(string floorId)
    {
        return _floors.TryGetValue(floorId, out var floor) ? floor : null;
    }

    public FloorStatus GetStatus(string floorId)
    {
        return _floors.TryGetValue(floorId, out var floor) ? floor.Status : FloorStatus.Locked;
    }

    public async Task SetStatusAsync(string floorId, FloorStatus status)
    {
        if (_floors.TryGetValue(floorId, out var floor))
        {
            floor.Status = status;
            _state.FloorStates[floorId] = status;
            await SaveAsync();
        }
    }

    public string GetAnimalImage(string floorId)
    {
        return _floors.TryGetValue(floorId, out var floor) ? floor.Animal.Image : "animal_default.png";
    }

    public async Task SaveAsync()
    {
        // Sincronizează _state cu _floors
        foreach (var floor in _floors.Values)
        {
            _state.FloorStates[floor.Id] = floor.Status;
        }

        var json = JsonSerializer.Serialize(_state);
        PreferencesManager.SetGameProgress(json);
    }

    public async Task UnlockNextFloorsAsync(string floorId)
    {
        if (_floors.TryGetValue(floorId, out var floor))
        {
            foreach (var nextFloorId in floor.NextFloorIds)
            {
                if (_floors.TryGetValue(nextFloorId, out var nextFloor))
                {
                    nextFloor.Status = FloorStatus.Unlocked;
                    _state.FloorStates[nextFloorId] = FloorStatus.Unlocked;
                }
            }
            await SaveAsync();
        }
    }

    public bool ShouldUnlockNextFloor(string currentPrefix)
    {
        if (currentPrefix == "GroundFloor")
        {
            return true;
        }

        var left = currentPrefix + "Left";
        var right = currentPrefix + "Right";

        return GetStatus(left) == FloorStatus.Resolved && GetStatus(right) == FloorStatus.Resolved;
    }

    public bool IsNewGame()
    {
        var status = GetStatus("GroundFloor");
        return status != FloorStatus.Resolved;
    }

    public async Task ResetAsync()
    {
        // Resetează toate etajele la starea inițială
        foreach (var floor in _floors.Values)
        {
            if (floor.Id == "GroundFloor")
                floor.Status = FloorStatus.Unlocked;
            else
                floor.Status = FloorStatus.Locked;
        }

        // Reconstruiește starea
        _state = new GameProgress();

        // Reset ingredient tracking
        _isIngredientCollected.Clear();
        _collectedMagicNumbers.Clear();

        // Salvează starea nouă
        await SaveAsync();

        // Marchează că UI-ul trebuie actualizat
        HasPendingUiUpdate = true;
    }

    public bool GameIsResolved()
    {
        _floors.TryGetValue("TopFloor", out Floor topFloor);
        return topFloor.Status == FloorStatus.Resolved;
    }
}