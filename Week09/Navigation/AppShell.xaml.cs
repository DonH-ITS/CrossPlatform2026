namespace Week9Demo
{
    public partial class AppShell : Shell
    {
        public AppShell() {
            InitializeComponent();
            Routing.RegisterRoute("details", typeof(DetailsPage));
            Routing.RegisterRoute("settings", typeof(SettingsPage));

        }
    }
}
