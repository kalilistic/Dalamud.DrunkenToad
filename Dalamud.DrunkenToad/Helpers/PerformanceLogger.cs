namespace Dalamud.DrunkenToad.Helpers;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Logging;

/// <summary>
/// Wraps method calls with stopwatches to track performance.
/// </summary>
public static class PerformanceHelper
{
    /// <summary>
    /// Executes the provided action and logs the time taken for its execution.
    /// </summary>
    /// <param name="action">The action to be executed.</param>
    public static void ExecuteAndLog(Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        PluginLog.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
    }

    /// <summary>
    /// Executes the provided asynchronous action and logs the time taken for its execution.
    /// </summary>
    /// <param name="asyncAction">The asynchronous action to be executed.</param>
    /// <returns>The result of the function execution.</returns>
    public static async Task ExecuteAndLogAsync(Func<Task> asyncAction)
    {
        var stopwatch = Stopwatch.StartNew();
        await asyncAction();
        stopwatch.Stop();
        PluginLog.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
    }

    /// <summary>
    /// Executes the provided function and logs the time taken for its execution.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The function to be executed.</param>
    /// <returns>The result of the function execution.</returns>
    public static TResult ExecuteAndLog<TResult>(Func<TResult> func)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = func();
        stopwatch.Stop();
        PluginLog.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
        return result;
    }

    /// <summary>
    /// Executes the provided asynchronous function and logs the time taken for its execution.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="asyncFunc">The asynchronous function to be executed.</param>
    /// <returns>The result of the function execution.</returns>
    public static async Task<TResult> ExecuteAndLogAsync<TResult>(Func<Task<TResult>> asyncFunc)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await asyncFunc();
        stopwatch.Stop();
        PluginLog.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
        return result;
    }
}
