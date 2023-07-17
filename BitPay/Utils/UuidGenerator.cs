// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;

namespace BitPay.Utils
{
    public class UuidGenerator : IGuidGenerator
    {
        public string Execute()
        {
            Guid guid = Guid.NewGuid();
            
            return guid.ToString();
        }
    }
}