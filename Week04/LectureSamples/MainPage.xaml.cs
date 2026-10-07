using System.Timers;

namespace Week4Demo
{
    public partial class MainPage : ContentPage
    {
        private System.Timers.Timer? _systemTimer;
        private IDispatcherTimer? _dispatcherTimer;
        private int _systemTimerCount;
        private int _dispatcherTimerCount;

        public MainPage() {
            InitializeComponent();
        }

        // Start both timer examples.
        private void StartTimers(object? sender, EventArgs e) {
            _systemTimer = new System.Timers.Timer(1000);
            _systemTimer.Elapsed += OnSystemTimerElapsed;
            _systemTimer.AutoReset = true;
            _systemTimer.Enabled = true;

            _dispatcherTimer = Dispatcher.CreateTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherTimer.Tick += OnDispatcherTimerTick;
            _dispatcherTimer.Start();

            StatusLabel.Text = "Both timers are running.";
        }

        // System.Timers.Timer runs its event on a ThreadPool thread.
        private void OnSystemTimerElapsed(object? sender, ElapsedEventArgs e) {
            _systemTimerCount++;

            Dispatcher.Dispatch(() =>
            {
                SystemTimerLabel.Text = $"System timer: {_systemTimerCount}";
            });
        }

        // Dispatcher timer runs its Tick event on the UI thread.
        private void OnDispatcherTimerTick(object? sender, EventArgs e) {
            _dispatcherTimerCount++;
            DispatcherTimerLabel.Text = $"Dispatcher timer: {_dispatcherTimerCount}";
        }

        // Deliberately block the UI thread with the Dispatcher timer.
        private void StartBadDispatcherTimer(object? sender, EventArgs e) {
            _dispatcherTimer?.Stop();

            _dispatcherTimer = Dispatcher.CreateTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherTimer.Tick += OnBadDispatcherTimerTick;
            _dispatcherTimer.Start();

            StatusLabel.Text = "Bad DispatcherTimer started.";
        }

        // The UI freezes because this code runs on the UI thread.
        private void OnBadDispatcherTimerTick(object? sender, EventArgs e) {
            _dispatcherTimerCount++;
            DispatcherTimerLabel.Text = $"Dispatcher timer: {_dispatcherTimerCount}";

            Thread.Sleep(3000);
        }

        // Stop both timer examples.
        private void StopTimers(object? sender, EventArgs e) {
            _systemTimer?.Stop();

            // This is only if we never want the timers again, if we want to keep just starting and stopping
            // them, we can just call Stop() and Start() without disposing of them.
            /*_systemTimer?.Dispose();
            _systemTimer = null;*/

            _dispatcherTimer?.Stop();
            //_dispatcherTimer = null;

            StatusLabel.Text = "Timers stopped.";
        }

        // Deliberately block the UI thread.
        private void BlockButton_Clicked(object? sender, EventArgs e) {
            StatusLabel.Text = "Blocking...";

            Thread.Sleep(5000);

            StatusLabel.Text = "Finished blocking.";
        }

        // Wait asynchronously without blocking the UI thread.
        private async void AsyncButton_Clicked(object? sender, EventArgs e) {
            StatusLabel.Text = "Waiting...";

            await Task.Delay(5000);

            StatusLabel.Text = "Finished waiting.";
        }

        // Run CPU-intensive work on a ThreadPool thread.
        private async void CalculateButton_Clicked(object? sender, EventArgs e) {
            StatusLabel.Text = "Calculating...";

            long result = await Task.Run(() =>
            {
                long total = 0;

                for (long i = 0; i < 500000000; i++) {
                    total += i;
                }

                return total;
            });

            StatusLabel.Text = $"Result: {result:N0}";
        }

        // Move the box using a built-in MAUI animation.
        private async void MoveButton_Clicked(object? sender, EventArgs e) {
            await AnimationBox.TranslateToAsync(150, 0, 1000);
            await AnimationBox.TranslateToAsync(0, 0, 1000);
        }

        // Fade the box in and out.
        private async void FadeButton_Clicked(object? sender, EventArgs e) {
            await AnimationBox.FadeToAsync(0, 1000);
            await AnimationBox.FadeToAsync(1, 1000);
        }

        // Change the size of the box.
        private async void ScaleButton_Clicked(object? sender, EventArgs e) {
            await AnimationBox.ScaleToAsync(1.5, 500);
            await AnimationBox.ScaleToAsync(1.0, 500);
        }

        // Run several animations at the same time.
        private async void SimultaneousButton_Clicked(object? sender, EventArgs e) {
            await Task.WhenAll(
                AnimationBox.FadeToAsync(0.3, 1000),
                AnimationBox.ScaleToAsync(1.5, 1000),
                AnimationBox.RotateToAsync(360, 1000)
            );

            AnimationBox.Opacity = 1;
            AnimationBox.Scale = 1;
            AnimationBox.Rotation = 0;
        }

        // Run several animations one after another.
        private async void SequenceButton_Clicked(object? sender, EventArgs e) {
            await AnimationBox.FadeToAsync(0.3, 500);
            await AnimationBox.ScaleToAsync(1.5, 500);
            await AnimationBox.RotateToAsync(360, 500);

            AnimationBox.Opacity = 1;
            AnimationBox.Scale = 1;
            AnimationBox.Rotation = 0;
        }

        // Show a simple alert.
        private async void AlertButton_Clicked(object? sender, EventArgs e) {
            await DisplayAlertAsync("Hello", "This is a MAUI alert.", "OK");
        }

        // Ask for a name and handle Cancel.
        private async void NameButton_Clicked(object? sender, EventArgs e) {
            string? name = await DisplayPromptAsync("Question", "What's your name?");

            if (name != null) {
                StatusLabel.Text = $"Hello {name}!";
            }
            else {
                await DisplayAlertAsync("⚠ Warning", "Why you no give name", "😠 OK");
            }
        }
    }
}
