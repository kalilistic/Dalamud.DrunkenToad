namespace Dalamud.DrunkenToad.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// IEnumerable extensions.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Split list into sub lists of equal size.
    /// </summary>
    /// <param name="list">list to divide.</param>
    /// <param name="parts">number of sub lists.</param>
    /// <typeparam name="T">type of elements in collection.</typeparam>
    /// <returns>list of sub lists.</returns>
    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
    {
        var i = 0;
        var splits = from item in list
            group item by i++ % parts into part
            select part.AsEnumerable();
        return splits;
    }

    public static bool TryGetFirst<T>(this IEnumerable<T> values, out T result) where T : struct
    {
        using var e = values.GetEnumerator();
        if (e.MoveNext())
        {
            result = e.Current;
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryGetFirst<T>(this IEnumerable<T> values, Predicate<T> predicate, out T result) where T : struct
    {
        using var e = values.GetEnumerator();
        while (e.MoveNext())
        {
            if (!predicate(e.Current))
            {
                continue;
            }

            result = e.Current;
            return true;
        }

        result = default;
        return false;
    }
}
