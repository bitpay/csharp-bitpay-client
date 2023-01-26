using System;

namespace BitPay.Exceptions
{
    public class InvoiceUpdateException : InvoiceException
    {
        private const string BitPayCode = "BITPAY-INVOICE-UPDATE";
        private const string BitPayMessage = "Failed to update invoice";

        public InvoiceUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public InvoiceUpdateException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
