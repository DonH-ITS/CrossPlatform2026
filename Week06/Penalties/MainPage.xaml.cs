namespace PenaltyKick
{

    public partial class MainPage : ContentPage
    {
        private const int MaxTime = 30;

        // How far (in pixels) the goalkeeper and the football move to the left or right.
        private const int SideOffset = 180;

        private bool _gamePlaying = false;
        private bool _ballMoving = false;

        private int _goals;
        private int _timeLeft;
        private int _bestScore;

        private IDispatcherTimer _gameTimer = null!;   // created in InitialiseTimer()
        private Random _random = new Random();

        private string _goaliePosition = "centre";

        // The three places the goalkeeper can stand, and the three directions you can shoot.
        private readonly string[] _directions = { "left", "centre", "right" };

        public MainPage() {
            InitializeComponent();

            InitialiseTimer();
            LoadBestScore();
        }

        private void InitialiseTimer() {
            _gameTimer = Dispatcher.CreateTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.Tick += TimerTick;
        }

        private void TimerTick(object? sender, EventArgs e) {
            _timeLeft--;
            TimeLbl.Text = $"Time: {_timeLeft}";

            if (_timeLeft <= 0) {
                EndGame();
            }
        }

        private async void StartBtn_Clicked(object sender, EventArgs e) {
            await StartGame();
        }

        private async Task StartGame() {
            _goals = 0;
            _timeLeft = MaxTime;
            ScoreLbl.Text = $"Score: {_goals}";
            TimeLbl.Text = $"Time: {_timeLeft}";

            // The goalkeeper stops wherever it was when the last game ended, so put it back.
            GoalKeeper.TranslationX = 0;

            _goaliePosition = "centre";

            FootballImg.TranslationX = 0;
            FootballImg.TranslationY = 0;
            FootballImg.Scale = 1;
            FootballImg.Rotation = 0;

            StartBtn.IsVisible = false;
            ControlGrid.IsVisible = true;

            _gamePlaying = true;
            _gameTimer.Start();

            // AnimateGoalKeeper only finishes once the game is over, so everything
            // below this line runs after the game has ended.
            await AnimateGoalKeeper();

            // Wait for any shot that is still in progress.
            while (_ballMoving) {
                await Task.Delay(50);
            }

            StartBtn.IsVisible = true;
        }

        private async void ShootButton_Clicked(object sender, EventArgs e) {
            if (!_gamePlaying || _ballMoving) {
                return;
            }

            Button btn = (Button)sender;

            if (btn.Text == "Shoot Left") {
                await MoveBall("left");
            }
            else if (btn.Text == "Shoot Centre") {
                await MoveBall("centre");
            }
            else if (btn.Text == "Shoot Right") {
                await MoveBall("right");
            }
        }

        // Maybe this method will work a little better than the _goaliePosition string.
        private string WhereIsGoalie() {
            if(GoalKeeper.TranslationX < -SideOffset / 2) {
                return "left";
            }
            else if (GoalKeeper.TranslationX > SideOffset / 2) {
                return "right";
            }
            else {
                return "centre";
            }
        }

        private async Task MoveBall(string direction) {
            _ballMoving = true;

            // Move, shrink and spin the football at the same time.
            await Task.WhenAll(
                FootballImg.TranslateToAsync(GetOffsetX(direction), -350, 500),
                FootballImg.ScaleToAsync(0.5, 500),
                FootballImg.RotateToAsync(360, 500));

            // The game may have ended while the ball was in the air.
            // If it has, the shot is too late and does not count.
            if (_gamePlaying) {

                if (direction == WhereIsGoalie()) {
                    await SaveShot();
                }
                else {
                    await ScoreGoal();
                }
            }

            // Slide the ball back to its starting position (growing it back to full size).
            await Task.WhenAll(
                FootballImg.TranslateToAsync(0, 0, 300),
                FootballImg.ScaleToAsync(1, 300));

            // A full 360 degree spin looks the same as 0, so this reset is invisible.
            FootballImg.Rotation = 0;

            _ballMoving = false;
        }

        private double GetOffsetX(string direction) {
            if (direction == "left") {
                return -SideOffset;
            }
            else if (direction == "right") {
                return SideOffset;
            }
            else {
                return 0;
            }
        }

        private async Task AnimateGoalKeeper() {
            while (_gamePlaying) {
                string target = _directions[_random.Next(3)];

                // The goalkeeper is on its way, so it cannot save anything yet.
                _goaliePosition = "moving";
                await GoalKeeper.TranslateToAsync(GetOffsetX(target), 0, 400);

                // Now it has arrived.
                _goaliePosition = target;

                // Stay in position for a moment.
                await Task.Delay(500);
            }
        }

        private async Task ScoreGoal() {
            _goals++;
            ScoreLbl.Text = $"Score: {_goals}";

            FeedbackLbl.Text = "GOAL!";
            FeedbackLbl.IsVisible = true;

            await Task.Delay(500);

            FeedbackLbl.IsVisible = false;
        }

        private async Task SaveShot() {
            FeedbackLbl.Text = "SAVED!";
            FeedbackLbl.IsVisible = true;
            if(WhereIsGoalie() == "left") {
                GoalKeeper.Source = "goalieleft.png";
            }
            else if(WhereIsGoalie() == "right") {
                GoalKeeper.Source = "goalieright.png";
            }
            await Task.Delay(500);
            GoalKeeper.Source = "goalie.png";
            FeedbackLbl.IsVisible = false;
        }

        private void EndGame() {
            _gamePlaying = false;
            _gameTimer.Stop();

            ControlGrid.IsVisible = false;

            // Save the new best score before showing the alert.
            if (_goals > _bestScore) {
                _bestScore = _goals;
                Preferences.Default.Set("BestScore", _bestScore);
                BestScoreLbl.Text = $"Best: {_bestScore}";
            }

            // No need to await: nothing here depends on the alert being closed.
            DisplayAlertAsync("Game Over", $"Final Score: {_goals}", "OK");
        }

        private void LoadBestScore() {
            _bestScore = Preferences.Default.Get("BestScore", 0);
            BestScoreLbl.Text = $"Best: {_bestScore}";
        }
    }
}