namespace Dalamud.DrunkenToad.Collections;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/// <summary>
/// Represents a thread-safe collection of items.
/// This class encapsulates a ConcurrentDictionary for thread-safe add and remove operations,
/// and provides additional methods that use locks for compound operations.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the collection.</typeparam>
/// <typeparam name="TValue">The type of the values in the collection.</typeparam>
public class ThreadSafeCollection<TKey, TValue> : IDisposable where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TValue> items;
    private readonly ReaderWriterLockSlim rwLock;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadSafeCollection{TKey, TValue}"/> class.
    /// </summary>
    public ThreadSafeCollection()
    {
        this.items = new ConcurrentDictionary<TKey, TValue>();
        this.rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadSafeCollection{TKey, TValue}"/> class with an initial set of items.
    /// </summary>
    /// <param name="initialItems">A dictionary containing the initial items for the collection.</param>
    public ThreadSafeCollection(Dictionary<TKey, TValue> initialItems)
    {
        this.items = new ConcurrentDictionary<TKey, TValue>(initialItems);
        this.rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    }

    /// <summary>
    /// Adds a range of key-value pairs to the collection.
    /// </summary>
    /// <param name="itemsToAdd">The key-value pairs to add to the collection.</param>
    /// <returns>The number of items successfully added to the collection.</returns>
    public int AddRange(IEnumerable<KeyValuePair<TKey, TValue>> itemsToAdd)
    {
        var addedCount = 0;
        this.rwLock.EnterWriteLock();
        try
        {
            foreach (var kvp in itemsToAdd)
            {
                if (this.items.TryAdd(kvp.Key, kvp.Value))
                {
                    addedCount++;
                }
            }
        }
        finally
        {
            this.rwLock.ExitWriteLock();
        }

        return addedCount;
    }

    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    /// <param name="key">The key of the item.</param>
    /// <param name="value">The value of the item.</param>
    /// <returns>True if the item was added successfully; otherwise, false.</returns>
    public bool Add(TKey key, TValue value) => this.items.TryAdd(key, value);

    /// <summary>
    /// Removes an item from the collection.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <returns>True if the item was removed successfully; otherwise, false.</returns>
    public bool Remove(TKey key) => this.items.TryRemove(key, out _);

    /// <summary>
    /// Gets an item from the collection.
    /// </summary>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <returns>The value associated with the specified key, or the default value of TValue if the key does not exist.</returns>
    public TValue? Get(TKey key)
    {
        this.items.TryGetValue(key, out var value);
        return value;
    }

    /// <summary>
    /// Gets all items from the collection as a list.
    /// </summary>
    /// <returns>A list containing all items in the collection.</returns>
    public List<TValue> GetAll()
    {
        this.rwLock.EnterReadLock();
        try
        {
            return this.items.Values.ToList();
        }
        finally
        {
            this.rwLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Updates an existing item in the collection.
    /// </summary>
    /// <param name="key">The key of the item to update.</param>
    /// <param name="newValue">The new value to set.</param>
    /// <returns>True if the item was updated successfully; otherwise, false.</returns>
    public bool Update(TKey key, TValue newValue)
    {
        if (!this.items.TryGetValue(key, out var existingValue))
        {
            return false;
        }

        return this.items.TryUpdate(key, newValue, existingValue);
    }

    /// <summary>
    /// Gets the count of items in the collection.
    /// </summary>
    /// <returns>The number of items in the collection.</returns>
    public int Count() => this.items.Count;

    /// <summary>
    /// Clears all items from the collection.
    /// </summary>
    public void Clear()
    {
        this.rwLock.EnterWriteLock();
        try
        {
            this.items.Clear();
        }
        finally
        {
            this.rwLock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Finds the first item in the collection that satisfies a specified condition.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>The first element that satisfies the condition, or the default value of TValue if no such element is found.</returns>
    public TValue? FindFirst(Func<TValue, bool> predicate)
    {
        this.rwLock.EnterReadLock();
        try
        {
            return this.items.Values.FirstOrDefault(predicate);
        }
        finally
        {
            this.rwLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Finds all items in the collection that satisfy a specified condition.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>A list of all elements that satisfy the condition.</returns>
    public List<TValue> FindAll(Func<TValue, bool> predicate)
    {
        this.rwLock.EnterReadLock();
        try
        {
            return this.items.Values.Where(predicate).ToList();
        }
        finally
        {
            this.rwLock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.rwLock.Dispose();
        GC.SuppressFinalize(this);
    }
}