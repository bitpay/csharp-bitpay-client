// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutQueryException : PayoutException
    {
        private new const string BitPayCode = "BITPAY-PAYOUT-GET";
        private const string BitPayMessage = "Failed to retrieve payout.";

        public PayoutQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutQueryException(Exception ex, string apiCode = "000000") 
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected PayoutQueryException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}