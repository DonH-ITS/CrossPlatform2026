using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Week8Demo
{
    public class MyViewModel : INotifyPropertyChanged
    {
        private string _myText = "";
        private int _timeLeft = 0;
        private int _score = 0;
        System.Timers.Timer timer;

        public Command ChangeTextCommand { get; }
        public Command ChangeScoreCommand { get; }
        public int TimeLeft
        {
            get
            {
                return _timeLeft;
            }
            set
            {
                if (_timeLeft == value)
                    return;

                _timeLeft = value;
                OnPropertyChanged();
            }
        }

        public int Score
        {
            get
            {
                return _score;
            }
            set
            {
                if (_score == value)
                    return;
                _score = value;
                OnPropertyChanged();
            }
        }

        public string MyText
        {
            get
            {
                return _myText;
            }
            set
            {
                if (_myText == value)
                    return;

                _myText = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MyViewModel() {


            // Instantiate the object with an interval of 1000ms
            timer = new System.Timers.Timer
            {
                Interval = 1000
            };
            
            // Hook up the EventHandler for it
            timer.Elapsed += OnTimerElapsed;

            // Start the timer
            timer?.Start();

            // This will work for a DispatcherTimer in a ViewModel
            // The Application.Current is what I was forgetting in the lecture
            //_timer = Application.Current.Dispatcher.CreateTimer();

            ChangeTextCommand = new Command(MakeaChange);
            ChangeScoreCommand = new Command<string>(ChangeScore);
        }

        public void OnTimerElapsed(object sender, EventArgs e) {
            --TimeLeft;
        }

        public void MakeaChange() {
            MyText = "Hello World";
            TimeLeft = 20;
        }
        
        public void ChangeScore(string score) {
            /*if (score == "-1")
                Score--;
            else if(score == "1")
                Score++;*/
            // Better to do this
            if(int.TryParse(score, out int result)) {
                Score += result;
            }
        }
    }
}
