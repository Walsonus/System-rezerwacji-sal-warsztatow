using MahApps.Metro.Controls;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Core;
using ReservationSystem.Data;
using System;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfAppNew;

public partial class MainWindow : MetroWindow
{
    //private readonly RoomService roomService;
    static public DbContextOptions<ReservationDbContext> options = new DbContextOptionsBuilder<ReservationDbContext>().Options;
    static public ReservationDbContext context = new ReservationDbContext(options);
    static public RoomService roomService = new RoomService(context);
    private object roomList;

    //public object Options { get => options; set => options = value; }

    public MainWindow(IRoomService roomService)
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ResizeMode = ResizeMode.NoResize;
        //_roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
        
    }


    //adding reservation
    private void AddReservationBTN_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var existingRoom = roomService.GetRoomById(1);

            if (existingRoom == null)
            {
                var newRoom = new Room
                {
                    Name = "SALA KONFERENCYJNA A",
                    Capacity = 20
                };
                roomService.AddRoom(newRoom);
                existingRoom = newRoom;
                MessageBox.Show("Pokój został pomyślnie dodany");
            }

            // Teraz tworzymy rezerwację dla istniejącego pokoju
            var reservation = new Reservation
            {
                Room = existingRoom,
                RoomId = existingRoom.Id,
                StartDate = new DateTime(2025, 6, 12),
                EndDate = new DateTime(2025, 6, 13),
                UserId = 1
            };

            roomService.AddReservation(reservation);
            MessageBox.Show("Rezerwacja została pomyślnie dodana");
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

    //button for showing available rooms
    private void ShowAvailableRoomsBTN_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(1);

            var availableRooms = roomService.GetAvaiableRooms(startDate, endDate).ToList();

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


    //button for login
    private void loginBTN_Click(object sender, RoutedEventArgs e)
    {
        var userService = new UserService(context);
        var roomService = new RoomService(context);

        var loginWindow = new LoginWindow(userService, roomService)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
        };
        loginWindow.Show();
        this.Close();
    }


    //button for showing own reservations
    private void ShowOwnReservationsBTN_Click(object sender, RoutedEventArgs e)
    {
        var reservationsWindow = new ReservationsWindow(roomService)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
        };
        reservationsWindow.Show();
        this.Close();
    }

}