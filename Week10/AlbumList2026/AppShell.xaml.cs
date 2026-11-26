namespace AlbumList
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AlbumDetailPage), typeof(AlbumDetailPage));
            Routing.RegisterRoute(nameof(AlbumEditPage), typeof(AlbumEditPage));
            Library1Tab.Title = Preferences.Default.Get("library1", "My Library 1");
            Library2Tab.Title = Preferences.Default.Get("library2", "My Library 2");
        }
    }
}
