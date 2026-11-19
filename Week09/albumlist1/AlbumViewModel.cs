
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace AlbumList
{
    public class AlbumViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Album> _albums = new();
        private string _libraryName = "";
        private bool _isLoading = false;
        private string _errorMessage = "";
        private string _apiText = "Testing Binding";
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string Library1Url = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/refs/heads/main/library1.json";
        private const string Library2Url = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/refs/heads/main/library2.json";
        public Command LoadLibrary1Command { get; }
        public Command LoadLibrary2Command { get; }
        public Command<string> SortCommand { get; }



        public ObservableCollection<Album> Albums
        {
            get => _albums;
            set
            {
                if (_albums != value)
                {
                    _albums = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LibraryName
        {
            get => _libraryName;
            set
            {
                if (!string.IsNullOrEmpty(value) && _libraryName != value)
                {
                    _libraryName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (!string.IsNullOrEmpty(value) && _errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ApiText
        {
            get => _apiText;
            set
            {
                if (!string.IsNullOrEmpty(value) && _apiText != value)
                {
                    _apiText = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public AlbumViewModel()
        {
            LoadLibrary1Command = new Command(async () => await LoadLibraryAsync(Library1Url, "Library 1"));
            LoadLibrary2Command = new Command(async () => await LoadLibraryAsync(Library2Url, "Library 2"));
            SortCommand = new Command<string>(SortAlbums);
        }


        private async Task LoadLibraryAsync(string url, string libraryName)
        {
            IsLoading = true;
            ErrorMessage = "";
            ApiText = "";
            try
            {
                string fileName = Path.Combine(FileSystem.Current.AppDataDirectory, libraryName + ".json");
                // If the cached file exists, use it
                // Probably need to check the library name for invalid characters but fine for now
                if (File.Exists(fileName))
                {
                    string text = await File.ReadAllTextAsync(fileName);
                    Albums = JsonSerializer.Deserialize<ObservableCollection<Album>>(text);
                    LibraryName = libraryName;
                }
                else
                {

                    // Send an HTTP GET request using _httpClient.
                    using var response = await _httpClient.GetAsync(url);
                    // Check that the response was successful.
                    if (response.IsSuccessStatusCode)
                    {
                        string text = await response.Content.ReadAsStringAsync();
                        Albums = JsonSerializer.Deserialize<ObservableCollection<Album>>(text);
                        LibraryName = libraryName;
                        // Cache the file locally
                        await File.WriteAllTextAsync(fileName, text);

                    }

                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }
            finally
            {
                // Finally ALWAYS runs after the others
                IsLoading = false;
            }
        }

        public void SortAlbums(string sortBy)
        {
            switch (sortBy)
            {
                case "Title":
                    Albums = new ObservableCollection<Album>(
                        Albums.OrderBy(a => a.Title));
                    break;

                case "Artist":
                    Albums = new ObservableCollection<Album>(
                        Albums.OrderBy(a => a.Artist));
                    break;

                case "Year":
                    Albums = new ObservableCollection<Album>(
                        Albums.OrderBy(a => a.Year));
                    break;
            }
        }













        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
