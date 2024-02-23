using MyHelper.Services.Interfaces;
using System.Windows;

namespace MyHelper.Services
{
    class UserDialogServices : IUserDialog
    {
        public void ErrorMessage(string message, string caption) => MessageBox.Show(
            message
            , caption
            , MessageBoxButton.OK
            , MessageBoxImage.Error);

        public void InformationMessage(string message, string caption) => MessageBox.Show(
            message
            , caption
            , MessageBoxButton.OK
            , MessageBoxImage.Information);

        public bool WarningMessage(string message, string caption) => MessageBox.Show(
            message
            , caption
            , MessageBoxButton.YesNo
            , MessageBoxImage.Warning) == MessageBoxResult.Yes;
    }
}
