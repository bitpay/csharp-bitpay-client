// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class WalletException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the wallet";
        private new const string BitPayCode = "BITPAY-WALLET-GENERIC";

        public WalletException() : base(BitPayCode, BitPayMessage)
        {
        }

        public WalletException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public WalletException(string bitPayCode, string message) : base(bitPayCode, message)
        {
        }

        public WalletException(string bitPayCode, string message, Exception cause) : base(bitPayCode, message, cause)
        {
        }

        protected WalletException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}