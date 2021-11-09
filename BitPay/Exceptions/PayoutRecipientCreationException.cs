using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutRecipientCreationException : PayoutRecipientException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-RECIPIENT-CREATE";
        private const string BitPayMessage = "Failed to submit payout recipient.";

        public PayoutRecipientCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutRecipientCreationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
