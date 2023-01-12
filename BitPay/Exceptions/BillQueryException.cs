using System;

namespace BitPay.Exceptions
{
    public class BillQueryException : BillException
    {
        private const string BitPayCode = "BITPAY-BILL-GET";
        private const string BitPayMessage = "Failed to retrieve bill";

        public BillQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BillQueryException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}