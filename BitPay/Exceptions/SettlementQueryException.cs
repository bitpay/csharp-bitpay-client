using System;

namespace BitPay.Exceptions
{
    public class SettlementQueryException : SettlementException
    {
        private const string BitPayCode = "BITPAY-SETTLEMENT-GET";
        private const string BitPayMessage = "Failed to retrieve settlement.";

        public SettlementQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public SettlementQueryException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}
