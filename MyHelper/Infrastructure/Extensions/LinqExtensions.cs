namespace MyHelper.Infrastructure.Extensions
{
    internal static class LinqExtensions
    {
        public static IEnumerable<T> Sort<T, TKey>(this IEnumerable<T> source, Func<T, TKey> func, bool ascending)
            => ascending ? source.OrderBy(func) : source.OrderByDescending(func);
    }
}
