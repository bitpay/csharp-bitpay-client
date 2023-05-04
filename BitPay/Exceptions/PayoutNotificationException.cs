// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutNotificationException : PayoutException
    {
        private new const string BitPayCode = "BITPAY-PAYOUT-NOTIFICATION";
        private const string BitPayMessage = "Failed to send payout notification.";

        public PayoutNotificationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutNotificationException(Exception ex, string? apiCode = "000000") 
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected PayoutNotificationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
