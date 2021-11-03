using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutRecipientQueryException : PayoutRecipientException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-RECIPIENT-GET";
        private const string BitPayMessage = "Failed to retrieve payout recipient.";

        public PayoutRecipientQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutRecipientQueryException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
