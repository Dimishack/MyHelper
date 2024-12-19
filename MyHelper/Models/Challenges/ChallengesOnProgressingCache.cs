namespace MyHelper.Models.Challenges
{
    internal struct ChallengesOnProgressingCache
    {
        const int COUNT = 100;
        private Dictionary<int, ChallengeOnProgressingModel> Cache { get; }
        public readonly int Count => Cache.Count;

        public ChallengesOnProgressingCache() => Cache = [];

        public readonly bool Add(ChallengeOnProgressingModel challenge)
            => Cache.Count + 1 <= COUNT && Cache.TryAdd(challenge.Id, challenge);

        public readonly bool Remove(ChallengeOnProgressingModel challenge) => Cache.Remove(challenge.Id);

        public readonly ChallengeOnProgressingModel? GetChallenge(int key)
            => Cache.TryGetValue(key, out var challenge) ? challenge : null;

        public readonly IList<ChallengeOnProgressingModel> GetChallenges() => [.. Cache.Values];
        public readonly IList<ChallengeOnProgressingModel> GetChallenges(Func<ChallengeOnProgressingModel, bool> predicate) 
            => [.. Cache.Values.Where(predicate)];
    }
}
