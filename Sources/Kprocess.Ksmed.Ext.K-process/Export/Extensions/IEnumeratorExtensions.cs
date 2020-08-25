using System.Collections;
using System.Collections.Generic;

namespace KProcess.Ksmed.Ext.Kprocess.Export
{
    public static class IEnumeratorExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator enumerator)
            where T : class
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current as T;
        }
    }
}
    