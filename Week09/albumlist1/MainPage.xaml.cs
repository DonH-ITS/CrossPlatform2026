namespace AlbumList
{
    public partial class MainPage : ContentPage
    {
        AlbumViewModel _viewmodel;
        public MainPage()
        {
            InitializeComponent();
            _viewmodel = new AlbumViewModel();
            BindingContext = _viewmodel;
        }

      
    }
}
