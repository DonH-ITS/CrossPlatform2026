using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace APIExample
{
    public class TriviaModel : INotifyPropertyChanged
    {
        private static readonly HttpClient _httpClient = new();
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _outputAPI = "Output from API will appear here";


        public ObservableCollection<Question> Questions { get; } = new();

        private bool _isBusy;
        private string _statusMessage = "";

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                if (_isBusy == value)
                    return;

                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                if (_statusMessage == value)
                    return;

                _statusMessage = value;
                OnPropertyChanged();
            }
        }
        public string OutputAPI
        {
            get => _outputAPI;
            set
            {
                if (_outputAPI != value) {
                    _outputAPI = value;
                    OnPropertyChanged();
                }
            }
        }

        public Command LoadQuestionsCommand { get; }

        public TriviaModel() {
            LoadQuestionsCommand = new Command(async () => await LoadQuestions());
        }

        private async Task LoadQuestions() {
            if (IsBusy)
                return;

            try {
                IsBusy = true;
                StatusMessage = "";
                Questions.Clear();

                string url = "https://opentdb.com/api.php?amount=5&type=multiple";

                using var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode) {
                    StatusMessage = "Could not retrieve the questions.";
                    return;
                }

                string json = await response.Content.ReadAsStringAsync();
                TriviaResponse? data = JsonSerializer.Deserialize<TriviaResponse>(json);

                if (data == null || data.response_code != 0) {
                    StatusMessage = "The API did not return any questions.";
                    return;
                }

                foreach (Question question in data.results) {
                    question.question = WebUtility.HtmlDecode(question.question);
                    question.correct_answer = WebUtility.HtmlDecode(question.correct_answer);

                    for (int i = 0; i < question.incorrect_answers.Count; i++) {
                        question.incorrect_answers[i] =
                            WebUtility.HtmlDecode(question.incorrect_answers[i]);
                    }

                    Questions.Add(question);
                }
            }
            catch (Exception) {
                StatusMessage = "There was a problem retrieving the questions.";
            }
            finally {
                IsBusy = false;
            }
        }



        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
