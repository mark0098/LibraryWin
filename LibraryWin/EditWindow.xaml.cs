using LibraryWin.Models;
using Microsoft.EntityFrameworkCore;
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

namespace LibraryWin
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private AllBooksInfo _book;
        public EditWindow(AllBooksInfo book)
        {
            InitializeComponent();
            _book = book;
            NameTextBox.Text = _book.Имя;
            AuthorTextBox.Text = _book.Автор;
            AuthorGroupTextBox.Text = _book.ГрупповаяПринадлежностьАвтора;
            GenreTextBox.Text = _book.Название;
            GenreGroupTextBox.Text = _book.ЖанроваяГруппа;
            CountryTextBox.Text = _book.Страна;
            PublisherTextBox.Text = _book.Издательство;
            IsbnTextBox.Text = _book.Isbn;
            PageCountTextBox.Text = _book.КолВоСтраниц.ToString();
            HallNumberTextBox.Text = _book.НомерЗала;
            RackNumberTextBox.Text = _book.НомерСтеллажа;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _book.Имя = NameTextBox.Text;
            _book.Автор = AuthorTextBox.Text;
            _book.ГрупповаяПринадлежностьАвтора = AuthorGroupTextBox.Text;
            _book.Название = GenreTextBox.Text;
            _book.ЖанроваяГруппа = GenreGroupTextBox.Text;
            _book.Страна = CountryTextBox.Text;
            _book.Издательство = PublisherTextBox.Text;
            _book.Isbn = IsbnTextBox.Text;
            _book.КолВоСтраниц = int.Parse(PageCountTextBox.Text);
            _book.НомерЗала = HallNumberTextBox.Text;
            _book.НомерСтеллажа = RackNumberTextBox.Text;
            using (var context = new LibraryDBContext())
            {
                context.Entry(_book).State = EntityState.Modified;
                context.SaveChanges();
            }
            DialogResult = true;
            Close();
        }
    }
}
