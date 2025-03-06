namespace MyHelper.Infrastructure.Extensions
{
    internal static class LinqExtensions
    {
        public static IOrderedQueryable<T> Sort<T>(this IQueryable<T> source, 
                                                    System.Linq.Expressions.Expression<Func<T, object>> func, bool ascending)
            => ascending
                ? source.OrderBy(func)
                : source.OrderByDescending(func);
    }
}