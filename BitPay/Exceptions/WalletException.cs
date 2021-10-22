using System;

namespace BitPaySDK.Exceptions
{
    public class WalletException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the wallet";
        private readonly string _bitpayCode = "BITPAY-WALLET-GENERIC";

        public WalletException() : base(BitPayMessage)
        {
            BitpayCode = _bitpayCode;
        }

        public WalletException(Exception ex) : base(BitPayMessage, ex)
        {
            BitpayCode = _bitpayCode;
        }

        public WalletException(string bitpayCode, string message) : base(bitpayCode, message)
        {
        }

        public WalletException(string bitpayCode, string message, Exception cause) : base(bitpayCode, message, cause)
        {
        }
    }
}