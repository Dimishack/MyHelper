using MyHelper.Models.Purposes;

namespace MyHelper.Services.Interfaces
{
    interface IOpenWindows
    {
        void OpenMainWindow();
        void OpenChecklistChallengeWindow();
        bool OpenCreator_EditPurposeWindow(MyPurpose purpose);
    }
}
