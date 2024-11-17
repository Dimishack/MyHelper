using MyHelper.Infrastructure.Commands.Base;
using System.Windows;

namespace MyHelper.Infrastructure.Commands
{
    class WindowStateChangedCommand : Command
    {
        protected override bool CanExecute(object? parameter) => parameter is Window;

        protected override void Execute(object? parameter)
        {
            if (parameter is Window window)
                window.WindowState = window.WindowState switch
                {
                    WindowState.Maximized => WindowState.Normal,
                    WindowState.Normal => WindowState.Maximized,
                    _ => WindowState.Normal,
                };
        }
    }
}
