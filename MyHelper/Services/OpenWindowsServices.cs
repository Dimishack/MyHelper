using Microsoft.Extensions.DependencyInjection;
using MyHelper.Models.Purposes;
using MyHelper.Services.Interfaces;
using MyHelper.Views.Windows;
using System;
using System.Windows;

namespace MyHelper.Services
{
    class OpenWindowsServices(IServiceProvider services) : IOpenWindows
    {
        private readonly IServiceProvider _services = services;
        private MainWindow? _mainWindow;
        private ChecklistChallengeWindow? _checklistChallengeWindow;

        public void OpenChecklistChallengeWindow()
        {
            if (_checklistChallengeWindow is { } window)
            {
                window.ShowDialog();
                return;
            }
            window = _services.GetRequiredService<ChecklistChallengeWindow>();
            window.Closed += (_, _) => _checklistChallengeWindow = null;
            _checklistChallengeWindow = window;
            window.ShowDialog();
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

        public bool OpenCreator_EditPurposeWindow(MyPurpose purpose)
        {
            var title = "Редактировать цель";
            if (string.IsNullOrEmpty(purpose.Purpose) && string.IsNullOrEmpty(purpose.Note))
                title = "Создать цель";

            var window = new Creator_EditorPurposeWindow
            {
                Title = title,
                Purpose = purpose.Purpose ?? String.Empty,
                Note = purpose.Note ?? String.Empty,
                Owner = App.ActivedWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (window.ShowDialog() != true) return false;
            purpose.Purpose = window.Purpose;
            purpose.Note = window.Note;

            return true;
        }
    }
}
