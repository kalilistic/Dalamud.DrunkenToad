namespace Dalamud.DrunkenToad.Caching;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core;

/// <summary>
/// An abstract base class for cache services that manage thread-safe caching operations.
/// This class provides a framework for efficiently handling cache updates and reset operations
/// while ensuring thread safety. It includes a queue (<see cref="pendingOperations"/>) to store
/// pending cache operations, allowing them to be executed once cache resetting is complete.
/// </summary>
public abstract class CacheService : IDisposable
{
    private readonly ReaderWriterLockSlim resetLock = new ();
    private readonly Queue<Action> pendingOperations = new ();
    private volatile bool isResettingCache;

    /// <summary>
    /// Event triggered when the cache is updated or changed.
    /// </summary>
    public event Action? CacheUpdated;

    /// <summary>
    /// Disposes of the resources used by the cache service.
    /// </summary>
    public void Dispose()
    {
        this.resetLock.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Invokes the CacheUpdated event. Derived classes should call this method
    /// to signal that the cache has been updated.
    /// </summary>
    protected void OnCacheUpdated() => this.CacheUpdated?.Invoke();

    /// <summary>
    /// Executes the specified operation immediately or enqueues it for later execution.
    /// </summary>
    /// <param name="operation">The operation to execute or enqueue.</param>
    protected void ExecuteOrEnqueue(Action operation)
    {
        if (this.isResettingCache)
        {
            this.resetLock.EnterReadLock();
            try
            {
                this.pendingOperations.Enqueue(operation);
            }
            finally
            {
                this.resetLock.ExitReadLock();
            }
        }
        else
        {
            operation();
        }
    }

    /// <summary>
    /// Reloads the cache and optionally executes a custom action before processing pending operations.
    /// </summary>
    /// <param name="customAction">An optional custom action to execute before processing pending operations.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task ExecuteReloadCacheAsync(Func<Task> customAction)
    {
        if (this.isResettingCache)
        {
            DalamudContext.PluginLog.Verbose("A cache reset is already in progress. Ignoring this request.");
            return;
        }

        await customAction.Invoke();

        while (this.pendingOperations.TryDequeue(out var operation))
        {
            operation();
        }

        this.isResettingCache = false;
        this.CacheUpdated?.Invoke();
    }

    /// <summary>
    /// Reloads the cache and optionally executes a custom action before processing pending operations.
    /// </summary>
    /// <param name="customAction">An optional custom action to execute before processing pending operations.</param>
    protected void ExecuteReloadCache(Action customAction)
    {
        if (this.isResettingCache)
        {
            DalamudContext.PluginLog.Verbose("A cache reset is already in progress. Ignoring this request.");
            return;
        }

        customAction.Invoke();

        while (this.pendingOperations.TryDequeue(out var operation))
        {
            operation();
        }

        this.isResettingCache = false;
        this.CacheUpdated?.Invoke();
    }
}
