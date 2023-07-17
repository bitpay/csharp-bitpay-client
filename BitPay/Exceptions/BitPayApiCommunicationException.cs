// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class BitPayApiCommunicationException : BitPayException
    {
        private const string BitPayMessage = "Error when communicating with the BitPay API";
        private new const string BitPayCode = "BITPAY-API";

        public BitPayApiCommunicationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BitPayApiCommunicationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public BitPayApiCommunicationException(string message) : base(BitPayCode, message)
        {
        }

        public BitPayApiCommunicationException(string apiCode, string message) : base(apiCode, message, true)
        {
        }

        public BitPayApiCommunicationException(string bitPayCode, string message, Exception cause,
            string apiCode = "000000")
            : base(bitPayCode, message, cause, apiCode)
        {
        }

        protected BitPayApiCommunicationException(SerializationInfo serializationInfo,
            StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}