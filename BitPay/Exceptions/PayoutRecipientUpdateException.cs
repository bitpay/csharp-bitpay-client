// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutRecipientUpdateException : PayoutRecipientException
    {
        private new const string BitPayCode = "BITPAY-PAYOUT-RECIPIENT-UPDATE";
        private const string BitPayMessage = "Failed to update payout recipient.";

        public PayoutRecipientUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutRecipientUpdateException(Exception ex, string? apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected PayoutRecipientUpdateException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
