using System;
using System.Linq;
using System.Windows;
using ReservationSystem.Core;

namespace WpfAppNew
{
    public partial class AddReservationWindow : Window
    {
        private readonly User _currentUser;
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;

        // Original constructor - keep this and fix the calling code
        public AddReservationWindow(User currentUser, IRoomService roomService, IUserService userService)
        {
            InitializeComponent();

            _currentUser = currentUser;
            _roomService = roomService;
            _userService = userService;

            LoadAvailableRooms();
            SetDefaultDates();
        }

        private void LoadAvailableRooms()
        {
            try
            {
                var availableRooms = _roomService.GetAllRooms().ToList();
                RoomComboBox.ItemsSource = availableRooms;

                if (availableRooms.Any())
                {
                    RoomComboBox.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Brak dostępnych sal. Najpierw administrator musi dodać sale.", "Błąd",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas ładowania sal: {ex.Message}", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void SetDefaultDates()
        {
            StartDatePicker.SelectedDate = DateTime.Today;
            EndDatePicker.SelectedDate = DateTime.Today.AddDays(1);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                var selectedRoom = (Room)RoomComboBox.SelectedItem;
                var startDate = StartDatePicker.SelectedDate.Value;
                var endDate = EndDatePicker.SelectedDate.Value;

                // Sprawdź dostępność sali w podanym terminie
                bool isRoomAvailable = _roomService.IsRoomAvailable(selectedRoom.Id, startDate, endDate);

                if (!isRoomAvailable)
                {
                    MessageBox.Show("Sala jest już zarezerwowana w podanym terminie.", "Błąd rezerwacji",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var reservation = new Reservation
                {
                    Room = selectedRoom,
                    RoomId = selectedRoom.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    UserId = _currentUser.UserId
                };

                _roomService.AddReservation(reservation, _currentUser);

                MessageBox.Show("Rezerwacja została pomyślnie dodana", "Sukces",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
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

        private bool ValidateInputs()
        {
            if (RoomComboBox.SelectedItem == null)
            {
                MessageBox.Show("Proszę wybrać salę", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Proszę wybrać datę rozpoczęcia i zakończenia", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (StartDatePicker.SelectedDate.Value > EndDatePicker.SelectedDate.Value)
            {
                MessageBox.Show("Data zakończenia nie może być wcześniejsza niż data rozpoczęcia", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (StartDatePicker.SelectedDate.Value < DateTime.Today)
            {
                MessageBox.Show("Data rozpoczęcia nie może być w przeszłości", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}