// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutRecipientException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the payout recipient";
        private new const string BitPayCode = "BITPAY-PAYOUT-RECIPIENT-GENERIC";

        public PayoutRecipientException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutRecipientException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public PayoutRecipientException(string bitPayCode, string message) : base(bitPayCode, message)
        {
        }

        public PayoutRecipientException(string bitPayCode, string message, Exception cause, string? apiCode = "000000")
            : base(bitPayCode, message, cause, apiCode)
        {
        }

        protected PayoutRecipientException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}