// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UseCollectionExpression
namespace Dalamud.DrunkenToad.Collections;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/// <summary>
/// Represents a thread-safe sorted collection of items.
/// </summary>
/// <remarks>
/// The ConcurrentDictionary is used to ensure thread-safe insertion and deletion of items. It provides a fast and
/// efficient way to check if an item is already present in the collection and allows for safe manipulation of items
/// in a multi-threaded environment. The SortedSet is used for maintaining the sorted order of items in the collection.
/// It is used when you need to retrieve the items in a sorted manner. However, SortedSet is not thread-safe by itself,
/// so we must use synchronization mechanisms to ensure that it is manipulated in a safe way.
///
/// In this class, we use ReaderWriterLockSlim instead of simple locks for better performance and scalability when
/// there are frequent concurrent read operations and infrequent write operations. ReaderWriterLockSlim allows
/// multiple threads to read the data concurrently, which can improve performance in scenarios where read operations
/// significantly outnumber write operations. Simple locks, on the other hand, would block all threads except the one
/// holding the lock, even if they are only performing read operations.
///
/// By combining these two data structures and using ReaderWriterLockSlim for synchronization, the
/// ThreadSafeSortedCollection provides both thread safety and the ability to maintain a sorted collection of elements.
/// </remarks>
/// <typeparam name="T">The type of the items in the collection.</typeparam>
public class ThreadSafeSortedCollection<T> : IDisposable where T : notnull
{
    private readonly ConcurrentDictionary<T, byte> items;
    private readonly ReaderWriterLockSlim setLock;
    private IComparer<T> comparer;
    private SortedSet<T> sortedSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadSafeSortedCollection{T}"/> class without any initial data.
    /// </summary>
    /// <param name="lockRecursionPolicy">The lock recursion policy.</param>
    /// <remarks>
    /// Used for early initialization of the collection when the initial data is not yet available.
    /// </remarks>
    public ThreadSafeSortedCollection(LockRecursionPolicy lockRecursionPolicy = LockRecursionPolicy.NoRecursion)
    {
        this.setLock = new ReaderWriterLockSlim(lockRecursionPolicy);
        this.items = new ConcurrentDictionary<T, byte>();
        this.sortedSet = new SortedSet<T>();
        this.comparer = Comparer<T>.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadSafeSortedCollection{T}"/> class with the specified comparer.
    /// </summary>
    /// <param name="initialItems">initial items.</param>
    /// <param name="comparer">The comparer used to sort items in the collection.</param>
    /// <param name="lockRecursionPolicy">The lock recursion policy.</param>
    public ThreadSafeSortedCollection(IEnumerable<T> initialItems, IComparer<T> comparer, LockRecursionPolicy lockRecursionPolicy = LockRecursionPolicy.NoRecursion)
    {
        this.setLock = new ReaderWriterLockSlim(lockRecursionPolicy);
        var initialList = initialItems.ToList();
        this.items = new ConcurrentDictionary<T, byte>(initialList.Select(item => new KeyValuePair<T, byte>(item, 0)), new ComparerBasedEqualityComparer(comparer));
        this.sortedSet = new SortedSet<T>(initialList, comparer);
        this.comparer = comparer;
    }

    /// <summary>
    /// Gets the number of items in the collection.
    /// </summary>
    public int Count
    {
        get
        {
            this.setLock.EnterReadLock();
            try
            {
                return this.sortedSet.Count;
            }
            finally
            {
                this.setLock.ExitReadLock();
            }
        }
    }

    /// <summary>
    /// Converts the sorted set to a dictionary with integer keys representing the index of each item.
    /// </summary>
    /// <returns>A dictionary where the key is the index and the value is the item from the sorted set.</returns>
    public Dictionary<int, T> ToDictionary()
    {
        this.setLock.EnterReadLock();
        try
        {
            return this.sortedSet
                .Select((item, index) => new { Index = index, Item = item })
                .ToDictionary(x => x.Index, x => x.Item);
        }
        finally
        {
            this.setLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Converts the thread-safe sorted collection to a dictionary with integer keys and transformed values.
    /// </summary>
    /// <typeparam name="TTarget">The target type to which items in the collection will be transformed.</typeparam>
    /// <param name="transform">A function that transforms each item of type T to type TTarget.</param>
    /// <returns>A dictionary where the key is the index of the item in the sorted set, and the value is the transformed item of type TTarget.</returns>
    public Dictionary<int, TTarget> ToDictionary<TTarget>(Func<T, TTarget> transform)
    {
        this.setLock.EnterReadLock();
        try
        {
            return this.sortedSet
                .Select((item, index) => new { Index = index, Item = transform(item) })
                .ToDictionary(x => x.Index, x => x.Item);
        }
        finally
        {
            this.setLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Returns the index of the first occurrence of the specified item in the sorted collection.
    /// </summary>
    /// <remarks>
    /// This method performs a binary search to find the index, which has a time complexity of O(log n).
    /// </remarks>
    /// <param name="item">The item to locate in the collection.</param>
    /// <returns>The zero-based index of the first occurrence of the item in the collection if found; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        this.setLock.EnterReadLock();
        try
        {
            var index = 0;
            foreach (var entry in this.sortedSet)
            {
                if (this.sortedSet.Comparer.Compare(item, entry) == 0)
                {
                    return index;
                }

                index++;
            }
        }
        finally
        {
            this.setLock.ExitReadLock();
        }

        return -1;
    }

    /// <summary>
    /// Returns the count of items in the collection that match the specified filter expression.
    /// </summary>
    /// <param name="filter">The filter expression to apply.</param>
    /// <returns>The count of items in the collection that match the filter expression.</returns>
    public int GetFilteredItemsCount(Func<T, bool> filter)
    {
        this.setLock.EnterReadLock();
        try
        {
            return this.sortedSet.Count(filter);
        }
        finally
        {
            this.setLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>The index of the added item.</returns>
    public int Add(T item)
    {
        this.setLock.EnterWriteLock();
        try
        {
            var index = this.sortedSet.Count;
            if (this.items.TryAdd(item, (byte)index))
            {
                this.sortedSet.Add(item);
                return index;
            }
        }
        finally
        {
            this.setLock.ExitWriteLock();
        }

        return -1; // Return -1 if the item was not added
    }

    /// <summary>
    /// Adds a range of items to the collection.
    /// </summary>
    /// <param name="newItems">The items to add.</param>
    public void AddRange(IEnumerable<T> newItems)
    {
        foreach (var item in newItems)
        {
            this.Add(item);
        }
    }

    /// <summary>
    /// Removes an item from the collection.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>True if the item was removed, otherwise false.</returns>
    public bool Remove(T item)
    {
        if (this.items.TryRemove(item, out _))
        {
            this.setLock.EnterWriteLock();
            try
            {
                return this.sortedSet.Remove(item);
            }
            finally
            {
                this.setLock.ExitWriteLock();
            }
        }

        return false;
    }

    /// <summary>
    /// Updates an item in the collection by removing it, performing the specified action, and adding it back.
    /// </summary>
    /// <param name="item">The item to update.</param>
    /// <param name="updateAction">The action to perform on the item.</param>
    /// <returns>The new index of the updated item if the update was successful, otherwise -1.</returns>
    public int Update(T item, Action<T> updateAction)
    {
        this.setLock.EnterWriteLock();
        try
        {
            if (this.items.TryGetValue(item, out _) && this.sortedSet.Remove(item))
            {
                updateAction(item);

                // Find the new index for the updated item
                var newIndex = this.sortedSet.TakeWhile(x => this.comparer.Compare(x, item) < 0).Count();

                // Update the index in the ConcurrentDictionary
                this.items[item] = (byte)newIndex;

                // Add the item back to the sorted set
                this.sortedSet.Add(item);
                return newIndex;
            }
        }
        finally
        {
            this.setLock.ExitWriteLock();
        }

        return -1; // Return -1 if the item was not updated
    }

    /// <summary>
    /// Updates an item in the collection by removing it and adding it back.
    /// </summary>
    /// <param name="item">The item to update.</param>
    /// <returns>The new index of the updated item, or -1 if the item was not updated.</returns>
    public int Update(T item)
    {
        if (this.Remove(item))
        {
            var newIndex = this.Add(item);
            return newIndex;
        }

        return -1; // Return -1 if the item was not updated
    }

    /// <summary>
    /// Adds a new item to the collection or updates an existing one.
    /// </summary>
    /// <param name="item">The item to be added or updated.</param>
    /// <returns>The index of the added or updated item. If the item was not added or updated, it returns -1.</returns>
    public int AddOrUpdate(T item)
    {
        this.setLock.EnterWriteLock();
        try
        {
            if (this.items.ContainsKey(item))
            {
                // Update existing item
                if (this.Remove(item))
                {
                    var newIndex = this.Add(item);
                    return newIndex;
                }
                else
                {
                    return -1; // Return -1 if the item was not updated
                }
            }
            else
            {
                // Add new item
                var index = this.sortedSet.Count;
                if (this.items.TryAdd(item, (byte)index))
                {
                    this.sortedSet.Add(item);
                    return index;
                }
                else
                {
                    return -1; // Return -1 if the item was not added
                }
            }
        }
        finally
        {
            this.setLock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Finds the first item in the collection that matches the specified filter expression, and updates it by removing it,
    /// performing the specified action, and adding it back.
    /// </summary>
    /// <param name="filter">The filter expression to find the item.</param>
    /// <param name="updateAction">The action to perform on the found item.</param>
    /// <returns>The new index of the updated item if the item was found and updated, otherwise -1.</returns>
    public int UpdateFirstMatch(Func<T, bool> filter, Action<T> updateAction)
    {
        var item = this.FindFirst(filter);

        if (item is not null)
        {
            return this.Update(item, updateAction);
        }

        return -1;
    }

    /// <summary>
    /// Updates all items in the collection that match the specified filter expression by removing them, performing the specified action, and adding them back.
    /// </summary>
    /// <param name="filter">The filter expression to identify the items to update.</param>
    /// <param name="updateAction">The action to perform on the matching items.</param>
    /// <returns>The number of items updated.</returns>
    public int UpdateAllMatching(Func<T, bool> filter, Action<T> updateAction)
    {
        var matchingItems = this.FindAll(filter);
        var updatedItemsCount = 0;

        foreach (var item in matchingItems)
        {
            if (this.Remove(item))
            {
                updateAction(item);
                this.Add(item);
                updatedItemsCount++;
            }
        }

        return updatedItemsCount;
    }

    /// <summary>
    /// Returns the first item in the collection that matches the specified filter expression.
    /// </summary>
    /// <param name="filter">The filter expression to apply.</param>
    /// <returns>The first item in the collection that matches the filter expression, or null if no match is found.</returns>
    public T? FindFirst(Func<T, bool> filter)
    {
        this.setLock.EnterReadLock();
        try
        {
            return this.sortedSet.FirstOrDefault(filter);
        }
        finally
        {
            this.setLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Returns a list of items in the collection that match the specified filter expression.
    /// </summary>
    /// <param name="filter">The filter expression to apply.</param>
    /// <returns>A list of items in the collection that match the filter expression, or an empty list if no match is found.</returns>
    public IEnumerable<T> FindAll(Func<T, bool> filter)
    {
        this.setLock.EnterReadLock();
        try
        {
            return this.sortedSet.Where(filter).ToList();
        }
        finally
        {
            this.setLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Retrieves the items in the collection sorted according to the specified comparer.
    /// </summary>
    /// <returns>An enumerable of sorted items.</returns>
    public List<T> GetSortedItems()
    {
        this.setLock.EnterReadLock();
        try
        {
            return this.sortedSet.ToList();
        }
        finally
        {
            this.setLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Retrieves the items in the collection sorted according to the specified comparer.
    /// </summary>
    /// <param name="startIndex">start index.</param>
    /// <param name="pageSize">page size.</param>
    /// <returns>An enumerable of sorted items.</returns>
    public List<T> GetSortedItems(int startIndex, int pageSize)
    {
        this.setLock.EnterReadLock();
        try
        {
            return this.sortedSet.Skip(startIndex).Take(pageSize).ToList();
        }
        finally
        {
            this.setLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Retrieves a filtered and sorted collection of items of type T, based on the given filter, startIndex, and pageSize.
    /// </summary>
    /// <param name="filter">A function to test each item for a condition.</param>
    /// <param name="startIndex">The zero-based index of the first element in the filtered and sorted collection to retrieve.</param>
    /// <param name="pageSize">The number of elements to retrieve from the filtered and sorted collection.</param>
    /// <returns>An enumerable of the filtered and sorted items of type T.</returns>
    public List<T> GetFilteredSortedItems(Func<T, bool> filter, int startIndex, int pageSize)
    {
        this.setLock.EnterReadLock();
        try
        {
            return this.sortedSet.Where(filter).Skip(startIndex).Take(pageSize).ToList();
        }
        finally
        {
            this.setLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Retrieves the index of the specified item in the filtered and sorted collection, based on the given filter.
    /// </summary>
    /// <param name="item">The item to locate in the filtered and sorted collection.</param>
    /// <param name="filter">A function to test each item for a condition.</param>
    /// <returns>The zero-based index of the item in the filtered and sorted collection if found; otherwise, -1.</returns>
    public int GetFilteredSortedIndex(T item, Func<T, bool> filter)
    {
        this.setLock.EnterReadLock();
        try
        {
            var index = 0;
            foreach (var entry in this.sortedSet)
            {
                if (!filter(entry))
                {
                    continue;
                }

                if (this.sortedSet.Comparer.Compare(item, entry) == 0)
                {
                    return index;
                }

                index++;
            }
        }
        finally
        {
            this.setLock.ExitReadLock();
        }

        return -1;
    }

    /// <summary>
    /// Returns a subset of the sorted collection based on the specified filter expression.
    /// </summary>
    /// <param name="filter">The filter expression to apply.</param>
    /// <returns>A subset of the sorted collection based on the filter expression.</returns>
    public List<T> GetFilteredSortedItems(Func<T, bool> filter)
    {
        List<T> filteredItems;

        this.setLock.EnterReadLock();
        try
        {
            filteredItems = new List<T>(this.sortedSet.Where(filter));
        }
        finally
        {
            this.setLock.ExitReadLock();
        }

        return filteredItems;
    }

    /// <summary>
    /// Resorts the sorted set using the specified new comparer.
    /// </summary>
    /// <param name="newComparer">The new comparer to be used for sorting the set.</param>
    public void Resort(IComparer<T> newComparer)
    {
        this.setLock.EnterWriteLock();
        try
        {
            this.comparer = newComparer;
            var newSortedSet = new SortedSet<T>(this.sortedSet, newComparer);
            this.sortedSet = newSortedSet;
        }
        finally
        {
            this.setLock.ExitWriteLock();
        }
    }

    public void Dispose()
    {
        this.setLock.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Represents an equality comparer that uses an IComparer to compare items for equality.
    /// </summary>
    private sealed class ComparerBasedEqualityComparer : IEqualityComparer<T>
    {
        private readonly IComparer<T> comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparerBasedEqualityComparer"/> class with the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer used to compare items for equality.</param>
        public ComparerBasedEqualityComparer(IComparer<T> comparer) => this.comparer = comparer;

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>True if the specified objects are equal, otherwise false.</returns>
        public bool Equals(T? x, T? y) => this.comparer.Compare(x, y) == 0;

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(T obj) => obj.GetHashCode();
    }
}
