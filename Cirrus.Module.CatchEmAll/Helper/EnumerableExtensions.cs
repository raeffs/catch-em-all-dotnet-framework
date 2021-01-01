using System;
using System.Collections.Generic;

namespace Cirrus.Module.CatchEmAll.Helper
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> DefaultForMissingDate<T>(this IEnumerable<T> source, IEnumerable<DateTime> allDates, Func<T, DateTime> dateSelector, Func<DateTime, T> defaultItemFactory)
        {
            var iterator = source.GetEnumerator();
            iterator.MoveNext();

            foreach (var desiredDate in allDates)
            {
                if (iterator.Current != null && dateSelector(iterator.Current) == desiredDate)
                {
                    yield return iterator.Current;
                    iterator.MoveNext();

                    if (iterator.Current == null)
                    {
                        continue;
                    }

                    if (dateSelector(iterator.Current) == desiredDate)
                    {
                        throw new Exception("More than one item found in source collection with date " + desiredDate);
                    }
                }
                else
                {
                    yield return defaultItemFactory(desiredDate);
                }
            }
        }
    }
}
