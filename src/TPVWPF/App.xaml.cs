using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TPVWPF.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using TPVWPF.ViewModels;
using System.Globalization;
using TPVWPF.Properties;
using TPVWPF.Services;
using Serilog;
using TPVWPF.Views;

namespace TPVWPF
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<LocalizationService>();

            _serviceProvider = services.BuildServiceProvider();


            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            // Configuración de Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    path: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app.txt"),
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .Enrich.FromLogContext()
                .CreateLogger();

            DispatcherUnhandledException += (s, e) =>
            {
                Log.Fatal(e.Exception, "Excepción no controlada en la aplicación.");
                e.Handled = true;
            };

            Exit += (s, e) => Log.CloseAndFlush();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var localizationService = _serviceProvider.GetRequiredService<LocalizationService>();
            this.Resources["LocalizationService"] = localizationService;

            string preferredLanguage = Settings.Default.PreferredLanguage;
            if (!string.IsNullOrEmpty(preferredLanguage))
            {
                try
                {
                    localizationService.CurrentCulture = new CultureInfo(preferredLanguage);

                }
                catch (CultureNotFoundException)
                {
                    localizationService.CurrentCulture = new CultureInfo("es-ES");

                }
            }
            else
            {
                localizationService.CurrentCulture = new CultureInfo("es-ES");

            }

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>(); // ???????
            mainWindow.Show();
        }
    }
}