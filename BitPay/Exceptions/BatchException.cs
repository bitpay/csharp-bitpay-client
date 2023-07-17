// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class BatchException : BitPayException
    {
        private new const string BitPayCode = "BITPAY-BATCH";
        private const string BitPayMessage = "Error when processing the batch";

        public BatchException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BatchException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public BatchException(string bitPayCode, string message, Exception cause, string apiCode = "000000")
            : base(bitPayCode, message, cause, apiCode)
        {
        }

        protected BatchException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}