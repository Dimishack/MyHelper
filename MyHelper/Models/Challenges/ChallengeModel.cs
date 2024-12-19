using MyHelper.DAL.Entyties;

namespace MyHelper.Models.Challenges
{
    internal class ChallengeModel(Challenge challenge)
    {
        public readonly Challenge _challenge = challenge;

        public int Id => _challenge.Id;
        public string Name { get => _challenge.Name; set => _challenge.Name = value; }
        public string? Note { get => _challenge.Note; set => _challenge.Note = value; }
        public bool InProgress => _challenge.InProgress;

    }
}
