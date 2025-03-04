using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.ObjectModel
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
