using System;

namespace BitPay.Exceptions
{
    public class InvoiceQueryException : InvoiceException
    {
        private const string BitPayCode = "BITPAY-INVOICE-GET";
        private const string BitPayMessage = "Failed to retrieve invoice";

        public InvoiceQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public InvoiceQueryException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}