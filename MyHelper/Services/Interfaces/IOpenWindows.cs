using MyHelper.Models.Purposes;

using MyHelper.Models.Challenges;

namespace MyHelper.Services.Interfaces
{
    interface IOpenWindows
    {
        void OpenMainWindow();
        bool OpenCreator_EditorPurposeWindow(MyPurpose purpose, string title);
        bool OpenCreator_EditorYearWindow(MyPurposes listPurposes, string title);
        bool OpenCreator_EditorChallengeWindow(MyChallenge challenge, string duration, string title);
        DateTime OpenSelectStartDateWindow(MyChallenge challenge);
        void OpenChecklistChallengeWindow(MyChallenge challenge);
    }
}
