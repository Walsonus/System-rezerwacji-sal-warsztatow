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
        private readonly User _currentUser;

        public ReservationsWindow(IRoomService roomService, User currentUser)
        {
            InitializeComponent();
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;

            Loaded += (s, e) => LoadReservations(); // Załaduj rezerwacje PO otwarciu okna
        }

        private void LoadReservations()
        {
            try
            {
                if (_currentUser == null)
                {
                    MessageBox.Show("Nie jesteś zalogowany", "Błąd",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                var reservations = _roomService.GetUserReservations(_currentUser.UserId);
                if (reservations == null || !reservations.Any())
                {
                    MessageBox.Show("Brak rezerwacji do wyświetlenia", "Informacja",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    //return;
                }

                reservationsList.ItemsSource = reservations
                    .OrderBy(r => r.StartDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania rezerwacji: {ex.Message}", "Błąd",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
        private void backBTN_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}