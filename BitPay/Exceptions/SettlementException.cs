using System;

namespace BitPaySDK.Exceptions
{
    public class SettlementException : BitPayException
    {
        private const string BitPayCode = "BITPAY-SETTLEMENT";
        private const string BitPayMessage = "Error when processing the settlement";
        protected string ApiCode;

        public SettlementException() : base(BitPayCode, BitPayMessage)
        {
        }

        public SettlementException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public SettlementException(string bitpayCode, string message) : base(bitpayCode, message)
        {
        }

        public SettlementException(string bitpayCode, string message, Exception cause, string apiCode = "000000") : base(bitpayCode, message, cause, apiCode)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}