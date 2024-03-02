using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyHelper.Services.Interfaces;
using MyHelper.Services.Registrator;
using MyHelper.ViewModels.Registrator;
using System;
using System.Linq;
using System.Windows;

namespace MyHelper
{
    public partial class App : Application
    {
        public static Window ActivedWindow => Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsActive);
        public static Window FocusedWindow => Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsFocused);

        private static IHost? __host;

        public static IHost Host => __host ??= Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder(Environment.GetCommandLineArgs())
            .ConfigureAppConfiguration(cfg => cfg.AddJsonFile("appsetting.json", true, true))
            .ConfigureServices((host, services) => services
            .AddViewModels()
            .AddServices())
            .Build();

        public static IServiceProvider Services => Host.Services;

        protected override async void OnStartup(StartupEventArgs e)
        {
            var host = Host;
            base.OnStartup(e);
            Services.GetRequiredService<IOpenWindows>().OpenMainWindow();
            await host.StartAsync();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using var host = Host;
            base.OnExit(e);
            await host.StopAsync();
        }
    }
}
