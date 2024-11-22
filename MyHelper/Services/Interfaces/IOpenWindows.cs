using MyHelper.Models.Books;
using MyHelper.Models.Challenges;
using MyHelper.Models.MyTasks;

namespace MyHelper.Services.Interfaces
{
    interface IOpenWindows
    {
        bool OpenCreator_EditorTaskWindow(MyTask task, IList<string> groups, string title);
        bool OpenCreator_EditorBookWindow(MyBook book, string title);
    }
}
