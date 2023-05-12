// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    /// <summary>
    ///     Provides an API specific exception handler.
    /// </summary>
    [Serializable]
    public class BitPayException : Exception
    {
        private const string BitPayMessage = "Unexpected Bitpay exeption";
        private readonly string _bitPayCode = "BITPAY-GENERIC";
        private readonly string? _apiCode;

        protected string BitPayCode
        {
            get
            {
                return _bitPayCode;
            }
        }

        public string? ApiCode
        {
            get
            {
                return _apiCode;
            }
        }
#pragma warning disable CA1711
        public Exception? Ex { get; }
#pragma warning restore CA1711

        public BitPayException() : base(BitPayMessage)
        {
        }

        /// <summary>
        ///     Constructor.  Creates an exception with bitPayCode and a message only.
        /// </summary>
        /// <param name="bitPayCode">The bitPayCode of the exception</param>
        /// <param name="message">The message text for the exception.</param>
        /// <param name="isApiCode"></param>
        public BitPayException(string bitPayCode, string message, bool isApiCode=false) : base(message)
        {
            if (isApiCode)
            {
                _apiCode = bitPayCode;
            }
            else
            {
                _bitPayCode = bitPayCode;
            }
        }

        /// <summary>
        ///     Constructor.  Creates an exception with a message and root cause exception.
        /// </summary>
        /// <param name="bitPayCode">The bitPayCode of the exception</param>
        /// <param name="message">The message text for the exception.</param>
        /// <param name="cause">The root cause of this exception.</param>
        /// <param name="apiCode">The API exception code to throw.</param>
        public BitPayException(string bitPayCode, string message, Exception cause, string? apiCode) : base(message, cause)
        {
            _bitPayCode = bitPayCode;
            _apiCode = apiCode;
        }
        
        public BitPayException(Exception cause) : base(BitPayMessage, cause)
        {
        }

        public BitPayException(string bitPayMessage) : base(bitPayMessage)
        {
        }

        public BitPayException(string bitPayMessage, Exception cause) : base(bitPayMessage, cause)
        {
        }

        public BitPayException(string bitPayCode, string message, Exception ex) : this(bitPayCode, message)
        {
        }

        protected BitPayException(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext)
        {
            _bitPayCode = (string)serializationInfo.GetValue(nameof(_bitPayCode), typeof(int))!;
            _apiCode = (string?)serializationInfo.GetValue(nameof(_apiCode), typeof(int));
        }
    }
}