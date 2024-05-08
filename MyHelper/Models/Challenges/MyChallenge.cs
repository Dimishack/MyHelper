using MyHelper.ViewModels.Base;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MyHelper.Models.Challenges
{
    internal class MyChallenge : ViewModel
    {
        private bool _isProgress;
        [JsonProperty("Выполняется?")]
        public bool IsProgress
        {
            get => _isProgress;
            set => Set(ref _isProgress, value);
        }
        private string? _challenge;
        [JsonProperty("Челлендж")]
        public string? Challenge { get => _challenge; set => Set(ref _challenge, value); }

        [JsonProperty("Начало челленджа")]
        public string? DateStartProgressing { get; set; }

        [JsonProperty("Примечание")]
        public string? Note { get; set; }

        [JsonProperty("Чек-лист")]
        public IList<Checklist>? Checklist { get; set; }
    }
}
