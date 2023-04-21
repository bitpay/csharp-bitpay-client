// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class RatesException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the rates";
        private new const string BitPayCode = "BITPAY-RATES-GENERIC";

        public RatesException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RatesException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public RatesException(string bitPayCode, string message) : base(bitPayCode, message)
        {
        }

        public RatesException(string bitPayCode, string message, Exception cause, string apiCode = "000000") 
            : base(bitPayCode, message, cause, apiCode)
        {
        }

        protected RatesException(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext)
        {
        }
    }
}