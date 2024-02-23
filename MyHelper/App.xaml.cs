using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyHelper.Services.Interfaces;
using MyHelper.Services.Registrator;
using MyHelper.ViewModels.Registrator_Locator;
using System;
using System.Windows;

namespace MyHelper
{
    public partial class App : Application
    {
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
            base.OnExit(e);
            using var host = Host;
            await host.StopAsync();
        }
    }
}
