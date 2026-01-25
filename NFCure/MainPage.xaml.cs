namespace NFCure
{
    public partial class MainPage : ContentPage
    {
        public ProgramRSA Rsa;



        public MainPage()
        {
            InitializeComponent();

            Rsa = new ProgramRSA();
        }

        private async void BtnPageEnvoyer_Click(object sender, EventArgs e)
        {
            //await Shell.Current.GoToAsync(nameof(EnvoyerPage));
            await Navigation.PushAsync(new EnvoyerPage());
        }

        private async void BtnPageRecevoir_Click(object sender, EventArgs e)
        {
            //await Shell.Current.GoToAsync(nameof(RecevoirPage));
            await Navigation.PushAsync(new RecevoirPage(Rsa));
        }
    }
}
