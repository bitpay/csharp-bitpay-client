// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class SettlementQueryException : SettlementException
    {
        private new const string BitPayCode = "BITPAY-SETTLEMENT-GET";
        private const string BitPayMessage = "Failed to retrieve settlement.";

        public SettlementQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public SettlementQueryException(Exception ex, string? apiCode = "000000") 
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected SettlementQueryException(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
