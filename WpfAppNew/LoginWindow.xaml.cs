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
using MahApps.Metro.Controls;
using ReservationSystem.Core;
using ReservationSystem.Data;

namespace WpfAppNew
{
    public partial class LoginWindow : MetroWindow
    {
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;

        public LoginWindow(IUserService userService, IRoomService roomService)
        {
            _userService = userService;
            _roomService = roomService;
            InitializeComponent();

            //focus on username textbox
            Loaded += (s, e) => UsernameTextBox.Focus();
        }

        private void loginBTN_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            //validate entry fields
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowErrorMessage("Proszę wprowadzić nazwę użytkownika i hasło!");
                return;
            }

            try
            {
                var user = _userService.Authenticate(username, password);
                if (user == null)
                {
                    ShowErrorMessage("Nieprawidłowa nazwa użytkownika lub hasło!");
                    return;
                }
                OpenDashboardWindow(user);
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Błąd podczas logowania: {ex.Message}");
            }
        }
        private void registrationBTN_Click(object sender, RoutedEventArgs e)
        {
            // Przejście do okna rejestracji
            var registrationWindow = new RegistrationWindow(_userService)
            {
                Owner = this, // Ustawienie właściciela dla poprawnego centrowania
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize
            };

            // Otwarcie okna modalnego
            registrationWindow.ShowDialog();

            // Po zamknięciu okna rejestracji możesz np. przenieść focus na pole loginu
            UsernameTextBox.Focus();
        }

        private void backBTN_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow(_roomService)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
            };
            mainWindow.Show();
            this.Close();
        }

        private void OpenDashboardWindow(User user)
        {
            MetroWindow dashboardWindow = user.Role switch
            {
                UserRole.Admin => new AdminDashboardWindow(_userService, _roomService),
                UserRole.Prowadzacy => new TeacherDashboardWindow(_userService, _roomService),
                UserRole.Student => new StudentDashboardWindow(_userService, _roomService)
            };

            dashboardWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dashboardWindow.Show();
            this.Close();
        }

        //error message
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(this, message, "Błąd logowania",
                MessageBoxButton.OK, MessageBoxImage.Error);
            PasswordBox.Clear();
            PasswordBox.Focus();
        }


    }

}
