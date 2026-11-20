using Microsoft.Extensions.DependencyInjection;

namespace Week9Demo
{
    public partial class App : Application
    {
        public static AppSettings Settings { get; } = new AppSettings();
        public static HttpClient Http { get; } = new HttpClient();
        public App() {
            InitializeComponent();
            
        }

        protected override Window CreateWindow(IActivationState? activationState) {
            return new Window(new AppShell());
        }
    }
}