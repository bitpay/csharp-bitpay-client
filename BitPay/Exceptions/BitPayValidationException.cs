// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    /// <summary>
    ///     Provides validation exception.
    /// </summary>
    [Serializable]
    public class BitPayValidationException : BitPayGenericException
    {
        public BitPayValidationException(string message) : base(message)
        {
        }
    }
}