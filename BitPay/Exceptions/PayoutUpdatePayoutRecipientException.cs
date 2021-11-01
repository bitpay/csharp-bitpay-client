using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutUpdatePayoutRecipientException : PayoutRecipientException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-UPDATE-RECIPIENT";
        private const string BitPayMessage = "Failed to update payout recipient.";

        public PayoutUpdatePayoutRecipientException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutUpdatePayoutRecipientException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}