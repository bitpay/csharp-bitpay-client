using System;

namespace BitPaySDK.Exceptions
{
    public class BillUpdateException : BillException
    {
        private const string BitPayCode = "BITPAY-BILL-UPDATE";
        private const string BitPayMessage = "Failed to update bill";
        protected string ApiCode;

        public BillUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BillUpdateException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}