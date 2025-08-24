using System.Diagnostics;
using System.Text.Json;

namespace GCMS.MathHouse
{
    public class LayoutConfig
    {
        public double AnimalSize { get; set; }
        public Dictionary<string, string> AnimalMargins { get; set; } = new();
        public List<double> RowHeights { get; set; } = new();

        public Dictionary<string, double>? FontSizes { get; set; }

        public double HouseImageMaxHeight { get; set; } = 1000;

        // FreeMath specific properties
        public double FreeMathImageMaxHeight { get; set; } = 800;
        public double CharacterImageSize { get; set; } = 80;
        public string CharacterImageMargin { get; set; } = "0,0,0,0";
    }

    public interface IDeviceScreenService
    {
        AndroidScreenCategory GetScreenCategory();
        double GetFontSize(double baseSize);
        double GetImageSize(double baseSize);
    }

    public class DeviceScreenService : IDeviceScreenService
    {
        private readonly AndroidScreenCategory _category;

        public DeviceScreenService()
        {
            var widthDp = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

            _category = widthDp switch
            {
                <= 360 => AndroidScreenCategory.SmallPhone,
                <= 430 => AndroidScreenCategory.MediumPhone,
                <= 600 => AndroidScreenCategory.LargePhone,
                <= 700 => AndroidScreenCategory.SmallTablet,
                <= 800 => AndroidScreenCategory.MediumTablet,
                _ => AndroidScreenCategory.LargeTablet
            };
        }

        public AndroidScreenCategory GetScreenCategory() => _category;

        public double GetFontSize(double baseSize) => baseSize; // poți extinde dacă vrei

        public double GetImageSize(double baseSize) => baseSize; // idem
    }

    public enum AndroidScreenCategory
    {
        SmallPhone,
        MediumPhone,
        LargePhone,
        SmallTablet,
        MediumTablet,
        LargeTablet,
    }

    public class ResponsiveLayoutService
    {
        private Dictionary<AndroidScreenCategory, LayoutConfig> _configs = new();
        private readonly AndroidScreenCategory _category;

        public ResponsiveLayoutService(IDeviceScreenService screenService)
        {
            _category = screenService.GetScreenCategory();
        }

        public async Task InitializeAsync()
        {
            try
            {
                _configs = await LoadLayoutConfigAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load layout config: {ex.Message}");
                // Fallback to default configuration to prevent crashes
                _configs = CreateDefaultLayoutConfig();
            }
        }

        private Dictionary<AndroidScreenCategory, LayoutConfig> CreateDefaultLayoutConfig()
        {
            // Create a basic fallback configuration to prevent crashes
            var defaultConfig = new LayoutConfig
            {
                AnimalSize = 70,
                AnimalMargins = new Dictionary<string, string>
                {
                    { "TopFloor", "70,40,0,0" },
                    { "ThirdFloorLeft", "10,32,0,0" },
                    { "ThirdFloorRight", "0,32,14,0" },
                    { "SecondFloorLeft", "10,20,0,0" },
                    { "SecondFloorRight", "0,20,14,0" },
                    { "FirstFloorLeft", "10,15,0,0" },
                    { "FirstFloorRight", "0,15,16,0" },
                    { "GroundFloor", "5,12,5,0" }
                },
                RowHeights = new List<double> { 1, 1, 1, 1, 1 },
                FontSizes = new Dictionary<string, double>
                {
                    { "GameMessage", 18 },
                    { "Feedback", 16 },
                    { "ScoreSummary", 20 },
                    { "FreeMathNarrative", 16 },
                    { "CharacterName", 14 },
                    { "ContinueButton", 18 }
                },
                HouseImageMaxHeight = 600,
                FreeMathImageMaxHeight = 500,
                CharacterImageSize = 70,
                CharacterImageMargin = "10,5,10,5"
            };

            return new Dictionary<AndroidScreenCategory, LayoutConfig>
            {
                { AndroidScreenCategory.SmallPhone, defaultConfig },
                { AndroidScreenCategory.MediumPhone, defaultConfig },
                { AndroidScreenCategory.LargePhone, defaultConfig },
                { AndroidScreenCategory.SmallTablet, defaultConfig },
                { AndroidScreenCategory.MediumTablet, defaultConfig },
                { AndroidScreenCategory.LargeTablet, defaultConfig }
            };
        }

        private async Task<Dictionary<AndroidScreenCategory, LayoutConfig>> LoadLayoutConfigAsync()
        {
            try
            {
                using var assetStream = await FileSystem.OpenAppPackageFileAsync("responsive_layout.json");

                using var memoryStream = new MemoryStream();
                await assetStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var reader = new StreamReader(memoryStream);
                var json = await reader.ReadToEndAsync();

                var rootDict = JsonSerializer.Deserialize<Dictionary<string, LayoutConfig>>(json)!;

                return rootDict.ToDictionary(
                    kv => Enum.Parse<AndroidScreenCategory>(kv.Key),
                    kv => kv.Value);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Could not load responsive_layout.json: {ex.Message}");
                throw;
            }
        }


        public double GetAnimalSize() => _configs[_category].AnimalSize;

        public Thickness GetAnimalMargin(string classId)
        {
            var raw = _configs[_category].AnimalMargins.TryGetValue(classId, out var marginStr)
                ? marginStr
                : "0,0,0,0";

            var parts = raw.Split(',').Select(p => double.Parse(p.Trim())).ToArray();
            return new Thickness(parts[0], parts[1], parts[2], parts[3]);
        }

        public GridLength GetRowHeight(int index)
        {
            var heights = _configs[_category].RowHeights;
            if (index >= 0 && index < heights.Count)
                return new GridLength(heights[index], GridUnitType.Star);

            return new GridLength(1, GridUnitType.Star);
        }

        public double GetFontSize(string key)
        {
            if (_configs[_category].FontSizes != null &&
                _configs[_category].FontSizes.TryGetValue(key, out var size))
            {
                return size;
            }

            return 16; // fallback default
        }

        public double GetHouseImageMaxHeight()
        {
            return _configs[_category].HouseImageMaxHeight;
        }

        // FreeMath specific methods
        public double GetFreeMathImageMaxHeight()
        {
            return _configs[_category].FreeMathImageMaxHeight;
        }

        public double GetCharacterImageSize()
        {
            return _configs[_category].CharacterImageSize;
        }

        public Thickness GetCharacterImageMargin()
        {
            var raw = _configs[_category].CharacterImageMargin ?? "0,0,0,0";
            var parts = raw.Split(',').Select(p => double.Parse(p.Trim())).ToArray();
            return new Thickness(parts[0], parts[1], parts[2], parts[3]);
        }
    }
}
