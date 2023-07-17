// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class RefundUpdateException : RefundException
    {
        private new const string BitPayCode = "BITPAY-REFUND-UPDATE";
        private const string BitPayMessage = "Failed to update refund";

        public RefundUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundUpdateException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        protected RefundUpdateException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}