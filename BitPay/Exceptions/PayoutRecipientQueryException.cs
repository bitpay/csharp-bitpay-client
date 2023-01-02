using System;

namespace BitPay.Exceptions
{
    public class PayoutRecipientQueryException : PayoutRecipientException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-RECIPIENT-GET";
        private const string BitPayMessage = "Failed to retrieve payout recipient.";
        protected string ApiCode;

        public PayoutRecipientQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutRecipientQueryException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}
