using LibraryWin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LibraryWin
{
    public partial class AdminWindow : Window
    {
        private LibraryDBContext _context;
        private ObservableCollection<AllBooksInfo> _allBooks;

        public AdminWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new LibraryDBContext())
            {
                BooksDataGrid.ItemsSource = context.Books.ToList();
                AuthorsDataGrid.ItemsSource = context.Authors.ToList();
                GenresDataGrid.ItemsSource = context.Genres.ToList();
                ReadersDataGrid.ItemsSource = context.Readers.ToList();
                BookIssuancesDataGrid.ItemsSource = context.BookIssuances.ToList();
                DepartmentsDataGrid.ItemsSource = context.DepartmentOfIssuances.ToList();
                PublishingHousesDataGrid.ItemsSource = context.PublishingHouses.ToList();
                RacksDataGrid.ItemsSource = context.Racks.ToList();
                LibrariansDataGrid.ItemsSource = context.Librarians.ToList();
                HallEmployeesDataGrid.ItemsSource = context.HallEmployees.ToList();
            }
        }

        private void BooksDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            // Запрещаем редактирование, если это не книга
            if (!(e.Row.Item is Book))
            {
                e.Cancel = true;
            }
        }

        private void BooksDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var row = e.Row.GetIndex();
                var column = e.Column.DisplayIndex;

                if (row == BooksDataGrid.Items.Count - 1 && column == 0) // ячейка для ввода нового ID
                {
                    var newBookId = int.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);

                    using (var context = new LibraryDBContext())
                    {
                        var newBook = new Book
                        {
                            BookId = newBookId,
                            Name = "",
                            Isbn = "",
                            NumberOfPages = 0,
                            PublishingHouse = new PublishingHouse { Name = "" },
                            Rack = new Rack { RackNumber = "" }
                        };

                        context.Books.Add(newBook);
                        context.SaveChanges();

                        BooksDataGrid.ItemsSource = context.Books.Include(b => b.PublishingHouse).Include(b => b.Rack).ToList();
                    }
                }
                else
                {
                    var book = (Book)e.Row.Item;
                    var columnName = e.Column.SortMemberPath;

                    switch (columnName)
                    {
                        case "BookId":
                            book.BookId = int.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                            break;
                        case "Name":
                            book.Name = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                            break;
                        case "Isbn":
                            book.Isbn = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                            break;
                        case "NumberOfPages":
                            book.NumberOfPages = int.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                            break;
                        case "PublishingHouse.Name":
                            book.PublishingHouse.Name = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                            break;
                        case "Rack.RackNumber":
                            book.Rack.RackNumber = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                            break;
                    }

                    using (var context = new LibraryDBContext())
                    {
                        context.Entry(book).State = EntityState.Modified;
                        void BooksDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
                        {
                            if (e.EditAction == DataGridEditAction.Commit)
                            {
                                var book = new Book();
                                var columnName = e.Column.SortMemberPath;

                                switch (columnName)
                                {
                                    case "BookId":
                                        // Проверьте, что значение BookId уникально и не используется в других записях
                                        book.BookId = int.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                                        break;
                                    case "Name":
                                        book.Name = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                                        break;
                                    case "Isbn":
                                        book.Isbn = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                                        break;
                                    case "NumberOfPages":
                                        book.NumberOfPages = int.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                                        break;
                                    case "PublishingHouse.Name":
                                        // Создайте новый объект PublishingHouse и добавьте его в контекст
                                        var publishingHouse = new PublishingHouse { Name = e.EditingElement.GetValue(TextBox.TextProperty) as string };
                                        context.PublishingHouses.Add(publishingHouse);
                                        context.SaveChanges();
                                        book.PublishingHouseId = publishingHouse.PublishingHouseId;
                                        break;
                                    case "Binding Rack.RackNumber":
                                        // Создайте новый объект Rack и добавьте его в контекст
                                        var rack = new Rack { RackNumber = e.EditingElement.GetValue(TextBox.TextProperty) as string };
                                        context.Racks.Add(rack);
                                        context.SaveChanges();
                                        book.RackId = rack.RackId;
                                        break;
                                        // Добавьте другие столбцы, которые вы хотите отслеживать
                                }

                                context.Books.Add(book);
                                context.SaveChanges();
                            }
                        }
                            context.SaveChanges();
                    }
                }
            }
        }

        private void BooksDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedBook = BooksDataGrid.SelectedItem as Book;
                if (selectedBook != null)
                {
                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить эту книгу?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new LibraryDBContext())
                        {
                            context.Books.Remove(selectedBook);
                            context.SaveChanges();
                            LoadData(); // Перезагрузка данных после удаления
                            MessageBox.Show("Книга удалена");
                        }
                    }
                }
            }
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditBookDialog();
            if (dialog.ShowDialog() == true)
            {
                using (var context = new LibraryDBContext())
                {
                    var newBook = dialog.Book;
                    context.Books.Add(newBook);
                    context.SaveChanges();
                    LoadData(); // Перезагрузка данных после изменения
                }
            }
        }

        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для редактирования выбранной книги
        }

        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для удаления выбранной книги
        }

        private void AddAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для добавления автора
        }

        private void EditAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для редактирования выбранного автора
        }

        private void DeleteAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для удаления выбранного автора
        }

        private void AddGenreButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для добавления жанра
        }

        private void EditGenreButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для редактирования выбранного жанра
        }

        private void DeleteGenreButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для удаления выбранного жанра
        }

        private void AddReaderButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для добавления читателя
        }

        private void EditReaderButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для редактирования выбранного читателя
        }

        private void DeleteReaderButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для удаления выбранного читателя
        }

        private void AddBookIssuanceButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для добавления выдачи книги
        }

        private void EditBookIssuanceButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для редактирования выбранной выдачи книги
        }

        private void DeleteBookIssuanceButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для удаления выбранной выдачи книги
        }

        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для добавления отдела
        }

        private void EditDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для редактирования выбранного отдела
        }

        private void DeleteDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для удаления выбранного отдела
        }

        private void AddPublishingHouseButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для добавления издательства
        }

        private void EditPublishingHouseButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для редактирования выбранного издательства
        }

        private void DeletePublishingHouseButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для удаления выбранного издательства
        }

        private void AddRackButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для добавления стеллажа
        }

        private void EditRackButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для редактирования выбранного стеллажа
        }

        private void DeleteRackButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для удаления выбранного стеллажа
        }

        private void AddLibrarianButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для добавления библиотекаря
        }

        private void EditLibrarianButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для редактирования выбранного библиотекаря
        }

        private void DeleteLibrarianButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для удаления выбранного библиотекаря
        }

        private void AddHallEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для добавления сотрудника зала
        }

        private void EditHallEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для редактирования выбранного сотрудника зала
        }

        private void DeleteHallEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для удаления выбранного сотрудника зала
        }

    }
}

