// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class InvoiceException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the invoice";
        private new const string BitPayCode = "BITPAY-INVOICE-GENERIC";

        public InvoiceException() : base(BitPayCode, BitPayMessage)
        {
        }

        public InvoiceException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public InvoiceException(string bitPayCode, string message)
            : base(bitPayCode, message)
        {
        }

        public InvoiceException(string bitPayCode, string message, Exception cause, string apiCode = "000000")
            : base(bitPayCode, message, cause, apiCode)
        {
        }

        protected InvoiceException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}