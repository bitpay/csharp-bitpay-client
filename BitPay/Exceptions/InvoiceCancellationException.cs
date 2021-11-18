using System;

namespace BitPaySDK.Exceptions
{
    public class InvoiceCancellationException : InvoiceException
    {
        private const string BitPayCode = "BITPAY-INVOICE-CANCELLATION";
        private const string BitPayMessage = "Failed to cancel invoice";

        public InvoiceCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public InvoiceCancellationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
