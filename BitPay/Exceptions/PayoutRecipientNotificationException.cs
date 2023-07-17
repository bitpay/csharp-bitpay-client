// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutRecipientNotificationException : PayoutRecipientException
    {
        private new const string BitPayCode = "BITPAY-PAYOUT-RECIPIENT-NOTIFICATION";
        private const string BitPayMessage = "Failed to send payout recipient notification.";

        public PayoutRecipientNotificationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutRecipientNotificationException(Exception ex, string? apiCode = "000000") 
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected PayoutRecipientNotificationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
