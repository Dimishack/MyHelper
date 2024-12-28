using MyHelper.Infrastructure.Commands;
using MyHelper.Interfaces;
using System.Windows.Input;

namespace MyHelper.ViewModels.Base
{
    internal abstract class MainFunctionsViewModel<T>(IRepository<T> itemsRepository) : ViewModel where T : class, IEntity, new()
    {
        protected readonly IRepository<T> _itemsRepository = itemsRepository;

        #region ShowAdd_EditUserControl : bool - Отобразить окно добавления (редактирования) элемента

        ///<summary>Отобразить окно добавления (редактирования) элемента</summary>
        public bool ShowAdd_EditUserControl => _addElement || _editElement;

        #endregion

        #region AddElement : bool - Добавить элемент

        ///<summary>Добавить элемент</summary>
        private bool _addElement;

        ///<summary>Добавить элемент</summary>
        public bool AddElement
        {
            get => _addElement;
            set
            {
                if (!Set(ref _addElement, value)) return;
                MethodBeforeAddElement();
                OnPropertyChanged(nameof(ShowAdd_EditUserControl));
                OnPropertyChanged(nameof(EnableFrameworkElements));
            }
        }

        protected virtual void MethodBeforeAddElement() { }

        #endregion

        #region EditElement : bool - Редактировать элемент

        ///<summary>Редактировать элемент</summary>
        private bool _editElement;

        ///<summary>Редактировать элемент</summary>
        public bool EditElement
        {
            get => _editElement;
            set
            {
                if (!Set(ref _editElement, value)) return;
                MethodBeforeEditElement();
                OnPropertyChanged(nameof(ShowAdd_EditUserControl));
                OnPropertyChanged(nameof(EnableFrameworkElements));
            }
        }

        protected virtual void MethodBeforeEditElement() { }

        #endregion

        #region EnableFrameworkElements : bool - Включить визуальные элементы

        ///<summary>Включить визуальные элементы</summary>
        public virtual bool EnableFrameworkElements => !ShowAdd_EditUserControl;

        #endregion

        #region LoadedCommand - Команда - загрузка окна

        ///<summary>Команда - загрузка окна</summary>
        private ICommand? _loadedCommand;

        ///<summary>Команда - загрузка окна</summary>
        public ICommand LoadedCommand => _loadedCommand
            ??= new LambdaCommand(OnLoadedCommandExecuted, CanLoadedCommandExecute);

        ///<summary>Проверка возможности выполнения - загрузка окна</summary>
        protected virtual bool CanLoadedCommandExecute(object? p) => true;

        ///<summary>Логика выполнения - загрузка окна</summary>
        protected abstract void OnLoadedCommandExecuted(object? p);

        #endregion

        #region ClosedCommand - Команда - зыкрытие окна

        ///<summary>Команда - зыкрытие окна</summary>
        private ICommand? _closedCommand;

        ///<summary>Команда - зыкрытие окна</summary>
        public ICommand ClosedCommand => _closedCommand
            ??= new LambdaCommand(OnClosedCommandExecuted, CanClosedCommandExecute);

        ///<summary>Проверка возможности выполнения - зыкрытие окна</summary>
        protected virtual bool CanClosedCommandExecute(object? p) => true;

        ///<summary>Логика выполнения - зыкрытие окна</summary>
        protected abstract void OnClosedCommandExecuted(object? p);

        #endregion

        #region AddElementCommand - Команда - добавить элемент

        ///<summary>Команда - добавить элемент</summary>
        private ICommand? _addElementCommand;

        ///<summary>Команда - добавить элемент</summary>
        public ICommand AddElementCommand => _addElementCommand
            ??= new LambdaCommandAsync(OnAddElementCommandExecuted, CanAddElementCommandExecute);

        ///<summary>Проверка возможности выполнения - добавить элемент</summary>
        protected virtual bool CanAddElementCommandExecute(object? p) => true;

        ///<summary>Логика выполнения - добавить элемент</summary>
        protected abstract Task OnAddElementCommandExecuted(object? p);

        #endregion

        #region EditElementCommand - Команда - редактировать элемент

        ///<summary>Команда - редактировать элемент</summary>
        private ICommand? _editElementCommand;

        ///<summary>Команда - редактировать элемент</summary>
        public ICommand EditElementCommand => _editElementCommand
            ??= new LambdaCommandAsync(OnEditElementCommandExecuted, CanEditElementCommandExecute);

        ///<summary>Проверка возможности выполнения - редактировать элемент</summary>
        protected virtual bool CanEditElementCommandExecute(object? p) => true;

        ///<summary>Логика выполнения - редактировать элемент</summary>
        protected abstract Task OnEditElementCommandExecuted(object? p);

        #endregion

        #region DeleteElementCommand - Команда - удалить элемент

        ///<summary>Команда - удалить элемент</summary>
        private ICommand? _deleteElementCommand;

        ///<summary>Команда - удалить элемент</summary>
        public ICommand DeleteElementCommand => _deleteElementCommand
            ??= new LambdaCommandAsync(OnDeleteElementCommandExecuted, CanDeleteElementCommandExecute);

        ///<summary>Проверка возможности выполнения - удалить элемент</summary>
        protected virtual bool CanDeleteElementCommandExecute(object? p) => true;

        ///<summary>Логика выполнения - удалить элемент</summary>
        protected abstract Task OnDeleteElementCommandExecuted(object? p);

        #endregion

        #region CancelOperationCommand - Команда - отменить операцию

        ///<summary>Команда - отменить операцию</summary>
        private ICommand? _cancelOperationCommand;

        ///<summary>Команда - отменить операцию</summary>
        public ICommand CancelOperationCommand => _cancelOperationCommand
            ??= new LambdaCommand(OnCancelOperationCommandExecuted, CanCancelOperationCommandExecute);

        ///<summary>Проверка возможности выполнения - отменить операцию</summary>
        protected virtual bool CanCancelOperationCommandExecute(object? p) => ShowAdd_EditUserControl;

        ///<summary>Логика выполнения - отменить операцию</summary>
        protected virtual void OnCancelOperationCommandExecuted(object? p) => AddElement = EditElement = false;

        #endregion

        #region SaveRepositoryCommand - Команда - сохранить репозиторий

        ///<summary>Команда - сохранить репозиторий</summary>
        private ICommand? _saveRepositoryCommand;

        ///<summary>Команда - сохранить репозиторий</summary>
        public ICommand SaveRepositoryCommand => _saveRepositoryCommand
            ??= new LambdaCommandAsync(OnSaveRepositoryCommandExecuted, CanSaveRepositoryCommandExecute);

        ///<summary>Проверка возможности выполнения - сохранить репозиторий</summary>
        private bool CanSaveRepositoryCommandExecute(object? p) => !_itemsRepository.AutoSaveChanges;

        ///<summary>Логика выполнения - сохранить репозиторий</summary>
        private async Task OnSaveRepositoryCommandExecuted(object? p) => await _itemsRepository.SaveChangedAsync();

        #endregion
    }
}
