// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutBatchCancellationException : PayoutBatchException
    {
        private new const string BitPayCode = "BITPAY-PAYOUT-BATCH-CANCELLATION";
        private const string BitPayMessage = "Failed to cancel payout batch.";

        public PayoutBatchCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutBatchCancellationException(Exception ex, string apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected PayoutBatchCancellationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}