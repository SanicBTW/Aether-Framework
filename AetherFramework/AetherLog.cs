// in the beginning this was under AetherFramework.Backend, then Logging then ended up on the global scope lol

using System.Diagnostics;
using AetherFramework.Interfaces;

namespace AetherFramework
{
    /// <summary>
    /// Log levels supported by the framework.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug log level, only printed if running under a debug configuration.
        /// </summary>
        Debug,

        /// <summary>
        /// Informational log level.
        /// </summary>
        Info,

        /// <summary>
        /// Warn log level.
        /// </summary>
        Warn,

        /// <summary>
        /// Error log level, could throw an exception after logging.
        /// </summary>
        Error,
    }

    /// <summary>
    /// Simple utility class used for logging and monitoring the framework runtime.
    /// </summary>
    public class AetherLog(string scope, Action<string, LogLevel, string> writer) : IAetherLogger
    {
        private readonly string scope = scope;

        void IAetherLogger.Debug(string message) => writer(scope, LogLevel.Debug, message);
        void IAetherLogger.Info(string message) => writer(scope, LogLevel.Info, message);
        void IAetherLogger.Warn(string message) => writer(scope, LogLevel.Warn, message);
        void IAetherLogger.Error(string message) => writer(scope, LogLevel.Error, message);

        private static AetherLog instance { get; } = new("global", write);

        private static Action<string, LogLevel> logHandler = defaultHandler;

        /// <summary>
        /// Changes the default logging handler.
        /// </summary>
        /// <param name="handler">The new handler to apply globally, if null it resets back to the default implementation.</param>
        public static void ChangeLogHandler(Action<string, LogLevel>? handler = null)
            => logHandler = handler ?? defaultHandler;

        /// <summary>
        /// Creates a scoped logger for the given context.
        /// </summary>
        /// <param name="newScope">The target scope of the logger.</param>
        /// <returns>A freshly scoped logger.</returns>
        public static IAetherLogger CreateScoped(string newScope)
            => new AetherLog($"{instance.scope}/{newScope}", write);

        /// <inheritdoc cref="IAetherLogger.Debug"/>
        public static void Debug(string message) => ((IAetherLogger)instance).Debug(message);

        /// <inheritdoc cref="IAetherLogger.Info"/>
        public static void Info(string message) => ((IAetherLogger)instance).Info(message);

        /// <inheritdoc cref="IAetherLogger.Warn"/>
        public static void Warn(string message) => ((IAetherLogger)instance).Warn(message);

        /// <inheritdoc cref="IAetherLogger.Error"/>
        public static void Error(string message) => ((IAetherLogger)instance).Error(message);

        // this handles the formatting of the context, level and message and passing it down to the log handler, thats why its not gonna ever meet the same type
        private static void write(string context, LogLevel level, string message)
        {
            // maybe its not the best approach? but im compiling the framework in release so uhh i guess it should work properly
            if (level == LogLevel.Debug && !Debugger.IsAttached)
                return;

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string formatted = $"[{timestamp}] [{context}] [{level.ToString().ToUpper()}] {message}";
            logHandler.Invoke(formatted, level);
        }

        // this handles the logging into the handler, let it be a log file, visual ui or just the console
        private static void defaultHandler(string message, LogLevel logLevel)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = logLevel switch
            {
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Info  => ConsoleColor.White,
                LogLevel.Warn  => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                _ => ConsoleColor.White
            };

            try
            {
                Console.WriteLine(message);
            }
            finally
            {
                Console.ForegroundColor = oldColor;
            }
        }
    }
}
