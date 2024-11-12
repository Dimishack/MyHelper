using System.Windows.Input;

namespace MyHelper.Infrastructure.Commands.Base
{
    internal abstract class Command : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        private bool _executable = true;
        public bool Executable
        {
            get => _executable;
            set
            {
                if (_executable == value) return;
                _executable = value;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        bool ICommand.CanExecute(object? parameter) => _executable && CanExecute(parameter);
        void ICommand.Execute(object? parameter)
        {
            if (CanExecute(parameter))
                Execute(parameter);
        }

        private event EventHandler? CanExecuteChangedHandlers;

        protected virtual void OnCanExecuteChanged(EventArgs? e = null)
        {
            this.CanExecuteChangedHandlers?.Invoke(this, e ?? EventArgs.Empty);
        }

        protected virtual bool CanExecute(object? parameter) => true;
        protected abstract void Execute(object? parameter);
    }
}
