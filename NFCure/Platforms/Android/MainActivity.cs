using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using NFCure.Platforms.Android;

namespace NFCure
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize
            | ConfigChanges.Orientation
            | ConfigChanges.UiMode
            | ConfigChanges.ScreenLayout
            | ConfigChanges.SmallestScreenSize
            | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public RfidTools? RfidTools;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RfidTools = new RfidTools(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            RfidTools?.OnResume();
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            if (intent != null)
                RfidTools?.OnNewIntent(intent);
        }
    }
}
