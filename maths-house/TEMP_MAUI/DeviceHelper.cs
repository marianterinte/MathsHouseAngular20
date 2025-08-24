using Microsoft.Maui.Devices;

namespace GCMS.MathHouse.Utils
{
    public static class DeviceHelper
    {
        public static bool IsTablet()
        {
            var display = DeviceDisplay.MainDisplayInfo;
            var width = display.Width / display.Density;
            return width >= 600;
        }

        public static bool IsPhone()
        {
            return !IsTablet();
        }

        public static double GetWidthInDp()
        {
            var display = DeviceDisplay.MainDisplayInfo;
            return display.Width / display.Density;
        }

        public static double GetHeightInDp()
        {
            var display = DeviceDisplay.MainDisplayInfo;
            return display.Height / display.Density;
        }

        public static bool IsLandscape()
        {
            var display = DeviceDisplay.MainDisplayInfo;
            return display.Width > display.Height;
        }

        public static bool IsPortrait()
        {
            return !IsLandscape();
        }
    }
}