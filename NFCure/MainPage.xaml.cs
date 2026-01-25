namespace NFCure
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        string NdefText = "";
        public ProgramRSA _myRsa;



        public MainPage()
        {
            InitializeComponent();

            _myRsa = new ProgramRSA();
        }

        /*
        private void ProcedureEnvoyeur(object? sender, EventArgs args)
        {
            // Récupérer la clé publique depuis le TextField
            var publicKeyText = PublicKeyEntry.Text?.Trim();
            
            if (string.IsNullOrWhiteSpace(publicKeyText))
            {
                SendStatusLabel.Text = "⚠️ Clé publique manquante";
                return;
            }

            var parts = publicKeyText.Split(',');
            if (parts.Length != 2)
            {
                SendStatusLabel.Text = "⚠️ Clé publique invalide (format: n,e)";
                return;
            }

            if (!long.TryParse(parts[0].Trim(), out long n) || !long.TryParse(parts[1].Trim(), out long e))
            {
                SendStatusLabel.Text = "⚠️ Clé publique invalide (valeurs non numériques)";
                return;
            }

            var msg = MessageEntry.Text?.Trim();
            if (string.IsNullOrWhiteSpace(msg))
            {
                SendStatusLabel.Text = "⚠️ Écris un message d'abord";
                return;
            }

            // Chiffrer le message avec la clé publique du destinataire
            List<long> encryptedMessage = _myRsa.EncryptString(msg, new long[] { n, e });
            string encryptedMsgString = string.Join(",", encryptedMessage);

            // Préparer l'envoi via NFC
            SetMessageToSend(encryptedMsgString);

            SendStatusLabel.Text = "✅ Message chiffré prêt à être envoyé en NFC";
        }
        */



        private void ProcedureRecepteur(object? sender, EventArgs args)
        {
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
                    long[] clePrivee = new long[] { _myRsa.n, _myRsa.d };
                    string decryptedMessage = _myRsa.DecryptString(ndefText, clePrivee);
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
            var msg = MessageEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(msg))
            {
                SendStatusLabel.Text = "⚠️ Écris un message d'abord";
                return;
            }

            long[] clePublique = new long[] { _myRsa.n, _myRsa.e };
            List<long> encryptedmessage = _myRsa.EncryptString(msg ?? "", clePublique);
            
            string encryptedmsgstring = string.Join(",", encryptedmessage);

            SetMessageToSend(encryptedmsgstring);
            SendStatusLabel.Text = "✅ Message prêt à être envoyé en NFC : " + msg;
        }

        private async void BtnPageEnvoyer_Click(object sender, EventArgs e)
        {
            //await Shell.Current.GoToAsync(nameof(EnvoyerPage));
            await Navigation.PushAsync(new EnvoyerPage());
        }

        private async void BtnPageRecevoir_Click(object sender, EventArgs e)
        {
            //await Shell.Current.GoToAsync(nameof(RecevoirPage));
            await Navigation.PushAsync(new RecevoirPage(_myRsa));
        }
    }
}
