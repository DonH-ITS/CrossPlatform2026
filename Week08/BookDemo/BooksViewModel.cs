

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BookDemo
{
    public class BooksViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Command ChangeList { get; }
        public Command AddBookCommand { get; }

        private int _nextId = 4;

        private ObservableCollection<Book> _books;
        public ObservableCollection<Book> Books {
            get { return _books; }
            set {
                _books = value;
                OnPropertyChanged();
            }
        }

        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string PublishedYear { get; set; } = "";


        public BooksViewModel() {
            ChangeList = new Command(ListChange);
            AddBookCommand = new Command(AddBook);
            Books = new ();
            Books.Add(new Book { Id = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", PublishedYear = 1925 });
            Books.Add(new Book { Id = 2, Title = "1984", Author = "George Orwell", PublishedYear = 1949 });
            Books.Add(new Book { Id = 3, Title = "To Kill a Mockingbird", Author = "Harper Lee", PublishedYear = 1960 });
        }

        public void ListChange() {
            Books = new ();
            Books.Add(new Book { Id = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", PublishedYear = 1925 });
            Books.Add(new Book { Id = 2, Title = "Lord of the Rings", Author = "Tolkien", PublishedYear = 1949 });
            Books.Add(new Book { Id = 3, Title = "Big", Author = "Roald Dahl", PublishedYear = 1960 });
            Debug.WriteLine("New list");
        }

        private void AddBook() {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Author))
                return;

            if (!int.TryParse(PublishedYear, out int year))
                return;

            Books.Add(new Book
            {
                Id = _nextId++,
                Title = Title,
                Author = Author,
                PublishedYear = year
            });
            Debug.WriteLine("Added Book");
        }
    }
}
