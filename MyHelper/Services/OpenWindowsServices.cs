using MyHelper.Models.Books;
using MyHelper.Services.Interfaces;
using MyHelper.Views.Windows.Books;
using System.Windows;

namespace MyHelper.Services
{
    class OpenWindowsServices : IOpenWindows
    {

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
