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
                MessageBox.Show("Musisz być zalogowany, aby dokonać rezerwacji", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (!_userService.HasAccess(_currentUser, "Prowadzacy"))
                {
                    MessageBox.Show("Tylko prowadzący mogą dokonywać rezerwacji", "Brak uprawnień",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var existingRoom = _roomService.GetRoomById(1);

                if (existingRoom == null)
                {
                    MessageBox.Show("Najpierw administrator musi dodać sale", "Błąd",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var reservation = new Reservation
                {
                    Room = existingRoom,
                    RoomId = existingRoom.Id,
                    StartDate = new DateTime(2025, 6, 12),
                    EndDate = new DateTime(2025, 6, 13),
                    UserId = _currentUser.UserId
                };

                _roomService.AddReservation(reservation, _currentUser);
                MessageBox.Show("Rezerwacja została pomyślnie dodana", "Sukces",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += "\nInner Exception: " + ex.InnerException.Message;
                }
                MessageBox.Show(errorMessage, "Błąd zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowAvailableRoomsBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var startDate = DateTime.Today;
                var endDate = DateTime.Today.AddDays(1);

                var availableRooms = _roomService.GetAvaiableRooms(startDate, endDate).ToList();

                var message = availableRooms.Any()
                    ? string.Join(Environment.NewLine, availableRooms.Select(r => $"Sala: {r.Name}"))
                    : "Brak dostępnych sal w podanym terminie.";

                MessageBox.Show(message, "Dostępne sale");
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