namespace Week8Demo
{
    public partial class MainPage : ContentPage
    {
        MyViewModel _viewModel;
        public MainPage() {
            InitializeComponent();
            _viewModel = new MyViewModel();
            BindingContext = _viewModel;
        }

       
    }
}
