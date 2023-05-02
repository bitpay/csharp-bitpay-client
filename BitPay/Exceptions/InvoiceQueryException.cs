// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class InvoiceQueryException : InvoiceException
    {
        private new const string BitPayCode = "BITPAY-INVOICE-GET";
        private const string BitPayMessage = "Failed to retrieve invoice";

        public InvoiceQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public InvoiceQueryException(Exception ex, string apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected InvoiceQueryException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}