// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutUpdateException : PayoutException
    {
        private new const string BitPayCode = "BITPAY-PAYOUT-UPDATE";
        private const string BitPayMessage = "Failed to update payout.";

        public PayoutUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutUpdateException(Exception ex, string apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected PayoutUpdateException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}