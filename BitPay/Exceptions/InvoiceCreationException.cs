using System;

namespace BitPaySDK.Exceptions
{
    public class InvoiceCreationException : BitPayException
    {
        private const string BitPayCode = "BITPAY-INVOICE-CREATE";
        private const string BitPayMessage = "Failed to create invoice";

        public InvoiceCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public InvoiceCreationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}