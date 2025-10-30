namespace AetherFramework.Interfaces
{
    /// <summary>
    /// Interface that is required to implement custom logging context's for <see cref="AetherLog"/>.
    /// </summary>
    public interface IAetherLogger
    {
        /// <summary>
        /// Log an informational message to the log context.
        /// </summary>
        /// <param name="message">The information to log.</param>
        void Info(string message);

        /// <summary>
        /// Log a warning message to the log context.
        /// </summary>
        /// <param name="message">The warn to log.</param>
        void Warn(string message);

        /// <summary>
        /// Log an error message to the log context.
        /// </summary>
        /// <param name="message">The error to log.</param>
        void Error(string message);

        /// <summary>
        /// Log a debug message to the log context.
        /// <remarks>
        /// Under the default implementation of this interface, the <paramref name="message"/> will be
        /// only printed if the program is built on debug configuration.
        /// </remarks>
        /// </summary>
        /// <param name="message">The debug message to log.</param>
        void Debug(string message);
    }
}
