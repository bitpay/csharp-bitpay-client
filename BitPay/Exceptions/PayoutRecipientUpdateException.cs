using System;

namespace BitPay.Exceptions
{
    public class PayoutRecipientUpdateException : PayoutRecipientException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-RECIPIENT-UPDATE";
        private const string BitPayMessage = "Failed to update payout recipient.";

        public PayoutRecipientUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutRecipientUpdateException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}
