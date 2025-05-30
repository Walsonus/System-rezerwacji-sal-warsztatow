using MahApps.Metro.Controls;
using ReservationSystem.Core;
using ReservationSystem.Data;
using System.Windows;

namespace WpfAppNew
{
    public partial class AddRoomWindow : MetroWindow
    {
        private readonly RoomService _roomService;

        public AddRoomWindow(RoomService roomService)
        {
            InitializeComponent();
            _roomService = roomService;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var room = new Room
                {
                    Name = NameTextBox.Text,
                    Capacity = int.Parse(CapacityTextBox.Text),
                };

                _roomService.AddRoom(room);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas dodawania sali: {ex.Message}",
                              "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}