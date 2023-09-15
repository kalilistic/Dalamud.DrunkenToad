namespace Dalamud.DrunkenToad.Caching;

using Collections;

/// <summary>
/// An abstract base class for cache services that manage thread-safe caching operations with sorted collections.
/// This class extends <see cref="CacheService"/> and provides a sorted cache collection for managing cached items.
/// </summary>
/// <typeparam name="T">The type of items to be cached.</typeparam>
public abstract class SortedCacheService<T> : CacheService where T : notnull
{
    protected ThreadSafeSortedCollection<T> cache = null!;
}
