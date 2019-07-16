using System;

namespace BitPaySDK.Exceptions
{
    public class BillUpdateException : BitPayException
    {
        private const string BitPayCode = "BITPAY-BILL-UPDATE";
        private const string BitPayMessage = "Failed to update bill";

        public BillUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BillUpdateException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}