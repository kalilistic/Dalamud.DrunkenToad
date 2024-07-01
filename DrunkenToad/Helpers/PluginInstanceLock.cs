namespace Dalamud.DrunkenToad.Helpers;

using System;
using System.IO;
using Core;

/// <summary>
/// Provides functionality to acquire and release a lock file, allowing features to be restricted to a single instance of the plugin.
/// </summary>
public static class PluginInstanceLock
{
    private static FileStream? lockFile;
    private static string? lockFilePath;
    private static bool isInitialized;

    /// <summary>
    /// Acquires a lock to prevent multiple instances of the plugin from running.
    /// </summary>
    /// <returns>True if the lock was acquired successfully; otherwise, false.</returns>
    public static bool AcquireLock()
    {
        Initialize();
        if (TryDeleteExistingLockFile())
        {
            return TryCreateAndLockFile();
        }

        DalamudContext.PluginLog.Warning("Failed to delete existing lock file. Another instance might be running.");
        return false;
    }

    /// <summary>
    /// Releases the lock.
    /// </summary>
    public static void ReleaseLock() => UnlockAndDisposeFile();

    private static void Initialize()
    {
        if (isInitialized)
        {
            return;
        }

        lockFilePath = Path.Combine(DalamudContext.PluginInterface.GetPluginConfigDirectory(), "lockfile");
        isInitialized = true;
    }

    private static bool TryDeleteExistingLockFile()
    {
        if (!File.Exists(lockFilePath))
        {
            return true;
        }

        try
        {
            File.Delete(lockFilePath);
            DalamudContext.PluginLog.Debug("Existing lock file deleted.");
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }

    private static bool TryCreateAndLockFile()
    {
        try
        {
            lockFile = new FileStream(lockFilePath!, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
            lockFile.Lock(0, 0);
            DalamudContext.PluginLog.Debug("Instance is running. Lock acquired.");
            return true;
        }
        catch (IOException)
        {
            DalamudContext.PluginLog.Warning("Another instance is already running.");
            return false;
        }
    }

    private static void UnlockAndDisposeFile()
    {
        try
        {
            if (lockFile == null)
            {
                return;
            }

            lockFile.Unlock(0, 0);
            lockFile.Close();
            lockFile.Dispose();
            DalamudContext.PluginLog.Debug("Lock released and lock file deleted.");
        }
        catch (Exception ex)
        {
            DalamudContext.PluginLog.Warning($"Error releasing lock file: {ex.Message}");
        }
    }
}
