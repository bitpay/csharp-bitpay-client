using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutRecipientCancellationException : PayoutRecipientException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-RECIPIENT-CANCELLATION";
        private const string BitPayMessage = "Failed to delete payout recipient.";
        protected string ApiCode;

        public PayoutRecipientCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutRecipientCancellationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}
