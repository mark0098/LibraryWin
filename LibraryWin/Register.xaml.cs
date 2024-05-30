using LibraryWin.Models;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace LibraryWin
{
    public partial class Register : Window
    {
        public event EventHandler<RegistrationEventArgs> RegistrationCompleted;

        public Register()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            string surName = SurNameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (ValidateInputs(firstName, lastName, surName, email, password, confirmPassword))
            {
                try
                {
                    using (var context = new LibraryDBContext())
                    {
                        var newReader = new Reader
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            SurName = surName,
                            Email = email,
                            Password = password,
                            Fine = decimal.Zero
                        };

                        newReader.ReaderId = context.Readers.Max(r => r.ReaderId) + 1;

                        context.Readers.Add(newReader);
                        context.SaveChanges();
                    }

                    MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    RegistrationCompleted?.Invoke(this, new RegistrationEventArgs { Email = email, Password = password });

                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private bool ValidateInputs(string firstName, string lastName, string surName, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Все поля (кроме отчества) должны быть заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Введите корректный email!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (password.Length < 8 ||
                !Regex.IsMatch(password, @"[A-Z]") ||
                !Regex.IsMatch(password, @"[a-z]") ||
                !Regex.IsMatch(password, @"[0-9]") ||
                !Regex.IsMatch(password, @"[\W_]") ||
                Regex.IsMatch(password, @"(.)\1{2,}"))
            {
                MessageBox.Show("Пароль должен содержать минимум 8 символов, включать буквы, заглавные буквы, цифры и специальные символы. Каждый символ не должен повторяться более двух раз подряд.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }

    public class RegistrationEventArgs : EventArgs
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
