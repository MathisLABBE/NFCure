namespace NFCure;

public partial class EnvoyerPage : ContentPage
{
	private string? CleN = null;
	private string? CleE = null;

	private string? Msg = null;

    public EnvoyerPage()
	{
		InitializeComponent();
	}

    private void BtnEnvoyer_Click(object sender, EventArgs e)
    {
		CleN = ClePubliqueN.Text;
		CleE = ClePubliqueE.Text;

		Msg = Message.Text;

		if (string.IsNullOrWhiteSpace(CleN))
		{
			BtnEnvoyer.Text = "Veuillez entrer une clé N valide";
			return;
        }
		if (string.IsNullOrWhiteSpace(CleE))
		{
			BtnEnvoyer.Text = "Veuillez entrer une clé E valide";
			return;
		}
        if (!long.TryParse(CleN, out long CleNLong))
        {
            BtnEnvoyer.Text = "Clé N invalide (nombre attendu)";
            return;
        }

        if (!long.TryParse(CleE, out long CleELong))
        {
            BtnEnvoyer.Text = "Clé E invalide (nombre attendu)";
            return;
        }
        if (string.IsNullOrWhiteSpace(Msg))
		{
			BtnEnvoyer.Text = "Veuillez entrer un message non vide";
			return;
        }
		ClePubliqueN.IsEnabled = false;
		ClePubliqueE.IsEnabled = false;
		Message.IsEnabled = false;
		BtnEnvoyer.IsEnabled = false;
		BtnEnvoyer.Text = "Le message est prêt à être envoyer";

		// Procedure d'envoie
		ProcedureEnvoyeur(CleNLong, CleELong);

        ClePubliqueN.IsEnabled = true;
		ClePubliqueE.IsEnabled = true;
		Message.IsEnabled = true;
        BtnEnvoyer.IsEnabled = true;
    }

    private void ProcedureEnvoyeur(long n, long e)
    {
		ProgramRSA Rsa = new ProgramRSA();
        // Chiffrer le message avec la clé publique du destinataire
        List<long> encryptedMessage = Rsa.EncryptString(Msg, new long[] { n, e });
        string encryptedMsgString = string.Join(",", encryptedMessage);

        // Préparer l'envoi via NFC
        SetMessageToSend(encryptedMsgString);
    }

    private void SetMessageToSend(string msg)
    {
		#if ANDROID
        NFCure.Platforms.Android.NfcHceService.CurrentMessage = msg;
		#endif
    }
}