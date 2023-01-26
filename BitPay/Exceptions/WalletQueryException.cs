using System;

namespace BitPay.Exceptions
{
    public class WalletQueryException : WalletException
    {
        private const string BitPayCode = "BITPAY-WALLET-GET";
        private const string BitPayMessage = "Failed to retrieve supported wallets";

        public WalletQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public WalletQueryException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
