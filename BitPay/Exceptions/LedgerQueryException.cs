// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class LedgerQueryException : LedgerException
    {
        private new const string BitPayCode = "BITPAY-LEDGER-GET";
        private const string BitPayMessage = "Failed to retrieve ledger";

        public LedgerQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public LedgerQueryException(Exception ex, string apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected LedgerQueryException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}