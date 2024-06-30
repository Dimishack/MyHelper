using MyHelper.Models.Purposes;

using MyHelper.Models.Challenges;
using MyHelper.Models.MyTasks;
using MyHelper.Models.Books;

namespace MyHelper.Services.Interfaces
{
    interface IOpenWindows
    {
        bool OpenCreator_EditorPurposeWindow(MyPurpose purpose, string title);
        bool OpenCreator_EditorYearWindow(MyPurposes listPurposes, string title);
        bool OpenCreator_EditorChallengeWindow(MyChallenge challenge, string duration, string title);
        DateTime OpenSelectStartDateWindow(MyChallenge challenge);
        void OpenChecklistChallengeWindow(MyChallenge challenge);
        bool OpenCreator_EditorTaskWindow(MyTask task, IList<string> groups, string title);
        bool OpenCreator_EditorBookWindow(MyBook book, string title);
    }
}
