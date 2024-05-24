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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LibraryWin
{
    public partial class AddEditBookDialog : Window
    {
        public Book Book { get; private set; }

        public AddEditBookDialog(Book book = null)
        {
            InitializeComponent();
            /*
            if (book != null)
            {
                Book = book;
                TitleTextBox.Text = book.Name;
                PublishingTextBox.Text = book.PublishingHouse.Name;
                GenreTextBox.Text = book.Genre;
                PublishingHouseTextBox.Text = book.PublishingHouse;
                RackTextBox.Text = book.Rack;
            }
            else
            {
                Book = new Book();
            }
            */
        }

        /*
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Book.Title = TitleTextBox.Text;
            Book.Author = AuthorTextBox.Text;
            Book.Genre = GenreTextBox.Text;
            Book.PublishingHouse = PublishingHouseTextBox.Text;
            Book.Rack = RackTextBox.Text;

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        */
    }
}