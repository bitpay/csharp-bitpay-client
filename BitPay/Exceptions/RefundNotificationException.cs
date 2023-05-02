// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class RefundNotificationException : RefundException
    {
        private new const string BitPayCode = "BITPAY-REFUND-NOTIFICATION";
        private const string BitPayMessage = "Failed to send refund notification";

        public RefundNotificationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundNotificationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        protected RefundNotificationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}