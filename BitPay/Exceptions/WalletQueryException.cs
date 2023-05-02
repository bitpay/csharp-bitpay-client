// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class WalletQueryException : WalletException
    {
        private new const string BitPayCode = "BITPAY-WALLET-GET";
        private const string BitPayMessage = "Failed to retrieve supported wallets";

        public WalletQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public WalletQueryException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        protected WalletQueryException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
