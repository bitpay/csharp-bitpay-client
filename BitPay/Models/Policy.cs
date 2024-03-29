﻿// Copyright (c) 2019 BitPay.
// All rights reserved.

using System.Collections.Generic;

using Newtonsoft.Json;

namespace BitPay.Models
{
    /// <summary>
    ///     Provides BitPay token policy information.
    /// </summary>
    public class Policy
    {
        [JsonProperty(PropertyName = "policy")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "method")]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "params")]
        public List<string>? Params { get; set; }

        public Policy(string value, string method)
        {
            Value = value;
            Method = method;
        }
    }
}