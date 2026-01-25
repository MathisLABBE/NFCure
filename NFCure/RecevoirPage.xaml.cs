using Cs;

namespace NFCure;

public partial class RecevoirPage : ContentPage
{
    public ProgramRSA RSA;
	public string? N { get; set; }
    public string? E { get; set; }

    public long D { get; set; }

    public RecevoirPage(ProgramRSA pRSA)
	{
        RSA = pRSA;
        N = pRSA.n.ToString() ?? null;
        E = pRSA.e.ToString() ?? null;
        D = pRSA.d;

        InitializeComponent();

		BindingContext = this;
	}

    private void BtnRecevoir_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(N))
        {
            BtnRecevoir.Text = "Problème avec la clé N, redémarrer l'application";
            return;
        }
        if (string.IsNullOrWhiteSpace(E))
        {
            BtnRecevoir.Text = "Problème avec la clé E, redémarrer l'application";
            return;
        }
        BtnRecevoir.IsEnabled = false;
        BtnRecevoir.Text = "Le message est prêt à être reçus";

        // Procedure receveur
        ProcedureRecepteur();

        BtnRecevoir.IsEnabled = true;

        MessageRecu.IsVisible = true;
    }

    private void ProcedureRecepteur()
    {
        // Configurer la lecture NFC et déchiffrer le message reçu
        RfidToolsGetter.SetOnTagRead((uid, ndefText) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (string.IsNullOrWhiteSpace(ndefText))
                {
                    return;
                }

                // Déchiffrer le message reçu
                long[] clePrivee = new long[] { long.Parse(N), D };
                string decryptedMessage = RSA.DecryptString(ndefText, clePrivee);
                MessageRecu.Text = decryptedMessage;
                BtnRecevoir.Text = "Message reçu et déchiffré";
            });

            RfidToolsGetter.SetOffTagRead();
        });
    }
}