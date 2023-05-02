// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class LedgerException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the ledger";
        private new const string BitPayCode = "BITPAY-LEDGER-GENERIC";

        public LedgerException() : base(BitPayCode, BitPayMessage)
        {
        }

        public LedgerException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public LedgerException(string bitPayCode, string message)
            : base(bitPayCode, message)
        {
        }

        public LedgerException(string bitPayCode, string message, Exception cause, string apiCode = "000000")
            : base(bitPayCode, message, cause, apiCode)
        {
        }

        protected LedgerException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}