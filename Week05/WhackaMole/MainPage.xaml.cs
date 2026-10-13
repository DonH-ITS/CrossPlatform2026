using Microsoft.Maui.Controls.Shapes;

namespace WhackAMole
{

    public partial class MainPage : ContentPage
    {
        private int _score;
        int _timeRemaining;
        IDispatcherTimer? _gameTimer;
        bool _gameRunning = false;
        bool _gridCreated = false;
        int _gridSize;
        // Missed isn't in the lab I shared but I thought it'd be a nice one to show
        int _missed;
        Random _random;


        public MainPage()
        {
            InitializeComponent();
            _gridSize = 3;
            CreateTimers();
            _random = new Random();
        }

        private void CreateTimers()
        {
            _gameTimer = Dispatcher.CreateTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.Tick += CountDownTick;
        }

        private void CountDownTick(object? sender, EventArgs e)
        {
            --_timeRemaining;
            TimeLabel.Text = $"Time: {_timeRemaining}";
            if(_timeRemaining <= 0)
                EndGame();
            
        }

        // You may think EndGame things can just go in the CountDownTick
        // But new task = new method
        // This is example of keeping code organised
        // And EndGame can be called elsewhere, such as when the player misses too many moles
        private void EndGame() {
            _gameTimer?.Stop();
            _gameRunning = false;
            Mole.CancelAnimations();
            Mole.Opacity = 0;
            Mole.InputTransparent = true;
            StartButton.IsEnabled = true;
            TimeLabel.Text = "Game Over!";
        }

        private async void StartGame_Clicked(object sender, EventArgs e)
        {
            // The button should be disabled when the game is running
            // But just in case, let's make sure it can't do anything
            if (_gameRunning) {
                return;
            }

            // Let the gridsize change the first time
            // This isn't in the lab book description, but I included it here anyway
            // It only creates the grid once, and then locks the size in place for further games
            // We could have it change, but that would require things I have not talked about in lectures
            if (!_gridCreated) {
                int.TryParse(GridSizeEntry.Text, out _gridSize);
                if (_gridSize < 3)
                    _gridSize = 3;
                else if (_gridSize > 7) 
                    _gridSize = 7;
                GridSizeEntry.Text = _gridSize.ToString();
                GridSizeEntry.IsEnabled = false;
                CreateGrid(_gridSize);
                _gridCreated = true;
            }
            StartButton.IsEnabled = false; 
            _gameRunning = true; 
            _timeRemaining = 30;
            _score = 0;
            _missed = 0;
            ScoreLabel.Text = $"Score: {_score}";
            MissedLabel.Text = $"Missed: {_missed}";
            _gameTimer?.Start();
            TimeLabel.Text = $"Time: {_timeRemaining}";
            await ShowMole(); 
        }

        private async Task ShowMole() {
            while (_gameRunning) {
                MoveMole();
                Mole.Opacity = 1;
                Mole.InputTransparent = false;
                Mole.Scale = 0;
                Mole.Rotation = 0;
                // Easing.SpringOut gives a spring like effect to the animation
                // It's not needed and maybe looks a little strange, so leave it out if you want
                await Task.WhenAll(
                    Mole.ScaleToAsync(1, 750, Easing.SpringOut),
                    Mole.RotateToAsync(360, 750, Easing.SpringOut)
                );
                bool clicked = await Mole.FadeToAsync(0, 750);
                // This makes sure we don't get stuck in an infinite loop when CancelAnimations is called.
                await Task.Yield();
                // If clicked is false, then the mole was not tapped
                if (!clicked) {
                    ++_missed;
                    MissedLabel.Text = $"Missed: {_missed}";
                    if(_missed > 3)
                        EndGame();
                }
                Mole.InputTransparent = true;
            }
        }



        private void CreateGrid(int size)
        {
            // This first bit is to make the mole a different size in each grid size
            // This would be a lot easier if we just used an image to start with
            // It also is not required if you stick with a 3x3 grid
            double cell = GameGrid.WidthRequest / size;
            MoleLabel.FontSize = cell * 0.4;
            Mole.Margin = new Thickness(cell * 0.05);
            Mole.Padding = new Thickness(cell * 0.1);
            for (int i = 0; i < size; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition());
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Border border = new Border
                    {
                        BackgroundColor = Colors.LightGreen,
                        Stroke = Colors.DarkGreen,
                        StrokeThickness = 3,
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = 15
                        },
                        Margin = new Thickness(5)
                    };
                    GameGrid.Add(border, j, i);
                }
            }
        }

        private void MoveMole()
        {
            int row = _random.Next(_gridSize);
            int column = _random.Next(_gridSize);
            Grid.SetRow(Mole, row);
            Grid.SetColumn(Mole, column);
        }

        private void Mole_Tapped(object sender, TappedEventArgs e)
        {
            IncreaseScore();
            Mole.CancelAnimations();
        }

        private void IncreaseScore()
        {
            _score += 10;
            ScoreLabel.Text = $"Score: {_score}";
        }


    }
}
