// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutRecipientCancellationException : PayoutRecipientException
    {
        private new const string BitPayCode = "BITPAY-PAYOUT-RECIPIENT-CANCELLATION";
        private const string BitPayMessage = "Failed to delete payout recipient.";

        public PayoutRecipientCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutRecipientCancellationException(Exception ex, string? apiCode = "000000") 
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected PayoutRecipientCancellationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
