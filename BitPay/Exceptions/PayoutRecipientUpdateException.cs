using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutRecipientUpdateException : PayoutRecipientException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-RECIPIENT-UPDATE";
        private const string BitPayMessage = "Failed to update payout recipient.";

        public PayoutRecipientUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutRecipientUpdateException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
