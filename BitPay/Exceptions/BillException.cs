// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class BillException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the bill";
        private new const string BitPayCode = "BITPAY-BILL-GENERIC";

        public BillException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BillException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public BillException(string bitPayCode, string message) : base(bitPayCode, message)
        {
        }

        public BillException(string bitPayCode, string message, Exception cause, string? apiCode = "000000")
            : base(bitPayCode, message, cause, apiCode)
        {
        }

        protected BillException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}