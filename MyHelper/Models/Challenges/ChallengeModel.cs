using MyHelper.DAL.Entyties;
using MyHelper.Models.Base;
using MyHelper.Models.Enums;
using System.Collections.ObjectModel;

namespace MyHelper.Models.Challenges
{
    internal class ChallengeModel : BaseModel, IDisposable
    {
        private readonly DateTime _dateToday = DateTime.Today;
        private readonly Challenge _challenge;
        public event EventHandler? CheckedChanged;
        public ChallengeModel(Challenge challenge)
        {
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

        private void CheckList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if(e.NewItems is not null && e.NewItems[0] is CheckModel newCheck)
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
            if(sender is CheckModel check 
                && e.PropertyName == nameof(check.Checked))
            {
                ProgressCount += check.Checked ? 1 : -1;
                CheckedChanged?.Invoke(sender, EventArgs.Empty);
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

        public int Id => _challenge.Id;
        public string Name { get => _challenge.Name; set => _challenge.Name = value; }
        public string? Note { get => _challenge.Note; set => _challenge.Note = value; }
        public string? Duration { get => _challenge.Duration; set => _challenge.Duration = value; }

        public DateTime? DateStart
        {
            get => _challenge.Start;
            set => _challenge.Start = value;
        }
        public DateTime? DateEnd
        {
            get => _challenge.End;
            set => _challenge.End = value;
        }

        public bool InProgress
        {
            get => _challenge.InProgress;
            set => _challenge.InProgress = value;
        }

        public string? Regularity
        {
            get => _challenge.Regularity;
            set => _challenge.Regularity = value;
        }

        public int? AdditionalRegularity
        {
            get => _challenge.AdditionalRegularity;
            set => _challenge.AdditionalRegularity = value;
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

        public ObservableCollection<CheckModel> CheckList { get; } = [];


        public IList<Check> StartChallenge(ChallengeModelStart challenge)
        {
            InProgress = true;
            DateStart = challenge.DateStart;
            DateEnd = challenge.DateEnd;
            Regularity = challenge.Regularity;
            Duration = challenge.Duration;
            if (challenge.Regularity == "По дням недели")
            {
                var result = 0;
                var pow = 0;
                for (int i = 1; i < challenge.DaysOfWeek.Length; i++)
                {
                    if (challenge.DaysOfWeek[i])
                    {

                        result += (i + 1) * (int)Math.Pow(10, pow++);
                    }
                }
                if (challenge.DaysOfWeek[0])
                    result += 1 * (int)Math.Pow(10, pow);

                AdditionalRegularity = result;
            }
            else if (Regularity == "Кол-во дней в неделю")
            {
                for (int i = 0; i < challenge.CountDay.Length; i++)
                {
                    if (challenge.CountDay[i])
                    {
                        AdditionalRegularity = i + 1;
                        break;
                    }
                }
            }
            List<Check> checklist = [];
            int index = 0;
            int step = 1;

            if (Regularity == "Через день")
                step = 2;

            CheckList.CollectionChanged += CheckList_CollectionChanged;
            while (index < challenge.DayCount)
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
            return checklist;
        }

        public void StopChallenge()
        {
            InProgress = false;
            DateStart = null;
            DateEnd = null;
            Regularity = null;
            AdditionalRegularity = null;
            while(CheckList.Count > 0) CheckList.RemoveAt(0);
            CheckList.CollectionChanged -= CheckList_CollectionChanged;
        }

        public void Dispose()
        {
            while (CheckList.Count > 0) CheckList.RemoveAt(0);
            CheckList.CollectionChanged -= CheckList_CollectionChanged;
        }
    }
}
