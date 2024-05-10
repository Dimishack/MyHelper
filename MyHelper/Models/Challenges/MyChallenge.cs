using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyHelper.Models.Challenges
{
    internal class MyChallenge : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private bool _isProgress;

        [JsonProperty("Выполняется?")]
        public bool IsProgress
        {
            get => _isProgress;
            set
            {
                if(Equals(value, _isProgress)) return;
                _isProgress = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("Челлендж")]
        public string? Challenge { get; set; }

        [JsonProperty("Продолжительность")]
        public string? Duration { get; set; }

        [JsonProperty("Начало челленджа")]
        public DateTime? DateStartProgressing { get; set; }

        [JsonProperty("Примечание")]
        public string? Note { get; set; }

        [JsonProperty("Чек-лист")]
        public IList<Checklist>? Checklist { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
