using MahApps.Metro.Controls;
using ReservationSystem.Core;
using System;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfAppNew
{
    public partial class MainWindow : MetroWindow
    {
        private readonly IRoomService _roomService;

        public MainWindow(IRoomService roomService)
        {
            InitializeComponent();
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
        }

        private void AddReservationBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //choosing by user from form
                int roomId = 1;
                int userId = 1;
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now.AddHours(2);

                //check if available
                if (!IsRoomAvailable(roomId, startDate, endDate))
                {
                    MessageBox.Show("Wybrana sala jest już zajęta w podanym terminie!",
                        "ErrorReservation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
                var reservation = new Reservation
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Room = new Room { Id = roomId },
                    UserId = userId
                };

                //add a reservation
                _roomService.AddReservation(reservation);

                //inform about successful reservation
                MessageBox.Show("Rezerwacja została dodana pomyślnie!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}\n\nSzczegóły:\n{ex.InnerException?.Message}", "Error",
               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsRoomAvailable(int roomId, DateTime start, DateTime end)
        {
            var availableRooms = _roomService.GetAvaiableRooms(start, end);
            return availableRooms.Any(r => r.Id == roomId);
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
    }
}