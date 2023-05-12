// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class InvoiceCreationException : InvoiceException
    {
        private new const string BitPayCode = "BITPAY-INVOICE-CREATE";
        private const string BitPayMessage = "Failed to create invoice";

        public InvoiceCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public InvoiceCreationException(Exception ex, string? apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected InvoiceCreationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}