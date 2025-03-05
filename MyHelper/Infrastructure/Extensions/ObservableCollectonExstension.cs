using System.Collections.ObjectModel;

namespace MyHelper.Infrastructure.Extensions
{
    internal static class ObservableCollectonExstension
    {
        public static void ClearAndAddElements<T>(this ObservableCollection<T> collection, IEnumerable<T> newElements)
        {
            collection.Clear();
            foreach (var element in newElements)
                collection.Add(element);
        }
    }
}
