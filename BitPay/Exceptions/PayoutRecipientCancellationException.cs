using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutRecipientCancellationException : PayoutRecipientException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-CANCEL-RECIPIENT";
        private const string BitPayMessage = "Failed to delete payout recipient.";

        public PayoutRecipientCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutRecipientCancellationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
