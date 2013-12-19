using System;

namespace BitPayAPI
{
    /// <summary>
    /// Provides an API specific exception handler.
    /// </summary>
    public class BitPayException : Exception
    {
        private string _message = "Exception information not provided";
        private Exception _inner = null;

        /// <summary>
        /// Constructor.  Creates an empty exception.
        /// </summary>
        public BitPayException()
        {
        }

        /// <summary>
        /// Constructor.  Creates an exception with a message only.
        /// </summary>
        /// <param name="message">The message text for the exception.</param>
        public BitPayException(string message) : base(message)
        {
            _message = message;
        }

        /// <summary>
        /// Constructor.  Creates an exception with a message and root cause exception.
        /// </summary>
        /// <param name="message">The message text for the exception.</param>
        /// <param name="inner">The root cause exception.</param>
        public BitPayException(string message, Exception inner) : base(message, inner)
        {
            _message = message;
            _inner = inner;
        }

        /// <summary>
        /// The exception message text.
        /// </summary>
        /// <returns>The exception message text.</returns>
        public string getMessage()
        {
            return _message;
        }

        /// <summary>
        /// The root cause exception.
        /// </summary>
        /// <returns>The root cause exception.</returns>
        public Exception getInner()
        {
            return _inner;
        }
    }
}