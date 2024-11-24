using MyHelper.DAL.Entyties;
using MyHelper.Models.Base;
using System.Collections.ObjectModel;

namespace MyHelper.Models.Challenges
{
    internal class ChallengeModel : BaseModel
    {
        private readonly DateTime _dateToday = DateTime.Today;
        private readonly Challenge _challenge;
        public ChallengeModel(Challenge challenge)
        {
            _challenge = challenge;
            foreach (var check in challenge.CheckList)
            {
                CheckList.Add(new CheckModel(check));
            }
        }

        public int Id => _challenge.Id;
        public string Name { get => _challenge.Name; set => _challenge.Name = value; }
        public string? Note { get => _challenge.Note; set => _challenge.Note = value; }
        public string? Duration { get => _challenge.Duration; set => _challenge.Duration = value; }

        public DateOnly? DateStart
        {
            get => _challenge.Start.HasValue ? DateOnly.FromDateTime((DateTime)_challenge.Start) : null;
            set => _challenge.Start = value.HasValue ? value.Value.ToDateTime(TimeOnly.MinValue) : null;
        }
        public DateOnly? DateEnd
        {
            get => _challenge.End.HasValue ? DateOnly.FromDateTime((DateTime)_challenge.End) : null;
            set => _challenge.End = value.HasValue ? value.Value.ToDateTime(TimeOnly.MinValue) : null;
        }

        public bool InProgress
        {
            get => _challenge.InProgress;
            set
            {
                if (value == _challenge.InProgress) return;
                _challenge.InProgress = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DateStart));
                OnPropertyChanged(nameof(DateEnd));
                OnPropertyChanged(nameof(DaysLeft));
                OnPropertyChanged(nameof(Status));
            }
        }

        public int? DaysLeft
        {
            get
            {
                if(DateStart.HasValue && DateEnd.HasValue)
                {
                    var dateToday = DateOnly.FromDateTime(_dateToday);
                    if (dateToday >= DateStart.Value && dateToday <= DateEnd.Value)
                        return DateEnd.Value.DayNumber - DateStart.Value.DayNumber;
                }
                return null;
            }
        }

        public string? Status
        {
            get
            {
                if (DateStart.HasValue && DateEnd.HasValue)
                {
                    DateOnly dateToday = DateOnly.FromDateTime(_dateToday);
                    if (dateToday < DateStart.Value) return "Подготовка";
                    if (dateToday > DateEnd.Value) return "Завершение";
                    return "Выполнение"; 
                }
                return null;
            }
        }

        public int? Regularity_CountDay
        {
            get => _challenge.Regularity_CountDay;
            set => _challenge.Regularity_CountDay = value;
        }

        public string? Regularity_UnitCalendr
        {
            get => _challenge.Regularity_UnitCalendar;
            set => _challenge.Regularity_UnitCalendar = value;
        }

        public ObservableCollection<CheckModel> CheckList { get; } = [];

    }
}
