using Microsoft.Extensions.Hosting;
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

        public static IHost Host => __host ??= Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder(Environment.GetCommandLineArgs())
            //.ConfigureAppConfiguration(cfg => cfg.AddJsonFile("appsetting.json", true, true))
            .ConfigureServices((host, services) => services
            .AddViewModels()
            .AddServices())
            .Build();

        public static IServiceProvider Services => Host.Services;

        protected override async void OnStartup(StartupEventArgs e)
        {
            var host = Host;
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
