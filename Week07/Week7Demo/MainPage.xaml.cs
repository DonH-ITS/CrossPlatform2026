namespace GithubDemo
{

    public partial class MainPage : ContentPage
    {
        private bool _alternateColour;

        private bool _isGameRunning;
        private bool _anotherField;

        private string _name = "";

        private int _score;
        private string _scoreInput = "0";
        private bool _isScoreInvalid;

        public MainPage() {
            InitializeComponent();

            BindingContext = this;
        }

        // ------------------------------------
        // Changing a DynamicResource
        // ------------------------------------

        private void ChangeColour_Clicked(object sender, EventArgs e) {
            _alternateColour = !_alternateColour;

            if (_alternateColour) {
                Resources["DemoColour"] = Colors.DarkOrange;
            }
            else {
                Resources["DemoColour"] = Color.FromArgb("#512BD4");
            }
        }

        // ------------------------------------
        // Name property
        // ------------------------------------

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        // ------------------------------------
        // Game status properties
        // ------------------------------------

        public bool IsGameRunning
        {
            get => _isGameRunning;
            set
            {
                _isGameRunning = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(IsGameStopped));
            }
        }

        public bool IsGameStopped => !IsGameRunning;

        private void ToggleGame_Clicked(object sender, EventArgs e) {
            IsGameRunning = !IsGameRunning;
        }

        // ------------------------------------
        // Score and input validation
        // ------------------------------------

        public int Score => _score;

        public string ScoreInput
        {
            get => _scoreInput;
            set
            {
                _scoreInput = value;

                if (int.TryParse(value, out int result) &&
                    result >= 0 && result <= 100) {
                    _score = result;
                    IsScoreInvalid = false;

                    OnPropertyChanged(nameof(Score));
                }
                else {
                    IsScoreInvalid = true;
                }

                OnPropertyChanged();
            }
        }

        public bool IsScoreInvalid
        {
            get => _isScoreInvalid;
            set
            {
                _isScoreInvalid = value;
                OnPropertyChanged();
            }
        }
    }
}
