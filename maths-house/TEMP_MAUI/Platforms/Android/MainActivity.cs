using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Service.Autofill;
using Java.Net;
using Java.Util;
using Kotlin.IO.Encoding;
using Microsoft.Maui.Controls.PlatformConfiguration;
using static Android.InputMethodServices.Keyboard;
using static Android.Provider.Contacts.Intents;
using static Android.Provider.Telephony.Mms;
using System.Diagnostics;
using System.Reflection.Metadata;
using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;

namespace GCMS.MathHouse
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {

    }
}
