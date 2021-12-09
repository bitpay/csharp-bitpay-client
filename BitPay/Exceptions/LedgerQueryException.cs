using System;

namespace BitPaySDK.Exceptions
{
    public class LedgerQueryException : LedgerException
    {
        private const string BitPayCode = "BITPAY-LEDGER-GET";
        private const string BitPayMessage = "Failed to retrieve ledger";
        protected string ApiCode;

        public LedgerQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public LedgerQueryException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}