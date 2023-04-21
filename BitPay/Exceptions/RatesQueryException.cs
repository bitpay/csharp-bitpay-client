// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class RatesQueryException : RatesException
    {
        private new const string BitPayCode = "BITPAY-RATES-GET";
        private const string BitPayMessage = "Failed to retirieve rates.";

        public RatesQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RatesQueryException(Exception ex, string apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected RatesQueryException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}