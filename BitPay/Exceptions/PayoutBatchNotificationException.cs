// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class PayoutBatchNotificationException : PayoutBatchException
    {
        private new const string BitPayCode = "BITPAY-PAYOUT-BATCH-NOTIFICATION";
        private const string BitPayMessage = "Failed to send payout batch notification.";

        public PayoutBatchNotificationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutBatchNotificationException(Exception ex, string apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected PayoutBatchNotificationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
