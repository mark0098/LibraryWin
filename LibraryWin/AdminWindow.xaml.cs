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
using System.Reflection.PortableExecutable;

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

        private void ContextMenu_Delete_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = sender as MenuItem;
            if (contextMenu != null)
            {
                var dataGrid = ((contextMenu.Parent as ContextMenu).PlacementTarget as DataGrid);
                if (dataGrid != null)
                {
                    if (dataGrid.SelectedItem is Book selectedBook)
                    {
                        DeleteItem(selectedBook);
                    }
                    else if (dataGrid.SelectedItem is Reader selectedReader)
                    {
                        DeleteItem(selectedReader);
                    }
                    else if (dataGrid.SelectedItem is BookIssuance selectedBookIssuance)
                    {
                        DeleteItem(selectedBookIssuance);
                    }
                    else if (dataGrid.SelectedItem is DepartmentOfIssuance selectedDepartment)
                    {
                        DeleteItem(selectedDepartment);
                    }
                    else if (dataGrid.SelectedItem is PublishingHouse selectedPublishingHouse)
                    {
                        DeleteItem(selectedPublishingHouse);
                    }
                    else if (dataGrid.SelectedItem is Rack selectedRack)
                    {
                        DeleteItem(selectedRack);
                    }
                    else if (dataGrid.SelectedItem is Librarian selectedLibrarian)
                    {
                        DeleteItem(selectedLibrarian);
                    }
                }
            }
        }

        private void DeleteItem<T>(T item) where T : class
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (var context = new LibraryDBContext())
                {
                    context.Set<T>().Remove(item);
                    context.SaveChanges();
                    LoadData();
                    MessageBox.Show("Запись удалена");
                }
            }
        }


        private bool isNewRow = false; // флаг, указывающий, является ли текущая строка новой
        private object oldValue = null; // значение ячейки до ее редактирования

        private void BooksDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            // запоминаем значение ячейки до ее редактирования
            oldValue = e.Row.Item;
        }



        private void BooksDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var book = (Book)e.Row.Item;
                var columnName = e.Column.SortMemberPath;

                switch (columnName)
                {
                    case "BookId":
                        // если BookId изменен, то это существующая строка
                        isNewRow = false;

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
                    case "Binding Rack.RackNumber":
                        book.Rack.RackNumber = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                }
                if (book.BookId <= 0 ||
            string.IsNullOrWhiteSpace(book.Name) ||
            string.IsNullOrWhiteSpace(book.Isbn) ||
            book.NumberOfPages <= 0 ||
            string.IsNullOrWhiteSpace(book.PublishingHouse.Name) ||
            string.IsNullOrWhiteSpace(book.Rack.RackNumber))
                {
                    MessageBox.Show("Все поля должны быть заполнены корректными данными!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Cancel = true;
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // проверяем, является ли текущая строка новой
                    if (isNewRow)
                    {
                        // если это новая строка, то добавляем ее в базу данных
                        using (var context = new LibraryDBContext())
                        {
                            context.Books.Add(book);
                            context.SaveChanges();
                        }

                        // обновляем значение BookId в таблице
                        e.Row.Item = book;
                        BooksDataGrid.CurrentCell = new DataGridCellInfo(e.Row, BooksDataGrid.Columns[0]);
                        BooksDataGrid.BeginEdit();

                        // сбрасываем флаг isNewRow
                        isNewRow = false;
                    }
                    else
                    {
                        // если это существующая строка, то обновляем ее в базе данных
                        using (var context = new LibraryDBContext())
                        {
                            context.Entry(book).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                }
                else
                {
                    // Если пользователь отказался от сохранения изменений, восстанавливаем предыдущее значение
                    e.Row.Item = oldValue;
                    LoadData();
                }
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
                // если редактирование ячейки отменено, то восстанавливаем ее предыдущее значение
                e.Row.Item = oldValue;
            }
        }

        private void BooksDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // проверяем, является ли текущая строка новой
            isNewRow = (e.Row.Item == null);
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
                    else
                    {
                        LoadData();
                    }
                }
            }
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {

            using (var context = new LibraryDBContext())
            {
                var newBook = new Book
                {
                    Name = "New Book",
                    Isbn = "123-456-7890",
                    NumberOfPages = 300,
                    PublishingHouseId = 1, // Убедитесь, что этот внешний ключ соответствует существующей записи в таблице "PublishingHouse"
                    RackId = 1 // Убедитесь, что этот внешний ключ соответствует существующей записи в таблице "Rack"
                };

                int maxId = context.Books.Max(r => r.BookId);
                newBook.BookId = maxId + 1;

                MessageBox.Show("Новая книга добавлена.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                context.Books.Add(newBook);
                context.SaveChanges();

            }

            // Обновляем данные в DataGrid
            LoadData();
        }

        private object oldAuthorValue;
        private bool isNewAuthorRow = false;

        private void AuthorsDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            oldAuthorValue = e.Row.Item;
        }

        private void AuthorsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var author = (Author)e.Row.Item;
                var columnName = e.Column.SortMemberPath;

                switch (columnName)
                {
                    case "AuthorId":
                        author.AuthorId = int.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                        break;
                    case "FirstName":
                        author.FirstName = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                    case "LastName":
                        author.LastName = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                    case "SurName":
                        author.SurName = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                    case "Group":
                        author.Group = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                        // Добавьте другие столбцы, которые вы хотите отслеживать
                }
                if (author.AuthorId <= 0 ||
                    string.IsNullOrWhiteSpace(author.FirstName) ||
                    string.IsNullOrWhiteSpace(author.LastName) ||
                    string.IsNullOrWhiteSpace(author.Group))
                {
                    MessageBox.Show("Все поля должны быть заполнены корректными данными!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Cancel = true;
                    return;
                }

                using (var context = new LibraryDBContext())
                {
                    context.Entry(author).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
                e.Row.Item = oldAuthorValue;
                LoadData();
            }
        }

        private void AuthorsDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedAuthor = AuthorsDataGrid.SelectedItem as Author;
                if (selectedAuthor != null)
                {
                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этого автора?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new LibraryDBContext())
                        {
                            context.Authors.Remove(selectedAuthor);
                            context.SaveChanges();
                            LoadData(); // Перезагрузка данных после удаления
                            MessageBox.Show("Автор удален");
                        }
                    }
                }

                else
                {
                    LoadData();
                }
            }
        }

        private void AddAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new LibraryDBContext())
            {
                var newAuthor = new Author
                {
                    FirstName = "New",
                    LastName = "Author",
                    SurName = "N/A",
                    Group = "Default"
                };

                int maxId = context.Authors.Max(r => r.AuthorId);
                newAuthor.AuthorId = maxId + 1;
                MessageBox.Show("Новый автор добавлен.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                context.Authors.Add(newAuthor);
                context.SaveChanges();
            }

            LoadData();
        }


        private object oldGenreValue;
        private bool isNewGenreRow = false;

        private void GenresDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            oldGenreValue = e.Row.Item;
        }

        private void GenresDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var genre = (Genre)e.Row.Item;
                var columnName = e.Column.SortMemberPath;

                switch (columnName)
                {
                    case "GenreId":
                        genre.GenreId = int.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                        break;
                    case "Name":
                        genre.Name = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                    case "GenreType":
                        genre.GenreType = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                        // Добавьте другие столбцы, которые вы хотите отслеживать
                }
                if (genre.GenreId <= 0 ||
                    string.IsNullOrWhiteSpace(genre.Name) ||
                    string.IsNullOrWhiteSpace(genre.GenreType))
                {
                    MessageBox.Show("Все поля должны быть заполнены корректными данными!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Cancel = true;
                    return;
                }

                using (var context = new LibraryDBContext())
                {
                    context.Entry(genre).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
                e.Row.Item = oldGenreValue;
                LoadData();
            }
        }

        private void GenresDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedGenre = GenresDataGrid.SelectedItem as Genre;
                if (selectedGenre != null)
                {
                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этот жанр?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new LibraryDBContext())
                        {
                            context.Genres.Remove(selectedGenre);
                            context.SaveChanges();
                            LoadData(); // Перезагрузка данных после удаления
                            MessageBox.Show("Жанр удален");
                        }
                    }
                }

                else
                {
                    LoadData();
                }
            }
        }

        private void AddGenreButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new LibraryDBContext())
            {
                var newGenre = new Genre
                {
                    Name = "New Genre",
                    GenreType = "Type"
                };

                int maxId = context.Genres.Max(r => r.GenreId);
                newGenre.GenreId = maxId + 1;

                MessageBox.Show("Новый жанр добавлен.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                context.Genres.Add(newGenre);
                context.SaveChanges();
            }

            LoadData();
        }


        private object oldReaderValue;
        private bool isNewReaderRow = false;

        private void ReadersDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            oldReaderValue = e.Row.Item;
        }

        private void ReadersDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var reader = (Reader)e.Row.Item;
                var columnName = e.Column.SortMemberPath;

                switch (columnName)
                {
                    case "ReaderId":
                        reader.ReaderId = int.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                        break;
                    case "FirstName":
                        reader.FirstName = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                    case "LastName":
                        reader.LastName = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                    case "SurName":
                        reader.SurName = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                    case "Email":
                        reader.Email = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                    case "Fine":
                        if (e.EditingElement is TextBox textBox && decimal.TryParse(textBox.Text, out decimal fine))
                        {
                            reader.Fine = fine;
                        }
                        break;
                        // Добавьте другие столбцы, которые вы хотите отслеживать
                }
                if(reader.ReaderId <= 0 ||
                    string.IsNullOrWhiteSpace(reader.FirstName) ||
                    string.IsNullOrWhiteSpace(reader.LastName) ||
                    reader.Fine <= 0)
                {
                    MessageBox.Show("Все поля должны быть заполнены корректными данными!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Cancel = true;
                    return;
                }

                using (var context = new LibraryDBContext())
                {
                    context.Entry(reader).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
                e.Row.Item = oldReaderValue;
                LoadData();
            }
        }

        private void ReadersDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedReader = ReadersDataGrid.SelectedItem as Reader;
                if (selectedReader != null)
                {
                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этого читателя?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new LibraryDBContext())
                        {
                            context.Readers.Remove(selectedReader);
                            context.SaveChanges();
                            LoadData(); // Перезагрузка данных после удаления
                            MessageBox.Show("Читатель удален");
                        }
                    }
                    else
                    {
                        LoadData();
                    }
                }
            }
        }

        private void AddReaderButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new LibraryDBContext())
            {
                var newReader = new Reader
                {
                    FirstName = "New",
                    LastName = "Reader",
                    SurName = "N/A",
                    Email = "example@example.com",
                    Fine = null
                };

                int maxId = context.Readers.Max(r => r.ReaderId);
                newReader.ReaderId = maxId + 1;

                MessageBox.Show("Новый читатель добавлен.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                context.Readers.Add(newReader);
                context.SaveChanges();

            }

            LoadData();
        }


        private object oldBookIssuanceValue;
        private bool isNewBookIssuanceRow = false;

        private void BookIssuancesDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            oldBookIssuanceValue = e.Row.Item;
        }

        private void BookIssuancesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var bookIssuance = (BookIssuance)e.Row.Item;
                var columnName = e.Column.SortMemberPath;

                switch (columnName)
                {
                    case "BookIssuanceId":
                        bookIssuance.BookIssuanceId = int.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                        break;
                    case "BookId":
                        bookIssuance.BookId = int.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                        break;
                    case "ReaderId":
                        bookIssuance.ReaderId = int.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                        break;
                    case "DateOfIssue":
                        bookIssuance.DateOfIssue = DateTime.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                        break;
                    case "DateOfReturn":
                        bookIssuance.DateOfReturn = DateTime.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                        break;
                    case "DateOfPlannedReturn":
                        bookIssuance.DateOfPlannedReturn = DateTime.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                        break;
                        // Добавьте другие столбцы, которые вы хотите отслеживать
                }
                if(bookIssuance.BookIssuanceId <= 0 ||
                    bookIssuance.BookIssuanceId <= 0 ||
                    bookIssuance.ReaderId <= 0)
                {
                    MessageBox.Show("Все поля должны быть заполнены корректными данными!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Cancel = true;
                    return;
                }

                using (var context = new LibraryDBContext())
                {
                    context.Entry(bookIssuance).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
                e.Row.Item = oldBookIssuanceValue;
                LoadData();
            }
        }

        private void BookIssuancesDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedBookIssuance = BookIssuancesDataGrid.SelectedItem as BookIssuance;
                if (selectedBookIssuance != null)
                {
                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить эту выдачу книги?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new LibraryDBContext())
                        {
                            context.BookIssuances.Remove(selectedBookIssuance);
                            context.SaveChanges();
                            LoadData(); // Перезагрузка данных после удаления
                            MessageBox.Show("Выдача книги удалена");
                        }
                    }
                    else
                    {
                        LoadData();
                    }
                }
            }
        }

        private void AddBookIssuanceButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new LibraryDBContext())
            {
                var newBookIssuance = new BookIssuance
                {
                    BookId = 1, // Пример значения
                    ReaderId = 1, // Пример значения
                    DateOfIssue = DateTime.Now,
                    DateOfPlannedReturn = DateTime.Now.AddDays(14) // Пример значения
                };

                int maxId = context.BookIssuances.Max(r => r.BookIssuanceId);
                newBookIssuance.BookId = maxId + 1;

                MessageBox.Show("Новая выдача книги добавлена.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                context.BookIssuances.Add(newBookIssuance);
                context.SaveChanges();
            }

            LoadData();
        }


        private object oldDepartmentValue;
        private bool isNewDepartmentRow = false;

        private void DepartmentsDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            oldDepartmentValue = e.Row.Item;
        }

        private void DepartmentsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var department = (DepartmentOfIssuance)e.Row.Item;
                var columnName = e.Column.SortMemberPath;
                string newValue = (e.EditingElement as TextBox)?.Text;

                if (!string.IsNullOrEmpty(newValue))
                {
                    switch (columnName)
                    {
                        case "DepartmentOfOssuanceId":
                            if (int.TryParse(newValue, out int departmentId))
                            {
                                department.DepartmentOfOssuanceId = departmentId;
                            }
                            else
                            {
                                MessageBox.Show("Некорректное значение для ID отдела.");
                                e.Cancel = true;
                                return;
                            }
                            break;
                        case "DepartmentNum":
                            department.DepartmentNum = e.EditingElement.GetValue(TextBox.TextProperty) as string; ;
                            break;
                            // Добавьте другие столбцы, которые вы хотите отслеживать
                    }
                    if(department.DepartmentOfOssuanceId <= 0 ||
                        string.IsNullOrWhiteSpace(department.DepartmentNum))
                    {
                        MessageBox.Show("Все поля должны быть заполнены корректными данными!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Cancel = true;
                        return;
                    }

                    using (var context = new LibraryDBContext())
                    {
                        context.Entry(department).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                else
                {
                    MessageBox.Show("Значение не может быть пустым.");
                    e.Cancel = true;
                }
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
                e.Row.Item = oldDepartmentValue;
                LoadData();
            }
        }


        private void DepartmentsDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedDepartment = DepartmentsDataGrid.SelectedItem as DepartmentOfIssuance;
                if (selectedDepartment != null)
                {
                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этот отдел?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new LibraryDBContext())
                        {
                            context.DepartmentOfIssuances.Remove(selectedDepartment);
                            context.SaveChanges();
                            LoadData(); // Перезагрузка данных после удаления
                            MessageBox.Show("Отдел удален");
                        }
                    }
                }
                else
                {
                    LoadData();
                }
            }
        }

        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new LibraryDBContext())
            {
                var newDepartment = new DepartmentOfIssuance
                {
                    DepartmentNum = "1" // Пример значения
                };

                int maxId = context.DepartmentOfIssuances.Max(r => r.DepartmentOfOssuanceId);
                newDepartment.DepartmentOfOssuanceId = maxId + 1;

                context.DepartmentOfIssuances.Add(newDepartment);
                context.SaveChanges();
            }

            LoadData();
        }


        private object oldPublishingHouseValue;
        private bool isNewPublishingHouseRow = false;

        private void PublishingHousesDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            oldPublishingHouseValue = e.Row.Item;
        }

        private void PublishingHousesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var publishingHouse = (PublishingHouse)e.Row.Item;
                var columnName = e.Column.SortMemberPath;
                string newValue = (e.EditingElement as TextBox)?.Text;

                switch (columnName)
                {
                    case "PublishingHouseId":
                        publishingHouse.PublishingHouseId = int.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                        break;
                    case "Name":
                        publishingHouse.Name = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                    case "Commercial":
                        publishingHouse.Commercial = Encoding.ASCII.GetBytes(newValue);
                        break;
                    case "Coverage":
                        publishingHouse.Coverage = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                }
                if(publishingHouse.PublishingHouseId <= 0 ||
                    string.IsNullOrWhiteSpace(publishingHouse.Name) ||
                    string.IsNullOrWhiteSpace(publishingHouse.Coverage))
                {
                    MessageBox.Show("Все поля должны быть заполнены корректными данными!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Cancel = true;
                    return;
                }

                using (var context = new LibraryDBContext())
                {
                    context.Entry(publishingHouse).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
                e.Row.Item = oldPublishingHouseValue;
                LoadData();
            }
        }

        private void PublishingHousesDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedPublishingHouse = PublishingHousesDataGrid.SelectedItem as PublishingHouse;
                if (selectedPublishingHouse != null)
                {
                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить это издательство?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new LibraryDBContext())
                        {
                            context.PublishingHouses.Remove(selectedPublishingHouse);
                            context.SaveChanges();
                            LoadData(); // Перезагрузка данных после удаления
                            MessageBox.Show("Издательство удалено");
                        }
                    }
                    else
                    {
                        LoadData();
                    }
                }
            }
        }

        private void AddPublishingHouseButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new LibraryDBContext())
            {
                var newPublishingHouse = new PublishingHouse
                {
                    Name = "New Publishing House",
                    Commercial = Encoding.ASCII.GetBytes("0"),
                    Coverage = "Local" // Пример значения
                };

                int maxId = context.PublishingHouses.Max(ph => ph.PublishingHouseId);
                newPublishingHouse.PublishingHouseId = maxId + 1;

                MessageBox.Show("Новое издательство добавлено.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                context.PublishingHouses.Add(newPublishingHouse);
                context.SaveChanges();
            }

            LoadData();
        }


        private object oldRackValue;
        private bool isNewRackRow = false;

        private void RacksDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            oldRackValue = e.Row.Item;
        }

        private void RacksDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var rack = (Rack)e.Row.Item;
                var columnName = e.Column.SortMemberPath;

                switch (columnName)
                {
                    case "RackId":
                        rack.RackId = int.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                        break;
                    case "RackNumber":
                        rack.RackNumber = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                        // Добавьте другие столбцы, которые вы хотите отслеживать
                }
                if(rack.RackId <= 0 ||
                   string.IsNullOrWhiteSpace(rack.RackNumber)) 
                {
                    {
                        MessageBox.Show("Все поля должны быть заполнены корректными данными!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Cancel = true;
                        return;
                    }
                }

                using (var context = new LibraryDBContext())
                {
                    context.Entry(rack).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
                e.Row.Item = oldRackValue;
                LoadData();
            }
        }

        private void RacksDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedRack = RacksDataGrid.SelectedItem as Rack;
                if (selectedRack != null)
                {
                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этот стеллаж?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new LibraryDBContext())
                        {
                            context.Racks.Remove(selectedRack);
                            context.SaveChanges();
                            LoadData(); // Перезагрузка данных после удаления
                            MessageBox.Show("Стеллаж удален");
                        }
                    }
                    else
                    {
                        LoadData();
                    }
                }
            }
        }

        private void AddRackButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new LibraryDBContext())
            {
                var newRack = new Rack
                {
                    RackNumber = "1", // Пример значения
                };

                int maxId = context.Racks.Max(r => r.RackId);
                newRack.RackId = maxId + 1;

                MessageBox.Show("Новый стеллаж добавлен.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                context.Racks.Add(newRack);
                context.SaveChanges();
            }

            LoadData();
        }


        private object oldLibrarianValue;
        private bool isNewLibrarianRow = false;

        private void LibrariansDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            oldLibrarianValue = e.Row.Item;
        }

        private void LibrariansDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var librarian = (Librarian)e.Row.Item;
                var columnName = e.Column.SortMemberPath;

                switch (columnName)
                {
                    case "LibrarianId":
                        librarian.LibrarianId = int.Parse(e.EditingElement.GetValue(TextBox.TextProperty) as string);
                        break;
                    case "FirstName":
                        librarian.FirstName = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                    case "LastName":
                        librarian.LastName = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                    case "SurName":
                        librarian.Surname = e.EditingElement.GetValue(TextBox.TextProperty) as string;
                        break;
                        // Добавьте другие столбцы, которые вы хотите отслеживать
                }
                if (librarian.LibrarianId <= 0 ||
                    string.IsNullOrWhiteSpace(librarian.FirstName) ||
                    string.IsNullOrWhiteSpace(librarian.LastName))

                using (var context = new LibraryDBContext())
                {
                    context.Entry(librarian).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
                e.Row.Item = oldLibrarianValue;
                LoadData();
            }
        }

        private void LibrariansDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedLibrarian = LibrariansDataGrid.SelectedItem as Librarian;
                if (selectedLibrarian != null)
                {
                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этого библиотекаря?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new LibraryDBContext())
                        {
                            context.Librarians.Remove(selectedLibrarian);
                            context.SaveChanges();
                            LoadData(); // Перезагрузка данных после удаления
                            MessageBox.Show("Библиотекарь удален");
                        }
                    }
                    else
                    {
                        LoadData();
                    }
                }
            }
        }

        private void AddLibrarianButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new LibraryDBContext())
            {
                var newLibrarian = new Librarian
                {
                    FirstName = "New",
                    LastName = "Librarian",
                    Surname = "N/A"
                };

                int maxId = context.Librarians.Max(r => r.LibrarianId);
                newLibrarian.LibrarianId = maxId + 1;

                MessageBox.Show("Новый библиотекарь добавлен.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                context.Librarians.Add(newLibrarian);
                context.SaveChanges();
            }

            LoadData();
        }


        private object oldHallEmployeeValue;
        private bool isNewHallEmployeeRow = false;

        private void HallEmployeesDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            oldHallEmployeeValue = e.Row.Item;
        }


        private void HallEmployeesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var hallEmployee = (HallEmployee)e.Row.Item;
                var columnName = e.Column.SortMemberPath;

                switch (columnName)
                {
                    case "HallEmployeeId":
                        hallEmployee.HallEmployeeId = int.Parse((e.EditingElement as TextBox)?.Text);
                        break;
                    case "FirstName":
                        hallEmployee.FirstName = (e.EditingElement as TextBox)?.Text;
                        break;
                    case "LastName":
                        hallEmployee.LastName = (e.EditingElement as TextBox)?.Text;
                        break;
                    case "SurName":
                        hallEmployee.SurName = (e.EditingElement as TextBox)?.Text;
                        break;
                    case "HallNumber":
                        hallEmployee.HallId = int.Parse((e.EditingElement as TextBox)?.Text);
                        break;
                        // Добавьте другие столбцы, которые вы хотите отслеживать
                }

                if (hallEmployee.HallEmployeeId <= 0 ||
                    string.IsNullOrEmpty(hallEmployee.FirstName) ||
                    string.IsNullOrEmpty(hallEmployee.LastName) ||
                    hallEmployee.HallId < 0)
                {
                    MessageBox.Show("Все поля должны быть заполнены корректными данными!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Cancel = true;
                    return;
                }

                using (var context = new LibraryDBContext())
                {
                    context.Entry(hallEmployee).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
                e.Row.Item = oldHallEmployeeValue;
                LoadData();
            }
        }


        private void HallEmployeesDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedHallEmployee = HallEmployeesDataGrid.SelectedItem as HallEmployee;
                if (selectedHallEmployee != null)
                {
                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этого сотрудника зала?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new LibraryDBContext())
                        {
                            context.HallEmployees.Remove(selectedHallEmployee);
                            context.SaveChanges();
                            LoadData(); // Перезагрузка данных после удаления
                            MessageBox.Show("Сотрудник зала удален");
                        }
                    }
                    else
                    {
                        LoadData();
                    }
                }
            }
        }

        private void AddHallEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new LibraryDBContext())
            {
                var newHallEmployee = new HallEmployee
                {
                    FirstName = "New",
                    LastName = "Employee",
                    SurName = "N/A",
                    HallId = 1 // Пример значения
                };

                int maxId = context.HallEmployees.Max(r => r.HallEmployeeId);
                newHallEmployee.HallEmployeeId = maxId + 1;

                MessageBox.Show("Новый сотрудник добавлен.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                context.HallEmployees.Add(newHallEmployee);
                context.SaveChanges();
            }

            LoadData();
        }


    }
}