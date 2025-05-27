using MahApps.Metro.Controls;
using ReservationSystem.Core;
using ReservationSystem.Data;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfAppNew
{
    public partial class LoginWindow : MetroWindow
    {
        private readonly IUserService _userService;
        private readonly IRoomService _roomService;

        public User AuthenticatedUser { get; private set; }

        public LoginWindow(IUserService userService, IRoomService roomService)
        {
            InitializeComponent();
            _userService = userService;
            _roomService = roomService;
            Loaded += (s, e) => UsernameTextBox.Focus();
        }

        private void loginBTN_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Proszę wprowadzić nazwę użytkownika i hasło!");
                return;
            }

            try
            {
                AuthenticatedUser = _userService.Authenticate(username, password);
                if (AuthenticatedUser == null)
                {
                    ShowError("Nieprawidłowa nazwa użytkownika lub hasło!");
                    return;
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ShowError($"Błąd podczas logowania: {ex.Message}");
            }
        }

        private void registrationBTN_Click(object sender, RoutedEventArgs e)
        {
            var registrationWindow = new RegistrationWindow(_userService)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            registrationWindow.ShowDialog();
        }

        private void backBTN_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string message)
        {
            MessageBox.Show(this, message, "Błąd",
                MessageBoxButton.OK, MessageBoxImage.Error);
            PasswordBox.Clear();
            PasswordBox.Focus();
        }
    }
}