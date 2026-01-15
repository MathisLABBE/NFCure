using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using NFCure.Platforms.Android;

namespace NFCure
{
    public class RfidTools
    {
        private readonly Activity _activity;
        private readonly NfcAdapter _nfcAdapter;

        public NfcDataReceive OnTagRead;

        public RfidTools(Activity activity)
        {
            _activity = activity;
            _nfcAdapter = NfcAdapter.GetDefaultAdapter(_activity);
        }

        public void OnResume()
        {
            if (_nfcAdapter == null)
            {
                var alert = new AlertDialog.Builder(_activity).Create();
                alert.SetTitle("NFC indisponible");
                alert.SetMessage("Ce téléphone ne supporte pas le NFC.");
                alert.Show();
                return;
            }

            var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
            var ndefDetected = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
            var techDetected = new IntentFilter(NfcAdapter.ActionTechDiscovered);

            var filters = new[] { ndefDetected, tagDetected, techDetected };

            var intent = new Intent(_activity, _activity.GetType())
                .AddFlags(ActivityFlags.SingleTop);

            PendingIntentFlags flags =
                PendingIntentFlags.UpdateCurrent
                | (Build.VERSION.SdkInt >= BuildVersionCodes.S ? PendingIntentFlags.Mutable : 0);

            var pendingIntent = PendingIntent.GetActivity(_activity, 0, intent, flags);


            _nfcAdapter.EnableForegroundDispatch(_activity, pendingIntent, filters, null);
        }

        public void OnNewIntent(Intent intent)
        {
            // Selon les tags, tu peux recevoir ActionNdefDiscovered / TagDiscovered / TechDiscovered
            var uid = RfidSimple.GetUid(intent);
            var text = RfidSimple.GetNdefText(intent);

            OnTagRead?.Invoke(uid, text);
        }
    }
}
