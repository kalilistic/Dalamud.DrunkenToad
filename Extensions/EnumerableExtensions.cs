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

    /// <summary>
    /// Try to get the first element of a collection.
    /// </summary>
    /// <param name="values">collection to get first element from.</param>
    /// <param name="result">first element of collection.</param>
    /// <typeparam name="T">type of elements in collection.</typeparam>
    /// <returns>true if first element was found, false otherwise.</returns>
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

    /// <summary>
    /// Try to get the first element of a collection that matches a predicate.
    /// </summary>
    /// <param name="values">collection to get first element from.</param>
    /// <param name="predicate">predicate to match.</param>
    /// <param name="result">first element of collection.</param>
    /// <typeparam name="T">type of elements in collection.</typeparam>
    /// <returns>true if first element was found, false otherwise.</returns>
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
