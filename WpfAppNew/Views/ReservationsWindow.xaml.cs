using MahApps.Metro.Controls;
using ReservationSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfAppNew
{
    public partial class ReservationsWindow : MetroWindow
    {
        private readonly IRoomService _roomService;

        public ReservationsWindow(IRoomService roomService)
        {
            InitializeComponent();
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;

            LoadReservations();
        }

        private void LoadReservations()
        {
            try
            {
                var rooms = _roomService.GetAllRooms();
                var allReservations = new List<Reservation>();

                foreach (var room in rooms)
                {
                    if (room.Reservations != null && room.Reservations.Any())
                    {
                        allReservations.AddRange(room.Reservations);
                    }
                }

                reservationsList.ItemsSource = allReservations.OrderBy(r => r.StartDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania rezerwacji: {ex.Message}",
                              "Błąd",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void loginBTN_Click(object sender, RoutedEventArgs e)
        {
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
    }
}