// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class MissingRequiredField : BitPayException
    {
        private new const string BitPayCode = "BITPAY-GENERIC";
        private const string BitPayMessage = "Missing required field";
        
        public MissingRequiredField(string fieldName) : base(BitPayCode, BitPayMessage + " " + fieldName)
        {
        }

        protected MissingRequiredField(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}