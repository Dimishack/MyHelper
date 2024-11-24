using MyHelper.DAL.Entyties;
using MyHelper.Models.Base;

namespace MyHelper.Models.Challenges
{
    internal class CheckModel(Check check) : BaseModel
    {
        private readonly Check _check = check;

        public int Id => _check.Id;
        public int NumberDay { get => _check.NumberDay; set => _check.NumberDay = value; }
        public DayOfWeek WeekDay => Date.DayOfWeek;
        public bool Checked { get => _check.Checked; set => _check.Checked = value; }
        public DateOnly Date { get => DateOnly.FromDateTime(_check.Date); set => _check.Date = value.ToDateTime(TimeOnly.MinValue); }
    }
}
