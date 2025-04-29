using MyHelper.Infrastructure.Attributes;
using MyHelper.Infrastructure.Commands;
using MyHelper.Interfaces;
using MyHelper.Services.Interfaces;
using System.Windows.Input;

namespace MyHelper.ViewModels.Base
{
    internal abstract class CrudViewModelBase<T>(
        IRepository<T> itemsRepository, 
        IPropertyDependency propertyDepenency) 
        : ConnectedViewModel (propertyDepenency), IDisposable where T : class, IEntity, new()
    {
        protected bool Disposed { get; set; } = false;

        protected readonly IRepository<T> ItemsRepository = itemsRepository;

        #region IsShowEditorUC : bool - Отобразить окно добавления (редактирования) элемента

        [ConnectedProperties(nameof(IsElementEnabled))]
        ///<summary>Отобразить окно добавления (редактирования) элемента</summary>
        public bool IsShowEditorUC => _isAddingElement || _isEditingElement;

        #endregion

        #region IsAddingElement : bool - Добавить элемент

        ///<summary>Добавить элемент</summary>
        private bool _isAddingElement;

        [ConnectedProperties(nameof(IsShowEditorUC))]
        ///<summary>Добавить элемент</summary>
        public bool IsAddingElement
        {
            get => _isAddingElement;
            set
            {
                if (!Set(ref _isAddingElement, value)) return;
                OnBeforeAddElement();
                OnConnectedPropertyChanged();
            }
        }

        protected virtual void OnBeforeAddElement() { }

        #endregion

        #region IsEditingElement : bool - Редактировать элемент

        ///<summary>Редактировать элемент</summary>
        private bool _isEditingElement;

        [ConnectedProperties(nameof(IsShowEditorUC))]
        ///<summary>Редактировать элемент</summary>
        public bool IsEditingElement
        {
            get => _isEditingElement;
            set
            {
                if (!Set(ref _isEditingElement, value)) return;
                OnBeforeEditElement();
                OnConnectedPropertyChanged();
            }
        }

        protected virtual void OnBeforeEditElement() { }

        #endregion

        #region IsElementEnabled : bool - Включить визуальные элементы

        ///<summary>Включить визуальный элемент</summary>
        public virtual bool IsElementEnabled => !IsShowEditorUC;

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
        protected virtual void OnClosedCommandExecuted(object? p) => Dispose();

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
        protected virtual bool CanCancelOperationCommandExecute(object? p) => IsShowEditorUC;

        ///<summary>Логика выполнения - отменить операцию</summary>
        protected virtual void OnCancelOperationCommandExecuted(object? p) => IsAddingElement = IsEditingElement = false;

        #endregion

        #region SaveRepositoryCommand - Команда - сохранить репозиторий

        ///<summary>Команда - сохранить репозиторий</summary>
        private ICommand? _saveRepositoryCommand;

        ///<summary>Команда - сохранить репозиторий</summary>
        public ICommand SaveRepositoryCommand => _saveRepositoryCommand
            ??= new LambdaCommandAsync(OnSaveRepositoryCommandExecuted, CanSaveRepositoryCommandExecute);

        ///<summary>Проверка возможности выполнения - сохранить репозиторий</summary>
        private bool CanSaveRepositoryCommandExecute(object? p) => !ItemsRepository.AutoSaveChanges;

        ///<summary>Логика выполнения - сохранить репозиторий</summary>
        private async Task OnSaveRepositoryCommandExecuted(object? p) => await ItemsRepository.SaveChangedAsync();


        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!Disposed)
            {
                if (disposing)
                    DisposeManagedResources();
                DisposeUnmanagedRecources();
                Disposed = true;
            }
        }

        protected virtual void DisposeUnmanagedRecources() { }

        protected virtual void DisposeManagedResources() { }
    }
}
