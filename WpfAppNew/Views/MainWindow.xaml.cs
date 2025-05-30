using MahApps.Metro.Controls;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Core;
using ReservationSystem.Data;
using System;
using System.Linq;
using System.Windows;

namespace WpfAppNew
{
    public partial class MainWindow : MetroWindow
    {
        private readonly ReservationDbContext _context;
        static public IUserService _userService = new UserService(new ReservationDbContext());
        static public RoomService _roomService = new RoomService(new ReservationDbContext(), _userService);
        private User _currentUser;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;

            // Inicjalizacja bazy danych z migracjami
            using (var dbContext = new ReservationDbContext())
            {
                dbContext.Database.Migrate();

                // Dodawanie początkowych danych jeśli baza była pusta
                if (!dbContext.Users.Any())
                {
                    dbContext.Users.Add(new User("admin", "admin", UserRole.Admin));
                    dbContext.Rooms.Add(new Room("Aula 1", 100));
                    dbContext.SaveChanges();
                }
            }

            // Inicjalizacja serwisów
            _context = new ReservationDbContext(new DbContextOptionsBuilder<ReservationDbContext>().Options);

            // Sprawdź czy użytkownik jest zalogowany
            _currentUser = null;
        }

        private void AddReservationBTN_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Musisz być zalogowany, aby zarządzać salami",
                              "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Dla administratora - otwórz okno dodawania sali
            if (_currentUser.Role == UserRole.Prowadzacy)
            {
                var addReservationWindow = new AddReservationWindow(_currentUser, _roomService, _userService)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    ResizeMode = ResizeMode.NoResize
                };

                if (addReservationWindow.ShowDialog() == true)
                {
                    MessageBox.Show("Rezerwacja została pomyślnie dodana", "Sukces",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                // Dla innych ról - pokaż dostępne sale
                var startDate = DateTime.Today;
                var endDate = DateTime.Today.AddDays(1);

                var availableRooms = _roomService.GetAvailableRooms(startDate, endDate).ToList();

                var message = availableRooms.Any()
                    ? string.Join(Environment.NewLine, availableRooms.Select(r => $"Sala: {r.Name}"))
                    : "Brak dostępnych sal w podanym terminie.";

                MessageBox.Show(message, "Dostępne sale");
            }
        }

        private void AddRoomsBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentUser == null)
                {
                    MessageBox.Show("Musisz być zalogowany, aby zarządzać salami",
                                  "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Dla administratora - otwórz okno dodawania sali
                if (_currentUser.Role == UserRole.Admin)
                {
                    var addRoomWindow = new AddRoomWindow(_roomService)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        ResizeMode = ResizeMode.NoResize
                    };

                    if (addRoomWindow.ShowDialog() == true)
                    {
                        MessageBox.Show("Sala została pomyślnie dodana", "Sukces",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    // Dla innych ról - pokaż dostępne sale
                    var startDate = DateTime.Today;
                    var endDate = DateTime.Today.AddDays(1);

                    var availableRooms = _roomService.GetAvailableRooms(startDate, endDate).ToList();

                    var message = availableRooms.Any()
                        ? string.Join(Environment.NewLine, availableRooms.Select(r => $"Sala: {r.Name}"))
                        : "Brak dostępnych sal w podanym terminie.";

                    MessageBox.Show(message, "Dostępne sale");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    

        private void loginBTN_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow(_userService, _roomService)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
            };

            if (loginWindow.ShowDialog() == true)
            {
                _currentUser = loginWindow.AuthenticatedUser;
                MessageBox.Show($"Zalogowano jako: {_currentUser.Login} ({_currentUser.Role})",
                    "Zalogowano", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ShowOwnReservationsBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentUser == null)
                {
                    MessageBox.Show("Musisz być zalogowany, aby zobaczyć swoje rezerwacje",
                                  "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var reservationsWindow = new ReservationsWindow(_roomService, _currentUser)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    ResizeMode = ResizeMode.NoResize
                };
                reservationsWindow.Show();
                //this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}