// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutRecipientQueryException : PayoutRecipientException
    {
        private new const string BitPayCode = "BITPAY-PAYOUT-RECIPIENT-GET";
        private const string BitPayMessage = "Failed to retrieve payout recipient.";

        public PayoutRecipientQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutRecipientQueryException(Exception ex, string apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected PayoutRecipientQueryException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
