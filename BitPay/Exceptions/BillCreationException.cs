// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class BillCreationException : BillException
    {
        private new const string BitPayCode = "BITPAY-BILL-CREATE";
        private const string BitPayMessage = "Failed to create bill";

        public BillCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BillCreationException(Exception ex, string apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected BillCreationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}