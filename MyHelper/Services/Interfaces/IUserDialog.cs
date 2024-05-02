namespace MyHelper.Services.Interfaces
{
    interface IUserDialog
    {
        bool WarningMessage(string message, string caption = "MyHelper");
        void InformationMessage(string message, string caption = "MyHelper");
        void ErrorMessage(string message, string caption = "MyHelper");
    }
}
