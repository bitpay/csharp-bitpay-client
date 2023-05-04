// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class BillQueryException : BillException
    {
        private new const string BitPayCode = "BITPAY-BILL-GET";
        private const string BitPayMessage = "Failed to retrieve bill";

        public BillQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BillQueryException(Exception ex, string? apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected BillQueryException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}