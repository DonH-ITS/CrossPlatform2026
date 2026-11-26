using Microsoft.Extensions.DependencyInjection;

namespace AlbumList
{
    public partial class App : Application
    {
        public static HttpClient Http = new();
        public App()
        {
            InitializeComponent();
            Http.DefaultRequestHeaders.UserAgent.ParseAdd("MyMauiApp/1.0");
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}