using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutRecipientException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the payout recipient";
        private readonly string _bitpayCode = "BITPAY-PAYOUT-RECIPIENT-GENERIC";

        public PayoutRecipientException() : base(BitPayMessage)
        {
            BitpayCode = _bitpayCode;
        }

        public PayoutRecipientException(Exception ex) : base(BitPayMessage, ex)
        {
            BitpayCode = _bitpayCode;
        }

        public PayoutRecipientException(string bitpayCode, string message) : base(bitpayCode, message)
        {
        }

        public PayoutRecipientException(string bitpayCode, string message, Exception cause) : base(bitpayCode, message, cause)
        {
        }
    }
}
