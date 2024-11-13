using Microsoft.EntityFrameworkCore;
using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Commands;
using MyHelper.Interfaces;
using MyHelper.Models.Targets;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    class TartetsUCViewModel(IOpenWindows openWindows,
                                  IUserDialog userDialog,
                                  IWorkWithJSONFile workWithJSONFile,
                                  IRepository<TargetsGroup> targets) : ViewModel
    {
        private readonly IOpenWindows _openWindows = openWindows;
        private readonly IUserDialog _userDialog = userDialog;
        private readonly IWorkWithJSONFile _workWithJSONFile = workWithJSONFile;
        private readonly IRepository<TargetsGroup> _targetsRepository = targets;

        #region Properties...

        #region GroupsTargets : ObservableCollection<TargetsGroup> - Список групп с целями

        ///<summary>Список групп с целями</summary>
        public ObservableCollection<TargetsModel> GroupsTargets { get; } = [];

        #endregion

        #region SelectedTargets : TargetsModel - Выбранный список целей

        ///<summary>Выбранный список целей</summary>
        private TargetsModel? _selectedTargets;

        ///<summary>Выбранный список целей</summary>
        public TargetsModel? SelectedTargets
        {
            get => _selectedTargets;
            set
            {
                if (_selectedTargets == value) return;
                _selectedTargets?.Targets.Clear();
                if (value is not null)
                {
                    var targets = _targetsRepository.Items.Include(g => g.Targets).First(ts => ts.Id == value.Id).Targets;
                    if (targets == null) return;
                    foreach (var target in targets)
                        value.Targets.Add(new TargetModel(target));
                }
                Set(ref _selectedTargets, value);
                OnPropertyChanged(nameof(SelectedTargetsView));

            }
        }

        #endregion

        private readonly CollectionViewSource _selectedTargetsViewSource = new();
        public ICollectionView SelectedTargetsView => _selectedTargetsViewSource.View;

        #endregion

        #region Commands...

        #region LoadCommand - Загрузка окна

        ///<summary>Загрузка окна</summary>
        private ICommand? _loadCommand;

        ///<summary>Загрузка окна</summary>
        public ICommand LoadCommand => _loadCommand
            ??= new LambdaCommand(OnLoadCommandExecuted);

        ///<summary>Логика выполнения - Загрузка окна</summary>
        private void OnLoadCommandExecuted(object? p)
        {
            foreach (TargetsGroup targets in _targetsRepository.Items)
                GroupsTargets.Add(new TargetsModel(targets));
            _selectedTargetsViewSource.Source = SelectedTargets?.Targets;
            OnPropertyChanged(nameof(SelectedTargetsView));
        }
        #endregion

        #region CreateTargetsGroupCommand - Команда - создать новую группу списка целей

        ///<summary>Команда - создать новую группу списка целей</summary>
        private ICommand? _createTargetsGroupCommand;

        ///<summary>Команда - создать новую группу списка целей</summary>
        public ICommand CreateTargetsGroupCommand => _createTargetsGroupCommand
            ??= new LambdaCommand(OnCreateTargetsGroupCommandExecuted);

        ///<summary>Логика выполнения - создать новую группу списка целей</summary>
        private void OnCreateTargetsGroupCommandExecuted(object? p)
        {
            var newYear = GroupsTargets[^1].Year == 0 ? (uint)DateTime.Now.Year : GroupsTargets[^1].Year + 1;
            TargetsGroup newTargetsGroup = new()
            {
                Year = newYear,
                Name = $"GroupsTargets {newYear}",
            };
            GroupsTargets.Add(new TargetsModel(_targetsRepository.Add(newTargetsGroup)));
        }

        #endregion

        #region ChangeTargetsGroupCommand - Команда - изменить значения групп списка целей

        ///<summary>Команда - изменить значения групп списка целей</summary>
        private ICommand? _changeTargetsGroupCommand;

        ///<summary>Команда - изменить значения групп списка целей</summary>
        public ICommand ChangeTargetsGroupCommand => _changeTargetsGroupCommand
            ??= new LambdaCommandAsync(OnChangeTargetsGroupCommandExecuted, CanChangeTargetsGroupCommandExecute);

        ///<summary>Проверка возможности выполнения - изменить значения групп списка целей</summary>
        private bool CanChangeTargetsGroupCommandExecute(object? p) => _selectedTargets is not null;

        ///<summary>Логика выполнения - изменить значения групп списка целей</summary>
        private async Task OnChangeTargetsGroupCommandExecuted(object? p)
        {
            _selectedTargets.Year = 0;
            _selectedTargets.Name = "Пожизненные";
            var item = _targetsRepository.Get(_selectedTargets.Id);
            await _targetsRepository.UpdateAsync(item);
            CollectionViewSource.GetDefaultView(GroupsTargets).Refresh();
        }

        #endregion

        #region RemoveTargetsGroupCommand - Команда - удалить группу списка целей

        ///<summary>Команда - удалить группу списка целей</summary>
        private ICommand? _removeTargetsGroupCommand;

        ///<summary>Команда - удалить группу списка целей</summary>
        public ICommand RemoveTargetsGroupCommand => _removeTargetsGroupCommand
            ??= new LambdaCommandAsync(OnRemoveTargetsGroupCommandExecuted, CanRemoveTargetsGroupCommandExecute);

        ///<summary>Проверка возможности выполнения - удалить группу списка целей</summary>
        private bool CanRemoveTargetsGroupCommandExecute(object? p) => _selectedTargets is not null;

        ///<summary>Логика выполнения - удалить группу списка целей</summary>
        private async Task OnRemoveTargetsGroupCommandExecuted(object? p)
        {
            await _targetsRepository.RemoveAsync(_selectedTargets!.Id);
            GroupsTargets.Remove(_selectedTargets);
            SelectedTargets = GroupsTargets.Count > 0 ? GroupsTargets.Last() : null;
        }

        #endregion

        #endregion
        public TartetsUCViewModel() : this(null, null, null, null)
        {

        }
    }
}
