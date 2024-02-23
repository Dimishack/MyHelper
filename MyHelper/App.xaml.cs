using Microsoft.Extensions.DependencyInjection;
using MyHelper.Services;
using MyHelper.Services.Implementations;
using MyHelper.ViewModels;
using MyHelper.Views.Windows;
using System;
using System.Windows;

namespace MyHelper
{
    public partial class App : Application
    {
        private static IServiceProvider? _services;
        public static IServiceProvider Services => _services ??= InitializeServices().BuildServiceProvider();

        private static ServiceCollection InitializeServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<ChecklistChallengeViewModel>();

            services.AddSingleton<IOpenWindows, OpenWindowsServices>();

            services.AddTransient(
                s =>
                {
                    var model = s.GetRequiredService<MainWindowViewModel>();
                    var window = new MainWindow { DataContext = model };
                    return window;
                });
            services.AddTransient(
                s =>
                {
                    var model = s.GetRequiredService<ChecklistChallengeViewModel>();
                    var window = new ChecklistChallengeWindow { DataContext = model };
                    return window;
                });

            return services;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Services.GetRequiredService<IOpenWindows>().OpenMainWindow();
        }
    }
}
