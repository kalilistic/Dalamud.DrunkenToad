using System;

using Dalamud.Plugin;

namespace DalamudPluginCommon
{
    /// <summary>
    /// Plugin logger.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Log verbose message.
        /// </summary>
        /// <param name="messageTemplate">message to log.</param>
        public static void LogVerbose(string messageTemplate)
        {
            PluginLog.LogVerbose(messageTemplate);
        }

        /// <summary>
        /// Log debug message.
        /// </summary>
        /// <param name="messageTemplate">message to log.</param>
        public static void LogDebug(string messageTemplate)
        {
            PluginLog.LogDebug(messageTemplate);
        }

        /// <summary>
        /// Log info message.
        /// </summary>
        /// <param name="messageTemplate">message to log.</param>
        public static void LogInfo(string messageTemplate)
        {
            PluginLog.Log(messageTemplate);
        }

        /// <summary>
        /// Log verbose message.
        /// </summary>
        /// <param name="messageTemplate">message to log.</param>
        /// <param name="values">object values to log.</param>
        public static void LogInfo(string messageTemplate, params object[] values)
        {
            PluginLog.Log(messageTemplate, values);
        }

        /// <summary>
        /// Log error message.
        /// </summary>
        /// <param name="messageTemplate">message to log.</param>
        public static void LogError(string messageTemplate)
        {
            PluginLog.LogError(messageTemplate);
        }

        /// <summary>
        /// Log error message.
        /// </summary>
        /// <param name="messageTemplate">message to log.</param>
        /// <param name="values">object values to log.</param>
        public static void LogError(string messageTemplate, params object[] values)
        {
            PluginLog.LogError(messageTemplate, values);
        }

        /// <summary>
        /// Log error message.
        /// </summary>
        /// <param name="exception">exception to log.</param>
        /// <param name="messageTemplate">message to log.</param>
        /// <param name="values">object values to log.</param>
        public static void LogError(Exception exception, string messageTemplate, params object[] values)
        {
            PluginLog.LogError(exception, messageTemplate, values);
        }
    }
}
