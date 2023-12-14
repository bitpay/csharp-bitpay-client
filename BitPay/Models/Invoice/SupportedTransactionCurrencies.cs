// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;

namespace BitPay.Models.Invoice
{
    public class SupportedTransactionCurrencies
    {
        public Dictionary<string, SupportedTransactionCurrency> SupportedCurrencies { get; set; }

        public SupportedTransactionCurrencies(Dictionary<string, SupportedTransactionCurrency> supportedCurrencies)
        {
            SupportedCurrencies = supportedCurrencies;
        }
        
        public SupportedTransactionCurrency? GetSupportedCurrency(string currency)
        {
            if (SupportedCurrencies.TryGetValue(currency, out SupportedTransactionCurrency? value))
            {
                return value;
            }

            return null;
        }
    }
}