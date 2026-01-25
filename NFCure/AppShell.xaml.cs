namespace NFCure
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EnvoyerPage), typeof(EnvoyerPage));
            Routing.RegisterRoute(nameof(RecevoirPage), typeof(RecevoirPage));
        }
    }
}
