using System;

namespace BitPaySDK.Exceptions
{
    public class InvoiceCancellationException : InvoiceException
    {
        private const string BitPayCode = "BITPAY-INVOICE-CANCELLATION";
        private const string BitPayMessage = "Failed to cancel invoice";
        protected string ApiCode;

        public InvoiceCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public InvoiceCancellationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}
