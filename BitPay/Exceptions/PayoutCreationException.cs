// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutCreationException : PayoutException
    {
        private new const string BitPayCode = "BITPAY-PAYOUT-CREATE";
        private const string BitPayMessage = "Failed to create payout.";

        public PayoutCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutCreationException(Exception ex, string? apiCode = "000000") 
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected PayoutCreationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}