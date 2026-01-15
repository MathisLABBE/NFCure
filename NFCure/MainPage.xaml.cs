using Microsoft.Maui;
using System.Text;

namespace NFCure
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            RfidToolsGetter.SetOnTagRead((uid, ndefText) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LabelUID.Text = string.IsNullOrEmpty(uid) ? "(UID vide)" : uid;
                    LabelNDEF.Text = string.IsNullOrEmpty(ndefText) ? "(Pas de texte NDEF)" : ndefText;
                });
                RfidToolsGetter.SetOffTagRead();
            });
        }

        private void SetMessageToSend(string msg)
        {
#if ANDROID
            NFCure.Platforms.Android.NfcHceService.CurrentMessage = msg;
#endif
        }

        private void OnSetMessageClicked(object sender, EventArgs e)
        {
            var rsa = new ProgramRSA();
            var msg = MessageEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(msg))
            {
                SendStatusLabel.Text = "⚠️ Écris un message d'abord";
                return;
            }

            List<long> encryptedmessage = rsa.EncryptString(msg ?? "");
            
            string encryptedmsgstring = string.Join(",", encryptedmessage);

            SetMessageToSend(encryptedmsgstring);
            SendStatusLabel.Text = "✅ Message prêt à être envoyé en NFC : " + msg;
        }
    }
}
