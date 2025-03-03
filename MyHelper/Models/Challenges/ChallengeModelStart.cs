using MyHelper.Models.Base;
using MyHelper.Models.Enums;
using System.Collections.ObjectModel;

namespace MyHelper.Models.Challenges
{
    class ChallengeModelStart : BaseModel
    {
        private int _currentDayDuration = 0;
        private DateTime _currentDateStart;
        private readonly DateTime TODAY = DateTime.Today;
        private readonly DateTime DateLimitForYear;
        private readonly DateTime DateLimitForPregnancy;
        private readonly DateTime DateLimitForHalf;
        private readonly DateTime DateLimitForQuarter;
        private int Year => (DateTime.IsLeapYear(_dateStart.Year) ? 366 : 365) - 1;

        public ObservableCollection<string> Durations
        {
            get
            {
                ObservableCollection<string> result = [];
                result.Add("Месяц");
                if (_dateStart < DateLimitForQuarter)
                    result.Add("Квартал");
                if (_dateStart < DateLimitForHalf)
                    result.Add("Полгода");
                if (_dateStart < DateLimitForPregnancy)
                    result.Add("Беременность");
                if (_dateStart < DateLimitForYear)
                    result.Add("Год");
                return result;
            }
        }

        private DateTime _dateStart = new(DateTime.Today.Year + 2, 1, 1);
        public DateTime DateStart
        {
            get => _dateStart;
            set
            {
                if (_dateStart != value)
                {
                    _dateStart = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Durations));
                    if (_duration == -1)
                        Duration = 0;
                    if (_currentDateStart.Month != value.Month)
                    {
                        _currentDateStart = value;
                        Duration = _duration;
                    }
                    else DateEnd = value.AddDays(_currentDayDuration);
                }
            }
        }

        private int _duration = 0;
        public int Duration
        {
            get => _duration;
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnPropertyChanged();
                }
                if (value != -1)
                {
                    _currentDayDuration = CurrentDayDuration((ChallengeDuration)value + 1, _dateStart);
                    DateEnd = _dateStart.AddDays(_currentDayDuration);
                }

            }
        }

        private DateTime _dateEnd;
        public DateTime DateEnd
        {
            get => _dateEnd;
            set
            {
                if (_dateEnd != value)
                {
                    _dateEnd = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DayCount));
                }
            }
        }

        public string[] Regularities { get; } = ["Каждый день"];
        //public string[] Regularities { get; } = ["Каждый день", "Через день", "По кол-ву дней в неделю", "По дням недели"];

        private int _regularity = 0;
        public int Regularity
        {
            get => _regularity;
            set
            {
                if (_regularity != value)
                {
                    _regularity = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool[] DaysOfWeek { get; set; } = new bool[7];
        public bool[] CountDay { get; set; } = [false, false, false, false, false, true];

        public int DayCount
        {
            get
            {
                int result = _dateEnd.DayOfYear - _dateStart.DayOfYear + 1;
                if (_dateEnd.Year > _dateStart.Year)
                    result += Year + 1;
                return result;
            }
        }

        public ChallengeModelStart()
        {
            int nextYear = TODAY.Year + 1;
            DateLimitForYear = new DateTime(nextYear, 1, 2);
            DateLimitForPregnancy = new DateTime(nextYear, 4, 2);
            DateLimitForHalf = new DateTime(nextYear, 7, 2);
            DateLimitForQuarter = new DateTime(nextYear, 10, 2);
        }

        public void ReturnToMainValues()
        {
            DateStart = DateTime.Today;
            Duration = 0;
            Regularity = 0;
            for (int i = 0; i < DaysOfWeek.Length; i++) DaysOfWeek[i] = false;
            for (int i = 0; i < CountDay.Length - 1; i++) CountDay[i] = false;
            CountDay[^1] = true;
        }

        private int CurrentDayDuration(ChallengeDuration duration, DateTime start)
        {
            int result = 0;
            switch (duration)
            {
                case ChallengeDuration.Month:
                    result = DateTime.DaysInMonth(start.Year, start.Month) - 1;
                    break;
                case ChallengeDuration.Quarter:
                case ChallengeDuration.HalfYear:
                case ChallengeDuration.Pregnancy:
                    int month = start.Month;
                    int year = start.Year;
                    int max = duration == ChallengeDuration.Quarter ? 3 : duration == ChallengeDuration.HalfYear ? 6 : 9;
                    for (int i = 0; i < max; i++)
                    {
                        if (month + 1 > 12)
                        {
                            year++;
                            month = 1;
                        }
                        result += DateTime.DaysInMonth(year, month);
                    }
                    result--;
                    break;
                case ChallengeDuration.Year:
                    result = Year;
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
