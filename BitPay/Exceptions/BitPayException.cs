using System;

namespace BitPaySDK.Exceptions
{
    /// <summary>
    ///     Provides an API specific exception handler.
    /// </summary>
    public class BitPayException : Exception
    {
        private const string BitPayMessage = "Unexpected Bitpay exeption";
        protected string BitpayCode = "BITPAY-GENERIC";

        public BitPayException() : base(BitPayMessage)
        {
        }

        /// <summary>
        ///     Constructor.  Creates an exception with bitpayCode and a message only.
        /// </summary>
        /// <param name="bitpayCode">The bitpayCode of the exception</param>
        /// <param name="message">The message text for the exception.</param>
        public BitPayException(string bitpayCode, string message) : base(message)
        {
            BitpayCode = bitpayCode;
        }

        /// <summary>
        ///     Constructor.  Creates an exception with a message and root cause exception.
        /// </summary>
        /// <param name="bitpayCode">The bitpayCode of the exception</param>
        /// <param name="message">The message text for the exception.</param>
        /// <param name="cause">The root cause of this exception.</param>
        public BitPayException(string bitpayCode, string message, Exception cause) : base(message, cause)
        {
            BitpayCode = bitpayCode;
        }

        public BitPayException(Exception cause) : base(BitPayMessage, cause)
        {
        }

        public BitPayException(string bitpayMessage) : base(bitpayMessage)
        {
        }

        public BitPayException(string bitpayMessage, Exception cause) : base(bitpayMessage, cause)
        {
        }

        public string Code => BitpayCode;
    }
}