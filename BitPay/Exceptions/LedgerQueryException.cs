using System;

namespace BitPaySDK.Exceptions
{
    public class LedgerQueryException : BitPayException
    {
        private const string BitPayCode = "BITPAY-LEDGER-GET";
        private const string BitPayMessage = "Failed to retrieve ledger";

        public LedgerQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public LedgerQueryException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}