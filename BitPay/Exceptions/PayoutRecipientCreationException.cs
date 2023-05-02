// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutRecipientCreationException : PayoutRecipientException
    {
        private new const string BitPayCode = "BITPAY-PAYOUT-RECIPIENT-CREATE";
        private const string BitPayMessage = "Failed to submit payout recipient.";

        public PayoutRecipientCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutRecipientCreationException(Exception ex, string apiCode = "000000") 
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected PayoutRecipientCreationException(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
