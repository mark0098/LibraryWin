using System.Linq;
using System.Windows;
using System.Windows.Input;
using LibraryWin.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryWin
{
    public partial class Login : Window
    {
        private LibraryDBContext _context;

        public Login()
        {
            InitializeComponent();
            _context = new LibraryDBContext();
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
                    mainWindow.CurrentReaderEmail = email;      
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


        private void RegisterTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Register registerWindow = new Register();
            registerWindow.RegistrationCompleted += RegisterWindow_RegistrationCompleted;
            registerWindow.ShowDialog();
        }

        private void RegisterWindow_RegistrationCompleted(object sender, RegistrationEventArgs e)
        {
            UpdateContext();

            Reader newReader = _context.Readers.FirstOrDefault(u => u.Email == e.Email);
            ProfileWindow profileWindow = new ProfileWindow(newReader);
            profileWindow.ShowDialog();
        }


        private void UpdateContext()
        {
            _context.ChangeTracker.Clear();    
            _context.Dispose();       
            _context = new LibraryDBContext();      
        }

    }
}
