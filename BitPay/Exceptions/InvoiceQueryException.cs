using System;

namespace BitPay.Exceptions
{
    public class InvoiceQueryException : InvoiceException
    {
        private const string BitPayCode = "BITPAY-INVOICE-GET";
        private const string BitPayMessage = "Failed to retrieve invoice";
        protected string ApiCode;

        public InvoiceQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public InvoiceQueryException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}