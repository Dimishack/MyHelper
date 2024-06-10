using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    internal static class DictionaryExtensions
    {
        public static IList<string> GetKeys(this Dictionary<string, int> dictionary) => new List<string>(dictionary.Keys);
    }
}
