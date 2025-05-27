using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using ReservationSystem.Core;
using ReservationSystem.Data;

namespace WpfAppNew
{
    public partial class RegistrationWindow : MetroWindow
    {
        private readonly IUserService _userService;

        public RegistrationWindow(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            InitializeComponent();
            LoadRoles();
            Loaded += RegistrationWindow_Loaded;

            RegisterButton.Click += RegisterButton_Click;
            backBTN.Click += backBTN_Click;
            PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
            ConfirmPasswordBox.PasswordChanged += ConfirmPasswordBox_PasswordChanged;
        }

        private void RegistrationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRoles();
            UsernameTextBox?.Focus();
        }

        private void LoadRoles()
        {
            try
            {
                var roles = new Dictionary<UserRole, string>
                {
                    { UserRole.Student, "Student" },
                    { UserRole.Prowadzacy, "Prowadzący" }
                };

                RoleComboBox.ItemsSource = roles;
                RoleComboBox.DisplayMemberPath = "Value";
                RoleComboBox.SelectedValuePath = "Key";
                RoleComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania ról: {ex.Message}", "Błąd",
                             MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Pobierz wybraną rolę
                UserRole selectedRole = (UserRole)RoleComboBox.SelectedValue;

                // Zarejestruj użytkownika z wybraną rolą
                var user = _userService.Register(UsernameTextBox.Text, PasswordBox.Password, selectedRole);

                MessageBox.Show("Rejestracja zakończona pomyślnie!", "Sukces",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (ArgumentException ex)
            {
                ErrorTextBlock.Text = ex.Message;
                PasswordBox.Focus();
            }
            catch (InvalidOperationException ex)
            {
                ErrorTextBlock.Text = ex.Message;
                UsernameTextBox.Focus();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show($"Błąd systemu: {ex.Message}", "Błąd",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            int length = passwordBox.Password.Length;

            if (length > 0 && length < 8)
            {
                ErrorTextBlock.Text = $"Wprowadź co najmniej 8 znaków ({length}/8)";
            }
            else if (passwordBox == PasswordBox && !string.IsNullOrEmpty(ConfirmPasswordBox.Password))
            {
                if (PasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    ErrorTextBlock.Text = "Hasła nie są identyczne";
                }
                else
                {
                    ErrorTextBlock.Text = string.Empty;
                }
            }
            else
            {
                ErrorTextBlock.Text = string.Empty;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PasswordBox.Password))
            {
                if (PasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    ErrorTextBlock.Text = "Hasła nie są identyczne";
                }
                else
                {
                    ErrorTextBlock.Text = string.Empty;
                }
            }
        }

        private void backBTN_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow(_userService, new RoomService(new ReservationDbContext()))
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            loginWindow.Show();
            this.Close();
        }
    }
}