using System;

namespace BitPay.Exceptions
{
    public class InvoiceCreationException : InvoiceException
    {
        private const string BitPayCode = "BITPAY-INVOICE-CREATE";
        private const string BitPayMessage = "Failed to create invoice";

        public InvoiceCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public InvoiceCreationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}