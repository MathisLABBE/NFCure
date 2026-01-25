using Microsoft.Maui;
using System.Text;

namespace NFCure
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        string NdefText = "";



        public MainPage()
        {
            InitializeComponent();

            
        }


        private void ProcedureEnvoyeur(object? sender, EventArgs args)
        {
            var rsa = new ProgramRSA();

            RfidToolsGetter.SetOnTagRead((uid, ndefText) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LabelUID.Text = string.IsNullOrEmpty(uid) ? "(UID vide)" : uid;
                    LabelNDEF.Text = string.IsNullOrEmpty(ndefText) ? "(Pas de texte NDEF)" : ndefText;
                    NdefText = ndefText;
                });

                RfidToolsGetter.SetOffTagRead();
            });

            if (string.IsNullOrWhiteSpace(NdefText))
            {
                SendStatusLabel.Text = "⚠️ Clé publique manquante";
                return;
            }

            var parts = NdefText.Split(',');
            if (parts.Length != 2)
            {
                SendStatusLabel.Text = "⚠️ Clé publique invalide";
                return;
            }

            long n = long.Parse(parts[0]);
            long e = long.Parse(parts[1]);

            var msg = MessageEntry.Text?.Trim();
            if (string.IsNullOrWhiteSpace(msg))
            {
                SendStatusLabel.Text = "⚠️ Écris un message d'abord";
                return;
            }

            List<long> encryptedMessage = rsa.EncryptString(msg, new long[] { n, e });
            string encryptedMsgString = string.Join(",", encryptedMessage);

            SetMessageToSend(encryptedMsgString);

            SendStatusLabel.Text = "✅ Message chiffré prêt à être envoyé en NFC";
        }



        private void ProcedureRecepteur(object? sender, EventArgs args)
        {
            var rsa = new ProgramRSA();

            // Envoyer la clé publique
            var clepubliqueaenvoyer = rsa.n + "," + rsa.e;
            SetMessageToSend(clepubliqueaenvoyer);

            // Configurer la lecture NFC et déchiffrer le message reçu
            RfidToolsGetter.SetOnTagRead((uid, ndefText) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LabelUID.Text = string.IsNullOrEmpty(uid) ? "(UID vide)" : uid;

                    if (string.IsNullOrWhiteSpace(ndefText))
                    {
                        LabelNDEF.Text = "(Pas de texte NDEF)";
                        return;
                    }

                    // Déchiffrer le message reçu
                    long[] clePrivee = new long[] { rsa.n, rsa.d };
                    string decryptedMessage = rsa.DecryptString(ndefText, clePrivee);
                    LabelNDEF.Text = decryptedMessage;
                    SendStatusLabel.Text = "✅ Message reçu et déchiffré";
                });

                RfidToolsGetter.SetOffTagRead();
            });
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

            long[] clePublique = new long[] { rsa.n, rsa.e };
            List<long> encryptedmessage = rsa.EncryptString(msg ?? "", clePublique);
            
            string encryptedmsgstring = string.Join(",", encryptedmessage);

            SetMessageToSend(encryptedmsgstring);
            SendStatusLabel.Text = "✅ Message prêt à être envoyé en NFC : " + msg;
        }

        private async void BtnPageEnvoyer_Click(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(EnvoyerPage));
        }

        private async void BtnPageRecevoir_Click(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RecevoirPage));
        }
    }
}
