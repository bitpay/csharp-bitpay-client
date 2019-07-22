using System;

namespace BitPaySDK.Exceptions
{
    public class InvoiceException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the invoice";
        private readonly string _bitpayCode = "BITPAY-INVOICE-GENERIC";

        public InvoiceException() : base(BitPayMessage)
        {
            BitpayCode = _bitpayCode;
        }

        public InvoiceException(Exception ex) : base(BitPayMessage, ex)
        {
            BitpayCode = _bitpayCode;
        }

        public InvoiceException(string bitpayCode, string message) : base(bitpayCode, message)
        {
        }

        public InvoiceException(string bitpayCode, string message, Exception cause) : base(bitpayCode, message, cause)
        {
        }
    }
}