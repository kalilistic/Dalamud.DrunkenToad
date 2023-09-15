namespace Dalamud.DrunkenToad.Caching;

using Collections;

/// <summary>
/// An abstract base class for cache services that manage thread-safe caching operations with unsorted collections.
/// This class extends <see cref="CacheService"/> and provides an unsorted cache collection for managing cached items.
/// </summary>
/// <typeparam name="T">The type of items to be cached.</typeparam>
public abstract class UnsortedCacheService<T> : CacheService where T : notnull
{
    protected ThreadSafeCollection<int, T> cache = null!;
}
