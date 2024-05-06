using MyHelper.Models.Purposes;

namespace MyHelper.Services.Interfaces
{
    interface IOpenWindows
    {
        void OpenMainWindow();
        void OpenChecklistChallengeWindow();
        bool OpenCreator_EditorPurposeWindow(MyPurpose purpose, string title);
        bool OpenCreator_EditorYearWindow(MyPurposes listPurposes, string title);
    }
}
