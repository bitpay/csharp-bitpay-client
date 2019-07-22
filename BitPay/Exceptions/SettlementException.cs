using System;

namespace BitPaySDK.Exceptions
{
    public class SettlementException : BitPayException
    {
        private const string BitPayCode = "BITPAY-SETTLEMENT";
        private const string BitPayMessage = "Error when processing the settlement";

        public SettlementException() : base(BitPayCode, BitPayMessage)
        {
        }

        public SettlementException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}