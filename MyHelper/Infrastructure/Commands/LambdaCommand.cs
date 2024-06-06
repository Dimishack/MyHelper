using MyHelper.Infrastructure.Commands.Base;

namespace MyHelper.Infrastructure.Commands
{
    class LambdaCommand(Action<object?> execute, Func<object?, bool>? canExecute = null) : Command
    {
        private readonly Action<object?> _execute = execute;
        private readonly Func<object?, bool>? _canExecute = canExecute;

        protected override bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        protected override void Execute(object? parameter) => _execute(parameter);
    }

    internal class LambdaCommand<T>(Action<T> execute, Func<T, bool>? canExecute = null) : Command
    {
        private readonly Action<T> _execute = execute;
        private readonly Func<T, bool>? _canExecute = canExecute;

        protected override bool CanExecute(object? parameter)
        {
            if (parameter is not T val) return false;
            return _canExecute?.Invoke(val) ?? true;
        }

        protected override void Execute(object? parameter)
        {
            if(!CanExecute(parameter) || parameter is not T val) return;
            _execute(val);
        }

        
    }
}
