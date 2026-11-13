namespace BookDemo
{
    public partial class MainPage : ContentPage
    {
        BooksViewModel _viewModel;
        public MainPage() {
            InitializeComponent();
            _viewModel = new BooksViewModel();
            BindingContext = _viewModel;
        }

       
    }
}
