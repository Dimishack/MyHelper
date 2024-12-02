using MyHelper.Models.Base;
using System.Collections.ObjectModel;

namespace MyHelper.Models.Challenges
{
    class Challenge_Start : BaseModel
    {
        private readonly DateTime TODAY = DateTime.Today;
        private int _month => DateTime.DaysInMonth(_dateStart.Year, _dateStart.Month) - 1;
        private int _quarter
        {
            get
            {
                int year = _dateStart.Year;
                int month = _dateStart.Month;
                int day = DateTime.DaysInMonth(year, month); ;
                for (int i = 0; i < 2; i++)
                {
                    if (month + 1 > 12)
                    {
                        year++;
                        month = 1;
                    }
                    day += DateTime.DaysInMonth(year, month);
                    month++;
                }
                return day - 1;
            }
        }
        private int _half => _year / 2 - 1;
        private int _pregnancy => (int)(_year * 0.75) - 1;
        private int _year => (DateTime.IsLeapYear(_dateStart.Year) ? 366 : 365) - 1;
        public ObservableCollection<string> Durations
        {
            get
            {
                DateTime lastDayInYear = new(_dateStart.Year, 12, 31);
                ObservableCollection<string> result = [];
                result.Add("Месяц");
                int difference = lastDayInYear.DayOfYear - _dateStart.DayOfYear;
                if (difference >= _quarter)
                    result.Add("Квартал");
                if (difference >= _half)
                    result.Add("Полгода");
                if (difference >= _pregnancy)
                    result.Add("Беременность");
                if (difference >= _year)
                    result.Add("Год");
                return result;
            }
        }

        private DateTime _dateStart = new(DateTime.Today.Year + 1, 1, 1);
        public DateTime DateStart
        {
            get => _dateStart;
            set
            {
                if (_dateStart != value)
                {
                    _dateStart = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DayCount));
                    OnPropertyChanged(nameof(Durations));

                }
            }
        }

        private string _duration = "Гхы";
        public string Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged();
                switch (value)
                {
                    case "Месяц":
                        DateEnd = DateStart.AddDays(_month);
                        break;
                    case "Квартал":
                        DateEnd = DateStart.AddDays(_quarter);
                        break;
                    case "Полгода":
                        DateEnd = DateStart.AddDays(_half);
                        break;
                    case "Беременность":
                        DateEnd = DateStart.AddDays(_pregnancy);
                        break;
                    case "Год":
                        DateEnd = DateStart.AddDays(_year);
                        break;
                    default:
                        break;
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

        public string[] Regularities { get; } = ["Каждый день", "Через день", "Кол-во дней в неделю", "По дням недели"];

        private string _regularity = "Каждый день";
        public string Regularity
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

        public int DayCount => _dateEnd.DayOfYear - _dateStart.DayOfYear + 1;


        public void ReturnToMainValues()
        {
            var day2DecemberOfYear = DateTime.IsLeapYear(TODAY.Year) ? 337 : 336;
            DateStart = day2DecemberOfYear - TODAY.DayOfYear > 0 ? TODAY : new DateTime(TODAY.Year + 1, 1, 1);
            Duration = "Месяц";
            Regularity = "Каждый день";
            for (int i = 0; i < DaysOfWeek.Length; i++) DaysOfWeek[i] = false;
            for (int i = 0; i < CountDay.Length - 1; i++) CountDay[i] = false;
            CountDay[^1] = true;
        }
    }
}
