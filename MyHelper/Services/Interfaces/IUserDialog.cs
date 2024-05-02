namespace MyHelper.Services.Interfaces
{
    interface IUserDialog
    {
        bool WarningMessage(string message, string caption);
        void InformationMessage(string message, string caption);
        void ErrorMessage(string message, string caption = "MyHelper");
    }
}
