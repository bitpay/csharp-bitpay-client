// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class InvoiceUpdateException : InvoiceException
    {
        private new const string BitPayCode = "BITPAY-INVOICE-UPDATE";
        private const string BitPayMessage = "Failed to update invoice";

        public InvoiceUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public InvoiceUpdateException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        protected InvoiceUpdateException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
