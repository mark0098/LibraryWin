// MainWindow.xaml.cs
using System.Linq;
using System.Windows;
using LibraryWin.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryWin
{
    public partial class MainWindow : Window
    {
        private LibraryDBContext context;

        public MainWindow()
        {
            
            InitializeComponent();
            LibraryDBContext DBContext = new();
            context = DBContext;
            LoadBooks();
            PopulateGenreFilter();
        }
        
        
        private void LoadBooks()
        {
            var books = context.AllBooksInfos.ToList();
            BooksDataGrid.ItemsSource = books;
        }

        private void PopulateGenreFilter()
        {
            var genres = context.AllBooksInfos
                .Select(b => b.ЖанроваяГруппа)
                .Distinct()
                .ToList();
            GenreFilter.ItemsSource = genres;
        }

        private void OnFilterClick(object sender, RoutedEventArgs e)
        {
            var selectedGenre = GenreFilter.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedGenre))
            {
                var filteredBooks = context.AllBooksInfos
                    .Where(b => b.ЖанроваяГруппа == selectedGenre)
                    .ToList();
                BooksDataGrid.ItemsSource = filteredBooks;
            }
            else
            {
                BooksDataGrid.ItemsSource = context.AllBooksInfos.ToList();
            }
        }
    }
}
