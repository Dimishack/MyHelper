using MyHelper.Models.Challenges;

namespace MyHelper.Services.Interfaces
{
    interface IOpenWindows
    {
        void OpenMainWindow();
        void OpenChecklistChallengeWindow(MyChallenge challenge);
    }
}
