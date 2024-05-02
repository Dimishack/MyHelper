using MyHelper.Infrastructure.Attributes;
using MyHelper.Infrastructure.Commands;
using MyHelper.Infrastructure.Commands.Base;
using MyHelper.Models.Purposes;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    class ListPurposesUserControlViewModel(IOpenWindows openWindows, IUserDialog userDialog, IWorkWithJSONFile workWithJSONFile) : ViewModel
    {
        private readonly IOpenWindows _openWindows = openWindows;
        private readonly IUserDialog _userDialog = userDialog;
        private readonly IWorkWithJSONFile _workWithJSONFile = workWithJSONFile;

        #region Свойства
        #region ListMyPurposes : ObservableCollection<MyPurposes> - Коллекция списков целей

        ///<summary>Коллекция списков целей</summary>
        private ObservableCollection<MyPurposes>? _listMyPurposes;

        ///<summary>Коллекция списков целей</summary>
        public ObservableCollection<MyPurposes>? ListMyPurposes { get => _listMyPurposes; set => Set(ref _listMyPurposes, value); }

        #endregion

        #region SelectedListMyPurposes : MyPurposes - Выбранный список целей

        ///<summary>Выбранный список целей</summary>
        private MyPurposes? _selectedListMyPurposes;

        ///<summary>Выбранный список целей</summary>
        [DependencyOn(nameof(MyPurposesCount))]
        [DependencyOn(nameof(CompletingMyPurposesCount))]
        [DependencyOn(nameof(Percent))]
        [DependencyOn(nameof(OffsetLimeGreenColor))]
        public MyPurposes? SelectedListMyPurposes
        {
            get => _selectedListMyPurposes;
            set
            {
                if (Set(ref _selectedListMyPurposes, value))
                    UpdatePropertyChanged();
            }
        }

        #endregion

        #region SelectedMyPurpose : MyPurpose - Выбранная цель

        ///<summary>Выбранная цель</summary>
        private MyPurpose? _selectedMyPurpose;

        ///<summary>Выбранная цель</summary>
        public MyPurpose? SelectedMyPurpose { get => _selectedMyPurpose; set => Set(ref _selectedMyPurpose, value); }

        #endregion

        #region MyPurposesCount : int - Количество целей в списке

        /// <summary>Количество целей в списке</summary>
        public int MyPurposesCount => SelectedListMyPurposes is null
            ? 0
            : SelectedListMyPurposes.ListPurposes.Count;

        #endregion

        #region CompletingMyPurposesCount : int - Количество выполненных целей в списке

        /// <summary>Количество выполненных целей в списке</summary>
        public int CompletingMyPurposesCount => SelectedListMyPurposes is null
            ? 0
            : SelectedListMyPurposes.ListPurposes.Where(p => p.IsCompleted).Count();

        #endregion

        #region Percent : double - Процент выполненных целей

        /// <summary>Процент выполненных целей</summary>
        public double Percent => (double)CompletingMyPurposesCount / (MyPurposesCount == 0 ? 1 : MyPurposesCount);

        #endregion

        #region OffsetLimeGreenColor : double - Местоположение лаймового цвета в ProgressBar'e

        /// <summary>Местоположение лаймового цвета в ProgressBar'e</summary>
        public double OffsetLimeGreenColor => 2.0 - Percent;

        #endregion 
        #endregion

        #region Команды
        #region LoadCommand - Загрузка окна

        ///<summary>Загрузка окна</summary>
        private ICommand? _loadCommand;

        ///<summary>Загрузка окна</summary>
        public ICommand LoadCommand => _loadCommand
            ??= new LambdaCommand(OnLoadCommandExecuted);

        ///<summary>Логика выполнения - Загрузка окна</summary>
        private void OnLoadCommandExecuted(object? p)
        {
            if (_listMyPurposes is not null) return;

            ((Command)SaveListMyPurposesCommand).Executable = false;
            if (_workWithJSONFile.ReadFile(@"Data/Purposes.json", out IList<MyPurposes>? listPurposes)
                && listPurposes is not null)
                ListMyPurposes = new(listPurposes);
            else
            {
                ListMyPurposes = new(Enumerable.Range(0, 1000).Select(p => new MyPurposes
                {
                    Year = DateTime.Now.Year + p,
                    Name = $"Name {p}",
                    ListPurposes = new(Enumerable.Range(1, 1000).Select(p => new MyPurpose
                    {
                        Purpose = p.ToString(),
                    }).ToList()),

                }));
                ((Command)SaveListMyPurposesCommand).Executable = true;
            }
            for (int i = 0; i < ListMyPurposes.Count; i++)
                ListMyPurposes[i].ListPurposes.ListChanged += ListPurposes_ListChanged;
        }

        #endregion

        #region CreateNewYearCommand - Команда создания нового списка целей

        ///<summary>Команда создания нового списка целей</summary>
        private ICommand? _createNewYearCommand;

        ///<summary>Команда создания нового списка целей</summary>
        public ICommand CreateNewYearCommand => _createNewYearCommand
            ??= new LambdaCommand(OnCreateNewYearCommandExecuted);

        ///<summary>Логика выполнения - Команда создания нового списка целей</summary>
        private void OnCreateNewYearCommandExecuted(object? p)
        {
            MyPurposes listPurposes = new();
            if (!_openWindows.OpenCreator_EditorYearWindow(listPurposes)) return;
            ListMyPurposes?.Add(listPurposes);
            ListMyPurposes![^1].ListPurposes.ListChanged += ListPurposes_ListChanged;
            _userDialog.InformationMessage("Новый список целей успешно добавлен", "MyHepler");
            ((Command)SaveListMyPurposesCommand).Executable = true;
        }

        #endregion

        #region DeleteYearCommand - Команда удаления списка целей

        ///<summary>Команда удаления списка целей</summary>
        private ICommand? _deleteYearCommand;

        ///<summary>Команда удаления списка целей</summary>
        public ICommand DeleteYearCommand => _deleteYearCommand
            ??= new LambdaCommand(OnDeleteYearCommandExecuted, CanDeleteYearCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда удаления списка целей</summary>
        private bool CanDeleteYearCommandExecute(object? p) => SelectedListMyPurposes is not null
            && !SelectedListMyPurposes.Name.Equals("все цели", StringComparison.CurrentCultureIgnoreCase);

        ///<summary>Логика выполнения - Команда удаления списка целей</summary>
        private void OnDeleteYearCommandExecuted(object? p)
        {
            ListMyPurposes?.Remove(SelectedListMyPurposes!);
            ((Command)SaveListMyPurposesCommand).Executable = true;
        }

        #endregion

        #region EditYearCommand - Команда редактирования списка целей

        ///<summary>Команда редактирования списка целей</summary>
        private ICommand? _editYearCommand;

        ///<summary>Команда редактирования списка целей</summary>
        public ICommand EditYearCommand => _editYearCommand
            ??= new LambdaCommand(OnEditYearCommandExecuted, CanEditYearCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда редактирования списка целей</summary>
        private bool CanEditYearCommandExecute(object? p) => SelectedListMyPurposes is not null
            && !SelectedListMyPurposes.Name.Equals("все цели", StringComparison.CurrentCultureIgnoreCase);

        ///<summary>Логика выполнения - Команда редактирования списка целей</summary>
        private void OnEditYearCommandExecuted(object? p)
        {
            if (!_openWindows.OpenCreator_EditorYearWindow(SelectedListMyPurposes!)) return;

            CollectionViewSource.GetDefaultView(ListMyPurposes).Refresh();
            _userDialog.InformationMessage("Список целей успешно отредактирован", "MyHelper");
            ((Command)SaveListMyPurposesCommand).Executable = true;
        }

        #endregion

        #region CreateNewPurposeCommand - Команда создания цели

        ///<summary>Команда создания цели</summary>
        private ICommand? _сreateNewPurposeCommand;

        ///<summary>Команда создания цели</summary>
        public ICommand CreateNewPurposeCommand => _сreateNewPurposeCommand
            ??= new LambdaCommand(OnCreateNewPurposeCommandExecuted, CanCreateNewPurposeCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда создания цели</summary>
        private bool CanCreateNewPurposeCommandExecute(object? p) => SelectedListMyPurposes is not null;

        ///<summary>Логика выполнения - Команда создания цели</summary>
        private void OnCreateNewPurposeCommandExecuted(object? p)
        {
            var purpose = new MyPurpose();
            if (!_openWindows.OpenCreator_EditPurposeWindow(purpose)) return;
            SelectedListMyPurposes?.ListPurposes.Add(purpose);
            _userDialog.InformationMessage("Цель успешно добавлена в список", "MyHelper");
        }

        #endregion

        #region DeletePurposeCommand - Команда удаления цели

        ///<summary>Команда удаления цели</summary>
        private ICommand? _deletePurposeCommand;

        ///<summary>Команда удаления цели</summary>
        public ICommand DeletePurposeCommand => _deletePurposeCommand
            ??= new LambdaCommand(OnDeletePurposeCommandExecuted, CanDeletePurposeCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда удаления цели</summary>
        private bool CanDeletePurposeCommandExecute(object? p) => p is not null
            && p is MyPurpose;

        ///<summary>Логика выполнения - Команда удаления цели</summary>
        private void OnDeletePurposeCommandExecuted(object? p)
            => SelectedListMyPurposes!.ListPurposes.Remove((p as MyPurpose)!);

        #endregion

        #region EditMyPurposeCommand - Редактирование цели

        ///<summary>Редактирование цели</summary>
        private ICommand? _editMyPurposeCommand;

        ///<summary>Редактирование цели</summary>
        public ICommand EditMyPurposeCommand => _editMyPurposeCommand
            ??= new LambdaCommand(OnEditMyPurposeCommandExecuted, CanEditMyPurposeCommandExecute);

        ///<summary>Проверка возможности выполнения - Редактирование цели</summary>
        private bool CanEditMyPurposeCommandExecute(object? p) => SelectedMyPurpose is not null;

        ///<summary>Логика выполнения - Редактирование цели</summary>
        private void OnEditMyPurposeCommandExecuted(object? p)
        {
            if (!_openWindows.OpenCreator_EditPurposeWindow(SelectedMyPurpose!)) return;

            CollectionViewSource.GetDefaultView(SelectedListMyPurposes?.ListPurposes).Refresh();
            _userDialog.InformationMessage("Цель отредактирована", "MyHelper");
            ((Command)SaveListMyPurposesCommand).Executable = true;
        }

        #endregion

        #region SaveListMyPurposesCommand - Команда сохранения списка целей

        ///<summary>Команда сохранения списка целей</summary>
        private ICommand? _saveListMyPurposesCommand;

        ///<summary>Команда сохранения списка целей</summary>
        public ICommand SaveListMyPurposesCommand => _saveListMyPurposesCommand
            ??= new LambdaCommand(OnSaveListMyPurposesCommandExecuted, CanSaveListMyPurposesCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда сохранения списка целей</summary>
        private bool CanSaveListMyPurposesCommandExecute(object? p) => true;

        ///<summary>Логика выполнения - Команда сохранения списка целей</summary>
        private void OnSaveListMyPurposesCommandExecuted(object? p)
        {
            _workWithJSONFile.WriteFile(@"Data/Purposes.json", p);
            _userDialog.InformationMessage("Список целей успешно сохранен", "MyHelper");
            ((Command)SaveListMyPurposesCommand).Executable = false;
        }

        #endregion
        #endregion

        private void ListPurposes_ListChanged(object? sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                case ListChangedType.ItemDeleted:
                case ListChangedType.ItemChanged:
                    UpdatePropertyChanged(nameof(SelectedListMyPurposes));
                    ((Command)SaveListMyPurposesCommand).Executable = true;
                    break;
                default:
                    break;
            }
        }

        private void UpdatePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            var property = this.GetType().GetProperty(propertyName!);
            if (property is null) return;

            foreach (var attr in property.GetCustomAttributes(false))
                OnPropertyChanged(((DependencyOnAttribute)attr).Name);
        }
    }
}
