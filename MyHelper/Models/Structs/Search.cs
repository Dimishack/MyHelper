namespace MyHelper.Models.Structs
{
    internal struct Search(string value, string additional, bool isSearch = false)
    {
        public bool IsSearch { get; private set; } = isSearch;
        public string Value { get; private set; } = value;
        public string Property { get; private set; } = additional;

        public static bool operator true(in Search search)
            => search.IsSearch;
        public static bool operator false(in Search search)
            => !search.IsSearch;

        public static bool operator !(in Search search)
            => !search.IsSearch;

        public static implicit operator bool(in Search search)
            => search.IsSearch;

        public void Set(bool isSearch, string value, string additionalValue)
        {
            IsSearch = isSearch;
            Value = value;
            Property = additionalValue;
        }
    }
}
