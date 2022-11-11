using System.Collections.Generic;
using System.Linq;

namespace Dalamud.DrunkenToad.Extension;

/// <summary>
/// IEnumerable extensions.
/// </summary>
public static class IEnumerableExtensions
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
}
