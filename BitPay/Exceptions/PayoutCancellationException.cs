// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutCancellationException : PayoutException
    {
        private new const string BitPayCode = "BITPAY-PAYOUT-CANCELLATION";
        private const string BitPayMessage = "Failed to cancel payout.";

        public PayoutCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutCancellationException(Exception ex, string? apiCode = "000000") 
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected PayoutCancellationException(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
