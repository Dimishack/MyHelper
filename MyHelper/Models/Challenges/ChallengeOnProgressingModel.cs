using MyHelper.DAL.Entyties;
using MyHelper.Models.Base;
using MyHelper.Models.Enums;
using System.Collections.ObjectModel;

namespace MyHelper.Models.Challenges
{
    internal class ChallengeOnProgressingModel : BaseModel, IDisposable
    {
        private readonly DateTime _dateToday = DateTime.Today;
        private readonly Challenge _challenge;
        public event EventHandler? CheckedChanged;

        public int Id => _challenge.Id;
        public string Name => _challenge.Name;
        public string? Note => _challenge.Note;
        public bool InProgress
        {
            get => _challenge.InProgress;
            private set => _challenge.InProgress = value;
        }

        public int Duration { get => _challenge.Duration; set => _challenge.Duration = value; }

        public DateTime? DateStart
        {
            get => _challenge.Start;
            private set => _challenge.Start = value;
        }
        public DateTime? DateEnd
        {
            get => _challenge.End;
            private set => _challenge.End = value;
        }

        public int Regularity
        {
            get => _challenge.Regularity;
            private set => _challenge.Regularity = value;
        }

        public int? AdditionalRegularity
        {
            get => _challenge.AdditionalRegularity;
            private set => _challenge.AdditionalRegularity = value;
        }

        public int? DaysLeft
        {
            get
            {
                if (DateEnd.HasValue)
                {
                    if (Status == ChallengeStatus.Progress)
                    {
                        int result = DateEnd.Value.DayOfYear - _dateToday.DayOfYear + 1;
                        if (DateEnd.Value.Year > _dateToday.Year)
                            result += DateTime.IsLeapYear(_dateToday.Year) ? 366 : 365;
                        return result;
                    }
                }
                return null;
            }
        }

        public ChallengeStatus? Status
        {
            get
            {
                if (DateStart.HasValue && DateEnd.HasValue)
                {
                    if (_dateToday < DateStart.Value) return ChallengeStatus.Ready;
                    if (_dateToday > DateEnd.Value) return ChallengeStatus.Success;
                    return ChallengeStatus.Progress;
                }
                return 0;
            }
        }

        private int _progressCount = 0;

        public int ProgressCount
        {
            get => _progressCount;
            set
            {
                if (Set(ref _progressCount, value))
                {
                    OnPropertyChanged(nameof(Progress));
                    OnPropertyChanged(nameof(OffsetProgressed));
                }
            }
        }

        public double Progress => (double)_progressCount / CheckList.Count;

        public double OffsetProgressed => 2.0 - Progress;

        public ObservableCollection<CheckModel> CheckList { get; } = [];

        public ChallengeOnProgressingModel(Challenge challenge)
        {
            if (!challenge.InProgress
                || challenge.Start is null
                || challenge.End is null)
                throw new ArgumentException($"Была произведена попытка отправить челлендж, находящийся не на выполнении" +
                                            $", либо челлендж не имеет свойства, определяющий челлендж на выполнении");
            _challenge = challenge;
            int progressCount = 0;
            if (_challenge.InProgress)
                CheckList.CollectionChanged += CheckList_CollectionChanged;
            foreach (var check in challenge.CheckList)
            {
                if (check.Checked) progressCount++;
                CheckList.Add(new CheckModel(check));
            }
            ProgressCount = progressCount;
        }

        public ChallengeOnProgressingModel(Challenge challenge, ChallengeModelStart addChecklist)
        {
            _challenge = challenge;
            InProgress = true;
            DateStart = addChecklist.DateStart;
            DateEnd = addChecklist.DateEnd;
            Regularity = addChecklist.Regularity + 1;
            Duration = addChecklist.Duration + 1;
            if (addChecklist.Regularity == (int)ChallengeRegularity.ByDayOfTheWeek)
            {
                var result = 0;
                var pow = 0;
                for (int i = 1; i < addChecklist.DaysOfWeek.Length; i++)
                {
                    if (addChecklist.DaysOfWeek[i])
                    {

                        result += (i + 1) * (int)Math.Pow(10, pow++);
                    }
                }
                if (addChecklist.DaysOfWeek[0])
                    result += 1 * (int)Math.Pow(10, pow);

                AdditionalRegularity = result;
            }
            else if (addChecklist.Regularity == (int)ChallengeRegularity.ByCountDays)
            {
                for (int i = 0; i < addChecklist.CountDay.Length; i++)
                {
                    if (addChecklist.CountDay[i])
                    {
                        AdditionalRegularity = i + 1;
                        break;
                    }
                }
            }
            int index = 0;
            int step = 1;
            List<Check> checklist = [];

            if (addChecklist.Regularity == (int)ChallengeRegularity.EveryOtherDay)
                step = 2;

            CheckList.CollectionChanged += CheckList_CollectionChanged;
            while (index < addChecklist.DayCount)
            {
                var check = new Check()
                {
                    NumberDay = index + 1,
                    Date = DateStart.Value.AddDays(index),
                    Checked = false,
                    ChallengeId = Id,
                };
                index += step;
                checklist.Add(check);
                CheckList.Add(new CheckModel(check));
            }
            _challenge.CheckList = checklist;
        }

        private void CheckList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems is not null && e.NewItems[0] is CheckModel newCheck)
                        newCheck.PropertyChanged += Check_PropertyChanged;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is not null && e.OldItems[0] is CheckModel oldCheck)
                        oldCheck.PropertyChanged -= Check_PropertyChanged;
                    break;
                default:
                    break;
            }
        }

        private void Check_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is CheckModel check
                && e.PropertyName == nameof(check.Checked))
            {
                ProgressCount += check.Checked ? 1 : -1;
                CheckedChanged?.Invoke(sender, EventArgs.Empty);
            }
        }

        public void StopChallenge()
        {
            InProgress = false;
            DateStart = null;
            DateEnd = null;
            Duration = 0;
            Regularity = 0;
            AdditionalRegularity = null;
            _challenge.CheckList.Clear();
            Dispose();
        }

        public void Dispose()
        {
            for (int i = 0; i < CheckList.Count; i++)
                CheckList[i].PropertyChanged -= Check_PropertyChanged;
            CheckList.Clear();
            CheckList.CollectionChanged -= CheckList_CollectionChanged;
        }
    }
}
