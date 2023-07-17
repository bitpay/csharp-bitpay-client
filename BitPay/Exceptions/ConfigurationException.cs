// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{

    [Serializable]
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {
        }

        protected ConfigurationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}