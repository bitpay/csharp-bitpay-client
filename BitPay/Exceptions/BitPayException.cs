// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    /// <summary>
    ///     Provides an common BitPay exception.
    /// </summary>
    [Serializable]
    public class BitPayException : Exception
    {
        public BitPayException(string message) : base(message)
        {
        }
        
        protected BitPayException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}