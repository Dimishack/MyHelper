using Newtonsoft.Json;
using System.Collections;
using System.ComponentModel;

namespace MyHelper.Models.Challenges
{
    internal class MyChallenges
    {
        [JsonProperty("Группа")]
        public string? Group { get; set; }
        [JsonProperty("Челленджи")]
        public BindingList<MyChallenge>? ListChallenges { get; set; }
    }
}
