using Microsoft.Extensions.DependencyInjection;
using MyHelper.Models.Purposes;
using MyHelper.Models.Challenges;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels;
using MyHelper.Views.Windows;
using System.Windows;

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

        public bool OpenCreator_EditorPurposeWindow(MyPurpose purpose, string title)
        {
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

        public bool OpenCreator_EditorYearWindow(MyPurposes listPurposes, string title)
        {
            var window = new Creator_EditorYearWindow
            {
                Title = title,
                Year = listPurposes.Year,
                Name = listPurposes.Name ?? "",
                Owner = App.ActivedWindow,
                WindowStartupLocation= WindowStartupLocation.CenterOwner
            };
            if (window.ShowDialog() != true) return false;
            listPurposes.Year = window.Year;
            listPurposes.Name = window.Name;

            return true;
        }
    }
}
