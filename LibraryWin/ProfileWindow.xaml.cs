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

namespace LibraryWin
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        public Reader CurrentReader { get; set; }

        public ProfileWindow(Reader reader)
        {
            InitializeComponent();
            CurrentReader = reader;
            LastNameTextBlock.Text = CurrentReader.LastName;
            FirstNameTextBlock.Text = CurrentReader.FirstName;
            SurNameTextBlock.Text = CurrentReader.SurName;
            EmailTextObjectBinding.Text = CurrentReader.Email;
            FineTextBlock.Text = CurrentReader.Fine.Value.ToString("C2");
        }
    }
}
