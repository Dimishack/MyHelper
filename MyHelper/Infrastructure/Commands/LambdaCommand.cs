using MyHelper.Infrastructure.Commands.Base;
using System;

namespace MyHelper.Infrastructure.Commands
{
    class LambdaCommand(Action<object?> execute, Func<object?, bool>? canExecute = null) : Command
    {
        private readonly Action<object?> _execute = execute;
        private readonly Func<object?, bool>? _canExecute = canExecute;

        protected override bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        protected override void Execute(object? parameter) => _execute(parameter);
    }
}
