using MyHelper.Models.Purposes;

using MyHelper.Models.Challenges;

namespace MyHelper.Services.Interfaces
{
    interface IOpenWindows
    {
        void OpenMainWindow();
        bool OpenCreator_EditorPurposeWindow(MyPurpose purpose, string title);
        bool OpenCreator_EditorYearWindow(MyPurposes listPurposes, string title);
        void OpenChecklistChallengeWindow(MyChallenge challenge);
    }
}
