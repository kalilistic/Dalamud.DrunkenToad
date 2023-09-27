namespace Dalamud.DrunkenToad.Helpers;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Core;

/// <summary>
/// Wraps method calls with stopwatches to track performance.
/// </summary>
public static class PerformanceHelper
{
    /// <summary>
    /// Executes the provided action and logs the time taken for its execution.
    /// </summary>
    /// <param name="action">The action to be executed.</param>
    /// <param name="description">The description.</param>
    public static void ExecuteAndLog(Action action, string description = "")
    {
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        DalamudContext.PluginLog.Info(string.IsNullOrEmpty(description)
            ? $"Time taken: {stopwatch.ElapsedMilliseconds} ms"
            : $"[{description}] Time taken: {stopwatch.ElapsedMilliseconds} ms");
    }

    /// <summary>
    /// Executes the provided asynchronous action and logs the time taken for its execution.
    /// </summary>
    /// <param name="asyncAction">The asynchronous action to be executed.</param>
    /// <param name="description">The description.</param>
    /// <returns>The result of the function execution.</returns>
    public static async Task ExecuteAndLogAsync(Func<Task> asyncAction, string description = "")
    {
        var stopwatch = Stopwatch.StartNew();
        await asyncAction();
        stopwatch.Stop();
        DalamudContext.PluginLog.Info(string.IsNullOrEmpty(description)
            ? $"Time taken: {stopwatch.ElapsedMilliseconds} ms"
            : $"[{description}] Time taken: {stopwatch.ElapsedMilliseconds} ms");
    }

    /// <summary>
    /// Executes the provided function and logs the time taken for its execution.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The function to be executed.</param>
    /// <param name="description">The description.</param>
    /// <returns>The result of the function execution.</returns>
    public static TResult ExecuteAndLog<TResult>(Func<TResult> func, string description = "")
    {
        var stopwatch = Stopwatch.StartNew();
        var result = func();
        stopwatch.Stop();
        DalamudContext.PluginLog.Info(string.IsNullOrEmpty(description)
            ? $"Time taken: {stopwatch.ElapsedMilliseconds} ms"
            : $"[{description}] Time taken: {stopwatch.ElapsedMilliseconds} ms");

        return result;
    }

    /// <summary>
    /// Executes the provided asynchronous function and logs the time taken for its execution.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="asyncFunc">The asynchronous function to be executed.</param>
    /// <param name="description">The description.</param>
    /// <returns>The result of the function execution.</returns>
    public static async Task<TResult> ExecuteAndLogAsync<TResult>(Func<Task<TResult>> asyncFunc, string description = "")
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await asyncFunc();
        stopwatch.Stop();
        DalamudContext.PluginLog.Info(string.IsNullOrEmpty(description)
            ? $"Time taken: {stopwatch.ElapsedMilliseconds} ms"
            : $"[{description}] Time taken: {stopwatch.ElapsedMilliseconds} ms");
        return result;
    }
}
