using Microsoft.Extensions.DependencyInjection;
using MyHelper.Models.Purposes;
using MyHelper.Models.Challenges;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels;
using MyHelper.Views.Windows;
using System.Windows;
using MyHelper.Models.MyTasks;
using MyHelper.Models.Books;
using MyHelper.Views.Windows.Books;

namespace MyHelper.Services
{
    class OpenWindowsServices(IServiceProvider services) : IOpenWindows
    {
        private readonly IServiceProvider _services = services;
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
                DataContext = viewModel,
                Owner = App.ActivedWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Closed += (_, _) => _checklistChallengeWindow = null;
            _checklistChallengeWindow = window;
            _checklistChallengeWindow.ShowDialog();
        }

        public bool OpenCreator_EditorChallengeWindow(MyChallenge challenge, string duration, string title)
        {
            var window = new Creator_EditorChallengeWindow
            {
                Title = title,
                Challenge = challenge.Challenge,
                Duration = duration.Contains("Все") ? "Месяц" : duration,
                Note = challenge.Note,
                Owner = App.ActivedWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            if (window.ShowDialog() != true) return false;
            challenge.Challenge = window.Challenge;
            challenge.Duration = window.Duration;
            challenge.Note = window.Note;
            return true;
        }

        public DateTime OpenSelectStartDateWindow(MyChallenge challenge)
        {
            var window = new SelectStartDateWindow
            {
                Challenge = challenge.Challenge,
                Owner = App.ActivedWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            window.ShowDialog();
            return window.StartDate;
        }

        public bool OpenCreator_EditorPurposeWindow(MyPurpose purpose, string title)
        {
            var window = new Creator_EditorPurposeWindow
            {
                Title = title,
                Purpose = purpose.Purpose ?? String.Empty,
                Note = purpose.Note ?? String.Empty,
                Owner = App.ActivedWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
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
                NameYear = listPurposes.Name ?? "",
                Owner = App.ActivedWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (window.ShowDialog() != true) return false;
            listPurposes.Year = window.Year;
            listPurposes.Name = window.NameYear;

            return true;
        }

        public bool OpenCreator_EditorTaskWindow(MyTask task, IList<string> groups, string title)
        {
            groups[0] = string.Empty;
            var window = new Creator_EditorTaskWindow
            {
                Title = title,
                Task = task.Task,
                Prompt = task.Prompt,
                Important = task.Important,
                Term = task.Term ?? DateTime.Today,
                SelectedGroup = task.Group ?? string.Empty,
                Note = task.Note,
                Groups = groups,
                Owner = App.ActivedWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            if (window.ShowDialog() != true) return false;

            task.Task = window.Task;
            task.Prompt = window.Prompt;
            task.Important = window.Important;
            task.Term = window.Term;
            task.Group = window.SelectedGroup == string.Empty ? null : window.SelectedGroup;
            task.Note = window.Note;
            return true;
        }

        public bool OpenCreator_EditorBookWindow(MyBook book, string title)
        {
            var window = new Creator_EditorBookWindow
            {
                Title = title,
                Author = book.Author,
                NameBook = book.Name,
                Pages = book.Pages,
                Owner = App.ActivedWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            if (window.ShowDialog() != true) return false;

            book.Name = window.NameBook;
            book.Author = window.Author;
            book.Pages = window.Pages;
            return true;
        }
    }
}
