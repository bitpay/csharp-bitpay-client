﻿// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class RefundQueryException : RefundException
    {
        private new const string BitPayCode = "BITPAY-REFUND-GET";
        private const string BitPayMessage = "Failed to retrieve refund";

        public RefundQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundQueryException(Exception ex, string apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected RefundQueryException(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext)
        {
        }
    }
}