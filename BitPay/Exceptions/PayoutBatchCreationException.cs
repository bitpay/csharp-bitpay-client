// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutBatchCreationException : PayoutBatchException
    {
        private new const string BitPayCode = "BITPAY-PAYOUT-BATCH-CREATE";
        private const string BitPayMessage = "Failed to create payout batch.";

        public PayoutBatchCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutBatchCreationException(Exception ex, string apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected PayoutBatchCreationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}