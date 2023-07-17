// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutBatchException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the payout batch";
        private new const string BitPayCode = "BITPAY-PAYOUT-BATCH-GENERIC";

        public PayoutBatchException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutBatchException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public PayoutBatchException(string bitPayCode, string message) : base(bitPayCode, message)
        {
        }

        public PayoutBatchException(string bitPayCode, string message, Exception cause, string apiCode = "000000")
            : base(bitPayCode, message, cause, apiCode)
        {
        }

        protected PayoutBatchException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}