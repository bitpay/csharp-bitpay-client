using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutRecipientCreationException : PayoutRecipientException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-RECIPIENT-CREATE";
        private const string BitPayMessage = "Failed to submit payout recipient.";
        protected string ApiCode;

        public PayoutRecipientCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutRecipientCreationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}
