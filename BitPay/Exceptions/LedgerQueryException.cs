using System;

namespace BitPayAPI.Exceptions
{
    public class LedgerQueryException : BitPayException
    {
        private const string BitPayCode = "BITPAY-LEDGER-GET";
        private const string BitPayMessage = "Failed to create invoice";

        public LedgerQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public LedgerQueryException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}