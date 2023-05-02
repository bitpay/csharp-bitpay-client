// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class BillUpdateException : BillException
    {
        private new const string BitPayCode = "BITPAY-BILL-UPDATE";
        private const string BitPayMessage = "Failed to update bill";

        public BillUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BillUpdateException(Exception ex, string apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected BillUpdateException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}