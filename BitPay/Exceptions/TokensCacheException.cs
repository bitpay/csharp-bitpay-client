using System;

namespace BitPayAPI.Exceptions {
    public class TokensCacheException : BitPayException {
        private readonly string _bitpayCode = "BITPAY-TOKENSCACHE-GENERIC";
        private const string BitPayMessage = "An unexpected error occured while trying to manage the tokens cache";

        public TokensCacheException() : base(BitPayMessage) {
            BitpayCode = _bitpayCode;
        }

        public TokensCacheException(Exception ex) : base(BitPayMessage, ex) {
            BitpayCode = _bitpayCode;
        }

        public TokensCacheException(string bitpayCode, string message) : base(bitpayCode, message) {
        }

        public TokensCacheException(string bitpayCode, string message, Exception cause) : base(bitpayCode, message,
            cause) {
        }
    }
}
