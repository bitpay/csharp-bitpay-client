using System;

namespace BitPay.Exceptions
{
    public class BillUpdateException : BillException
    {
        private const string BitPayCode = "BITPAY-BILL-UPDATE";
        private const string BitPayMessage = "Failed to update bill";

        public BillUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BillUpdateException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}