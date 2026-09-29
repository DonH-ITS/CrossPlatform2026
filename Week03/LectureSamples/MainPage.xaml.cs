namespace Week3Lecture
{
    public partial class MainPage : ContentPage
    {
        private IDispatcherTimer? _timer;

        private int _labelNumber = 1;

        public MainPage() {
            InitializeComponent();

            // Lambda event handler.
            btnLambda.Clicked += (sender, e) =>
            {
                lblLambda.Text = "The lambda event handler ran!";
            };
        }


        // Button events
        private void OnColourButtonClicked(object sender, EventArgs e) {
            if (sender is Button button) {
                lblMessage.Text = $"You clicked the {button.Text} button.";

                if (button.Text == "Red") {
                    lblMessage.TextColor = Colors.Red;
                }
                else if (button.Text == "Blue") {
                    lblMessage.TextColor = Colors.Blue;
                }
            }
        }


        // Entry events and validation
        private void OnQuantityTextChanged(object sender, TextChangedEventArgs e) {
            lblInput.Text = $"Old value: {e.OldTextValue}\nNew value: {e.NewTextValue}";
        }


        private void OnValidateClicked(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(txtQuantity.Text)) {
                lblResult.Text = "Please enter a quantity.";
                return;
            }

            if (!double.TryParse(txtQuantity.Text, out double quantity)) {
                lblResult.Text = "Please enter a valid number.";
                return;
            }

            lblResult.Text = $"Quantity entered: {quantity}";
        }


        // ---------------------------------------------------------
        // Creating controls at runtime
        // ---------------------------------------------------------

        private void OnCreateLabelClicked(object sender, EventArgs e) {
            Label label = new Label
            {
                Text = $"Created at runtime: Label {_labelNumber}",
                FontSize = 16
            };

            MainVerticalLayout.Add(label);

            _labelNumber++;
        }

        // DispatcherTimer
        private void CreateTimer() {
            if (_timer != null) {
                return;
            }

            _timer = Dispatcher.CreateTimer();

            _timer.Interval = TimeSpan.FromSeconds(1);

            _timer.Tick += OnTimerTick;
        }


        private void OnStartTimerClicked(object sender, EventArgs e) {
            CreateTimer();

            _timer?.Start();

            lblTime.Text = "Timer running...";
        }


        private void OnStopTimerClicked(object sender, EventArgs e) {
            _timer?.Stop();

            lblTime.Text = "Timer stopped.";
        }


        private void OnTimerTick(object sender, EventArgs e) {
            lblTime.Text = DateTime.Now.ToLongTimeString();
        }


        private void OnDisposeTimerClicked(object sender, EventArgs e) {
            _timer?.Stop();
           // _timer?.
           // _timer = null;

            lblTime.Text = "Timer disposed.";
        }

        // Gestures

        private void OnBoxTapped(object sender, TappedEventArgs e) {
            lblGesture.Text = "The box was tapped!";
        }

        private void OnSwipeGesture(object sender, SwipedEventArgs e) {
            lblGesture.Text = $"Swiped {e.Direction}";
        }
    }
}
