using MyHelper.DAL.Entyties;
using MyHelper.Models.Enums;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace MyHelper.Models.Challenges
{
    internal class ChallengeModel
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
                if(DateStart.HasValue && DateEnd.HasValue)
                {
                    var dateToday = _dateToday;
                    if (dateToday >= DateStart.Value && dateToday <= DateEnd.Value)
                        return DateEnd.Value.DayOfYear - DateStart.Value.DayOfYear;
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
                    var dateToday = _dateToday;
                    if (dateToday < DateStart.Value) return ChallengeStatus.Ready;
                    if (dateToday > DateEnd.Value) return ChallengeStatus.Success;
                    return ChallengeStatus.Progress; 
                }
                return 0;
            }
        }

        public ObservableCollection<CheckModel> CheckList { get; } = [];


        public void StartChallenge(ChallengeModelStart challenge)
        {
            InProgress = true;
            DateStart = challenge.DateStart;
            DateEnd = challenge.DateEnd;
            Regularity = challenge.Regularity;
            if (challenge.Regularity == "По дням недели")
            {
                var result = 0;
                var pow = 0;
                for (int i = 0; i < challenge.DaysOfWeek.Length; i++)
                {
                    if (challenge.DaysOfWeek[i])
                    {

                        result += i * (int)Math.Pow(10, pow++);
                    }
                }
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
        }

        public void StopChallenge()
        {
            InProgress = false;
            DateStart = null;
            DateEnd = null;
            Regularity = null;
            AdditionalRegularity = null;
        }

    }
}
