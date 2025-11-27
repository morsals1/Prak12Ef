using EfCore.Service;
using EfCore.data;
using System;
using System.Windows;
using System.Windows.Controls;
using EfCore.Validators;
using System.Text.RegularExpressions;

namespace EfCore.Pages
{
    public partial class UserChangePage : Page
    {
        private UserService _service = new UserService();
        public User CurrentUser { get; set; } = new User();
        public bool IsEdit { get; set; } = false;

        public UserChangePage(User? editUser = null)
        {
            InitializeComponent();
            DataContext = this;
            if (editUser != null)
            {
                CurrentUser = editUser;
                IsEdit = true;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(LoginTextBox) ||
                Validation.GetHasError(NameTextBox) ||
                Validation.GetHasError(EmailTextBox) ||
                Validation.GetHasError(PasswordBox))
            {
                MessageBox.Show("Исправьте ошибки в форме", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateOnly date = DateOnly.Parse(CurrentUser.CreatedAt);
            if (date < DateOnly.FromDateTime(DateTime.Now))
            {
                MessageBox.Show("Дата создания не может быть ранее текущей даты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsEdit)
            {
                if (_service.Users.Any(u => u.Login.ToLower() == CurrentUser.Login.ToLower()))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_service.Users.Any(u => u.Email.ToLower() == CurrentUser.Email.ToLower()))
                {
                    MessageBox.Show("Пользователь с таким email уже существует", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (IsEdit)
            {
                _service.Update(CurrentUser);
                MessageBox.Show("Данные пользователя обновлены", "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _service.Add(CurrentUser);
                MessageBox.Show("Пользователь добавлен", "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            NavigationService.GoBack();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;
            CurrentUser.Password = password;
            PasswordErrorText.Text = "";

            if (string.IsNullOrEmpty(password))
            {
                PasswordErrorText.Text = "Пароль обязателен для заполнения";
            }
            else if (password.Length < 8)
            {
                PasswordErrorText.Text = "Пароль должен содержать минимум 8 символов";
            }
            else if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"))
            {
                PasswordErrorText.Text = "Пароль должен содержать буквы верхнего и нижнего регистра и цифры";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}