using MyHelper.Infrastructure.Commands.Base;
using System.Diagnostics.CodeAnalysis;

namespace MyHelper.Infrastructure.Commands
{
    internal class LambdaCommandAsync([NotNull] Func<object?, Task> executeAsync, Func<object?, bool>? canExecuteAsync = null) : Command
    {
        private readonly Func<object?, Task> _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
        private readonly Func<object?, bool>? _canExecuteAsync = canExecuteAsync;

        private volatile Task? _executingAction;

        protected override bool CanExecute(object? parameter) => _canExecuteAsync?.Invoke(parameter) ?? true;

        protected override async void Execute(object? parameter)
        {
            if (!CanExecute(parameter)) return;

            Task task = _executeAsync(parameter);
            _ = Interlocked.Exchange(ref _executingAction, task);
            _executingAction = task;
            OnCanExecuteChanged();
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext: true);
            }
            catch (OperationCanceledException)
            {
            }
            OnCanExecuteChanged();
        }
    }

    internal class LambdaCommandAsync<T>([NotNull] Func<T, Task> executeAsync, Func<T, bool>? canExecuteAsync = null) : Command
    {
        private readonly Func<T, Task> _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
        private readonly Func<T, bool>? _canExecuteAsync = canExecuteAsync;

        private volatile Task? _executingAction;

        protected override bool CanExecute(object? parameter)
        {
            if(parameter is not T val) return false;
            return _canExecuteAsync?.Invoke(val) ?? true;
        }

        protected override async void Execute(object? parameter)
        {
            if (!CanExecute(parameter) || parameter is not T val) return;

            Task task = _executeAsync(val);
            _ = Interlocked.Exchange(ref _executingAction, task);
            _executingAction = task;
            OnCanExecuteChanged();
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext: true);
            }
            catch (OperationCanceledException)
            {
            }
            OnCanExecuteChanged();
        }
    }
}
