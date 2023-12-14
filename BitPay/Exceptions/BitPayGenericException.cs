// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    /// <summary>
    ///     Provides generic exception.
    /// </summary>
    [Serializable]
    public class BitPayGenericException : BitPayException
    {
        public BitPayGenericException(string message) : base(message)
        {
        }
    }
}