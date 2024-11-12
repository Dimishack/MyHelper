using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyHelper.Data;
using MyHelper.Services.Registrator;
using MyHelper.ViewModels.Registrator_Locator;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace MyHelper
{
    public partial class App : Application
    {
        public static Window ActivedWindow => Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsActive) ?? Current.MainWindow;
        public static Window FocusedWindow => Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsFocused) ?? Current.MainWindow;

        private static IHost? __host;

        //public static IHost Host => __host ??= Microsoft.Extensions.Hosting.Host
        //    .CreateDefaultBuilder(Environment.GetCommandLineArgs())
        //    //.ConfigureAppConfiguration(cfg => cfg.AddJsonFile("appsetting.json", true, true))
        //    .ConfigureServices((host, services) => services
        //    .AddViewModels()
        //    .AddServices())
        //    .Build();

        public static IHost Host => __host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();
        internal static void ConfigureServices(HostBuilderContext host, IServiceCollection services) => services
            .AddDatabase(host.Configuration.GetSection("Database"))
            .AddViewModels()
            .AddServices()

            ;

        public static IServiceProvider Services => Host.Services;

        protected override async void OnStartup(StartupEventArgs e)
        {
            var host = Host;

            using (var scope = Services.CreateScope())
                await scope.ServiceProvider.GetRequiredService<DbInitializer>().InitializeAsync();

            base.OnStartup(e);
            await host.StartAsync();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            using (Host) await Host.StopAsync();
        }
    }
}
