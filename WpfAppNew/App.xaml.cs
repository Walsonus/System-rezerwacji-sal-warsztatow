// App.xaml.cs w WpfAppNew
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReservationSystem.Core;
using ReservationSystem.Data;
using System;
using System.Windows;

namespace WpfAppNew
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            // Globalna obsługa nieprzechwyconych wyjątków
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Exception ex = (Exception)args.ExceptionObject;
                MessageBox.Show($"Krytyczny błąd: {ex.Message}", "Błąd aplikacji",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            };

            DispatcherUnhandledException += (sender, args) =>
            {
                MessageBox.Show($"Błąd interfejsu: {args.Exception.Message}", "Błąd",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Konfiguracja DbContext
            services.AddDbContext<ReservationDbContext>(options =>
                options.UseSqlServer("Twój_connection_string"));

            // Rejestracja serwisów
            services.AddTransient<IRoomService, RoomService>();
            services.AddSingleton<MainWindow>();
        }
    }
}