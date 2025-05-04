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