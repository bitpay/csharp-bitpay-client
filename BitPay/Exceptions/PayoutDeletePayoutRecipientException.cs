using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutDeletePayoutRecipientException : PayoutRecipientException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-DELETE-RECIPIENT";
        private const string BitPayMessage = "Failed to delete payout recipient.";

        public PayoutDeletePayoutRecipientException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutDeletePayoutRecipientException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}