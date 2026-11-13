namespace APIExample
{
    public partial class MainPage : ContentPage
    {
        TriviaModel _triviaModel;
        public MainPage() {
            InitializeComponent();
            _triviaModel = new TriviaModel();
            BindingContext = _triviaModel;
        }



    }
}
