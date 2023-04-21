// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class SettlementException : BitPayException
    {
        private new const string BitPayCode = "BITPAY-SETTLEMENT";
        private const string BitPayMessage = "Error when processing the settlement";

        public SettlementException() : base(BitPayCode, BitPayMessage)
        {
        }

        public SettlementException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public SettlementException(string bitPayCode, string message) : base(bitPayCode, message)
        {
        }

        public SettlementException(string bitPayCode, string message, Exception cause, string apiCode = "000000") 
            : base(bitPayCode, message, cause, apiCode)
        {
        }

        protected SettlementException(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext)
        {
        }
    }
}