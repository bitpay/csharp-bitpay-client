using System;

namespace BitPaySDK.Exceptions
{
    public class BillCreationException : BitPayException
    {
        private const string BitPayCode = "BITPAY-BILL-CREATE";
        private const string BitPayMessage = "Failed to create bill";

        public BillCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BillCreationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}