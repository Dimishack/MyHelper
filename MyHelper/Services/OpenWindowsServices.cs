using Microsoft.Extensions.DependencyInjection;
using MyHelper.Models.Challenges;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels;
using MyHelper.Views.Windows;
using System;

namespace MyHelper.Services
{
    class OpenWindowsServices(IServiceProvider services) : IOpenWindows
    {
        private readonly IServiceProvider _services = services;
        private MainWindow? _mainWindow;
        private ChecklistChallengeWindow? _checklistChallengeWindow;

        public void OpenChecklistChallengeWindow(MyChallenge challenge)
        {
            if (_checklistChallengeWindow is { } window)
            {
                window.ShowDialog();
                return;
            }
            var viewModel = new ChecklistChallengeViewModel(challenge);
            window = new ChecklistChallengeWindow()
            {
                DataContext = viewModel
            };
            window.Closed += (_, _) => _checklistChallengeWindow = null;
            _checklistChallengeWindow = window;
            _checklistChallengeWindow.ShowDialog();
        }

        public void OpenMainWindow()
        {
            if (_mainWindow is { } window)
            {
                window.Show();
                return;
            }
            window = _services.GetRequiredService<MainWindow>();
            window.Closed += (_, _) => _mainWindow = null;
            _mainWindow = window;
            window.Show();
        }
    }
}
