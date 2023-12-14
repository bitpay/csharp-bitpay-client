// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    /// <summary>
    ///     Provides an API specific exception handler.
    /// </summary>
    [Serializable]
    public class BitPayApiException : BitPayException
    {
        private readonly string? _code;

        public BitPayApiException(string message) : base(message)
        {
        }
        
        public BitPayApiException(string message, string code) : base(message)
        {
            this._code = code;
        }
        
        public string? Code
        {
            get
            {
                return _code;
            }
        }
    }
}