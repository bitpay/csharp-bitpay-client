﻿// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Ledger
{
    public class Ledger
    {
        [JsonProperty(PropertyName = "currency")] public string Currency { get; set; }
        
        [JsonProperty(PropertyName = "balance")] public decimal Balance { get; set; }

        public Ledger(string currency, decimal balance)
        {
            Currency = currency;
            Balance = balance;
        }
    }
}