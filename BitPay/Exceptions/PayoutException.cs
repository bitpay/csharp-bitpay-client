// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the payout.";
        private new const string BitPayCode = "BITPAY-PAYOUT-GENERIC";

        public PayoutException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public PayoutException(string bitPayCode, string message) : base(bitPayCode, message)
        {
        }

        public PayoutException(string bitPayCode, string message, Exception cause, string? apiCode = "000000")
            : base(bitPayCode, message, cause, apiCode)
        {
        }

        protected PayoutException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}