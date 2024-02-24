using MyHelper.Infrastructure.Commands;
using MyHelper.Infrastructure.Commands.Base;
using MyHelper.Models.Purposes;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.ComponentModel;

namespace MyHelper.ViewModels
{
    class ListPurposesViewModel : ViewModel
    {
        private readonly IOpenWindows _openWindows;
        private readonly IUserDialog _userDialog;
        private readonly IWorkWithJSONFile _workWithJSONFile;

        public ObservableCollection<MyPurposes>? ListMyPurposes { get; }

        #region SelectedListMyPurposes : MyPurposes - Выбранный список целей

        ///<summary>Выбранный список целей</summary>
        private MyPurposes? _selectedListMyPurposes;

        ///<summary>Выбранный список целей</summary>
        public MyPurposes? SelectedListMyPurposes
        {
            get => _selectedListMyPurposes;
            set
            {
                if (Set(ref _selectedListMyPurposes, value))
                {
                    OnPropertyChanged(nameof(MyPurposesCount));
                    OnPropertyChanged(nameof(CompletingMyPurposesCount));
                    OnPropertyChanged(nameof(Percent));
                }
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

        #region Команды

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
            ((Command)SaveListMyPurposesCommand).Executable = true;
        }

        #endregion

        #region DeletePurposeCommand - Команда удаления цели

        ///<summary>Команда удаления цели</summary>
        private ICommand? _deletePurposeCommand;

        ///<summary>Команда удаления цели</summary>
        public ICommand DeletePurposeCommand => _deletePurposeCommand
            ??= new LambdaCommand(OnDeletePurposeCommandExecuted, CanDeletePurposeCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда удаления цели</summary>
        private bool CanDeletePurposeCommandExecute(object? p) => SelectedMyPurpose is not null;

        ///<summary>Логика выполнения - Команда удаления цели</summary>
        private void OnDeletePurposeCommandExecuted(object? p)
        {
            SelectedListMyPurposes!.ListPurposes.Remove(SelectedMyPurpose!);
            ((Command)SaveListMyPurposesCommand).Executable = true;
        }

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
            _workWithJSONFile.WriteFile(@"Data/MyPurposes.json", p);
            _userDialog.InformationMessage("Список целей успешно сохранен", "MyHelper");
            ((Command)SaveListMyPurposesCommand).Executable = false;
        }

        #endregion

        #endregion

        public ListPurposesViewModel(IOpenWindows openWindows, IUserDialog userDialog, IWorkWithJSONFile workWithJSONFile)
        {
            _openWindows = openWindows;
            _userDialog = userDialog;
            _workWithJSONFile = workWithJSONFile;

            ((Command)SaveListMyPurposesCommand).Executable = false;
            if (_workWithJSONFile.ReadFile(@"Data/MyPurposes.json", out IList<MyPurposes>? listPurposes) && listPurposes is not null)
                ListMyPurposes = new(listPurposes);
            else
            {
                ListMyPurposes = new (Enumerable.Range(0, 100).Select(p => new MyPurposes
                {
                    Year = DateTime.Now.Year + p,
                    Name = $"Name {p}",
                    ListPurposes = new (Enumerable.Range(1, 23).Select(p => new MyPurpose
                    {
                        Purpose = p.ToString(),
                    })),
                    
                }));
                ((Command)SaveListMyPurposesCommand).Executable = true;
            }

            for (int i = 0; i < ListMyPurposes.Count; i++)
                ListMyPurposes[i].ListPurposes.ListChanged += ListPurposes_ListChanged;
        }

        private void ListPurposes_ListChanged(object? sender,  ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                case ListChangedType.ItemDeleted:
                case ListChangedType.ItemChanged:
                    OnPropertyChanged(nameof(MyPurposesCount));
                    OnPropertyChanged(nameof(CompletingMyPurposesCount));
                    OnPropertyChanged(nameof(Percent));
                    break;
                default:
                    break;
            }
        }
    }
}
