using MyHelper.ViewModels.Base;
using Newtonsoft.Json;
using System.ComponentModel;

namespace MyHelper.Models.Challenges
{
    internal class MyChallenges
    {
        [JsonProperty("Тип челленджей")]
        public string? ListName { get; set; }
        [JsonProperty("Челленджи")]
        public BindingList<MyChallenge>? ListChallenges { get; set; }
    }

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
        public IList<Checklist>? Checklist { get; set; }
    }
}
