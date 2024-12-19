using MyHelper.Models.Books;

namespace MyHelper.Services.Interfaces
{
    interface IOpenWindows
    {
        bool OpenCreator_EditorBookWindow(MyBook book, string title);
    }
}
