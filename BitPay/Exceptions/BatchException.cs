using System;

namespace BitPay.Exceptions
{
    public class BatchException : BitPayException
    {
        private const string BitPayCode = "BITPAY-BATCH";
        private const string BitPayMessage = "Error when processing the batch";

        public BatchException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BatchException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public BatchException(string bitpayCode, string message, Exception cause, string apiCode = "000000") : base(bitpayCode, message, cause, apiCode)
        {
            ApiCode = apiCode;
        }
    }
}