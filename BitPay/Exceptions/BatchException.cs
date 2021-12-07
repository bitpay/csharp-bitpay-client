using System;

namespace BitPaySDK.Exceptions
{
    public class BatchException : BitPayException
    {
        private const string BitPayCode = "BITPAY-BATCH";
        private const string BitPayMessage = "Error when processing the batch";
        protected string ApiCode;

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

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}