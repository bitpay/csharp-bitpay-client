using System;

namespace BitPaySDK.Exceptions
{
    public class InvoiceQueryException : InvoiceException
    {
        private const string BitPayCode = "BITPAY-INVOICE-GET";
        private const string BitPayMessage = "Failed to retrieve invoice";

        public InvoiceQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public InvoiceQueryException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}