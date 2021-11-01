using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutSubmitPayoutRecipientsException : PayoutRecipientException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-SUBMIT-RECIPIENT";
        private const string BitPayMessage = "Failed to submit payout recipient.";

        public PayoutSubmitPayoutRecipientsException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutSubmitPayoutRecipientsException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}