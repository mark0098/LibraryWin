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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace LibraryWin
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private LibraryDBContext _context;
        public Login()
        {
            InitializeComponent();
            LibraryDBContext DBContext = new();
            _context = DBContext;
        }


        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailTextBox.Text;
            var password = PasswordBox.Password;

            var user = await _context.Readers.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                if (user.Password == password)
                {
                    MessageBox.Show("Вы успешно вошли в систему.");
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Неверный пароль.");
                }
            }
            else
            {
                var librarian = await _context.Librarians.FirstOrDefaultAsync(l => l.Email == email && l.Password == password);
                if (librarian != null)
                {
                    MessageBox.Show("Здравствуйте, библиотекарь.");
                    AdminWindow adminWindow = new AdminWindow();
                    adminWindow.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Пользователь с таким адресом электронной почты не найден.");
                }
            }
        }


    }
}
