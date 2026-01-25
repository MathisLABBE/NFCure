namespace NFCure;

public partial class RecevoirPage : ContentPage
{
	public string? N { get; set; }
    public string? E { get; set; }

    public RecevoirPage()
	{
        N = "N" ?? null;
        E = "E" ?? null;

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

        BtnRecevoir.IsEnabled = true;
        BtnRecevoir.Text = "Recevoir";

        MessageRecu.Text = "Text du message recu";
        MessageRecu.IsVisible = true;
    }
}