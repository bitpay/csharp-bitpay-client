using System;

namespace BitPayAPI.Exceptions
{
    public class BillQueryException : BillException
    {
        private const string BitPayCode = "BITPAY-BILL-GET";
        private const string BitPayMessage = "Failed to retrieve bill";

        public BillQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BillQueryException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}