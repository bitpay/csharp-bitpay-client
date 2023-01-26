using System;

namespace BitPay.Exceptions
{
    public class LedgerQueryException : LedgerException
    {
        private const string BitPayCode = "BITPAY-LEDGER-GET";
        private const string BitPayMessage = "Failed to retrieve ledger";

        public LedgerQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public LedgerQueryException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}