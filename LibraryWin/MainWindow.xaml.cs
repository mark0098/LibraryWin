// MainWindow.xaml.cs
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LibraryWin.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace LibraryWin
{
    public partial class MainWindow : Window
    {
        private LibraryDBContext context;
        public string CurrentReaderEmail { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            using (LibraryDBContext DBContext = new())
            {
                context = DBContext;
                LoadBooks();
                PopulateGenreFilter();
            }
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
        }

        private void OnFilterClick(object sender, RoutedEventArgs e)
        {
            var query = context.AllBooksInfos.AsQueryable();

            if (!string.IsNullOrEmpty(SearchTextBox.Text))
            {

                if (AuthorCheckBox.IsChecked == true)
                {
                    query = query.Where(b => b.Автор.Contains(SearchTextBox.Text));
                }
                if (GenreCheckBox.IsChecked == true)
                {
                    query = query.Where(b => b.ЖанроваяГруппа.Contains(SearchTextBox.Text));
                }
                if (NameCheckBox.IsChecked == true)
                {
                    query = query.Where(b => b.Имя.Contains(SearchTextBox.Text));
                }
                if (CountryCheckBox.IsChecked == true)
                {
                    query = query.Where(b => b.Страна.Contains(SearchTextBox.Text));
                }
                if (PublisherCheckBox.IsChecked == true)
                {
                    query = query.Where(b => b.Издательство.Contains(SearchTextBox.Text));
                }
                if (HallCheckBox.IsChecked == true)
                {
                    query = query.Where(b => b.НомерЗала.Contains(SearchTextBox.Text));
                }

                MessageBox.Show("Фильтр применен");
            }

            BooksDataGrid.ItemsSource = query.ToList();
        }

        private void BooksDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb)
            {
                // Убираем выделение у предыдущего блока
                if (BooksTextBlock != null && BooksTextBlock != tb)
                {
                    BooksTextBlock.FontWeight = FontWeights.Normal;
                    BooksTextBlock.FontSize = 16;
                }
                if (ProfileTextBlock != null && ProfileTextBlock != tb)
                {
                    ProfileTextBlock.FontWeight = FontWeights.Normal;
                    ProfileTextBlock.FontSize = 16;
                }

                // Выделяем текст текущего блока
                tb.FontWeight = FontWeights.Bold;
                tb.FontSize = 18;
            }
        }

        private async void ProfileTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Retrieve the current reader's information from the database using the email
            using (var context = new LibraryDBContext())
            {
                var reader = await context.Readers.FirstOrDefaultAsync(r => r.Email == CurrentReaderEmail);

                if (reader != null)
                {
                    // Open the profile window
                    ProfileWindow profileWindow = new ProfileWindow(reader);
                    profileWindow.Show();
                }
                else
                {
                    MessageBox.Show("Не удалось получить информацию о читателе.");
                }
            }
        }

    }
}