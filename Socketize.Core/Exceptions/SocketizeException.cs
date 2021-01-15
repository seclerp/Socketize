using System;

namespace Socketize.Core.Exceptions
{
    /// <summary>
    /// Exception that fires when any Socketize-related error occurs.
    /// </summary>
    public class SocketizeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SocketizeException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public SocketizeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketizeException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Parent exception.</param>
        public SocketizeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}