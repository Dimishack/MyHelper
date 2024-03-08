using MyHelper.Infrastructure.Commands.Base;
using System.Windows;

namespace MyHelper.Infrastructure.Commands
{
    class CloseDialogCommand : Command
    {
        public bool? DialogResult { get; set; }
        protected override bool CanExecute(object? parameter) => parameter is Window;

        protected override void Execute(object? parameter)
        {
            if (!CanExecute(parameter)) return;

            var window = (Window)parameter!;
            window.DialogResult = this.DialogResult;
            window.Close();
        }
    }
}
