// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class InvoiceCancellationException : InvoiceException
    {
        private new const string BitPayCode = "BITPAY-INVOICE-CANCELLATION";
        private const string BitPayMessage = "Failed to cancel invoice";

        public InvoiceCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public InvoiceCancellationException(Exception ex, string apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected InvoiceCancellationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}