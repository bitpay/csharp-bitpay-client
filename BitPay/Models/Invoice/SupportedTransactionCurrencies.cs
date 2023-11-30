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

        [Obsolete("Deprecated, use GetSupportedCurrency(\"BTC\") or directly SupportedCurrencies[\"BTC\"]")]
        public SupportedTransactionCurrency? Btc
        {
            get
            {
                return GetSupportedCurrency("BTC");
            }
        }

        [Obsolete("Deprecated, use GetSupportedCurrency(\"BCH\") or directly SupportedCurrencies[\"BCH\"]")]
        public SupportedTransactionCurrency? Bch
        {
            get
            {
                return GetSupportedCurrency("BCH");
            }
        }
        
        [Obsolete("Deprecated, use GetSupportedCurrency(\"ETH\") or directly SupportedCurrencies[\"ETH\"]")]
        public SupportedTransactionCurrency? Eth
        {
            get
            {
                return GetSupportedCurrency("ETH");
            }
        }
        
        [Obsolete("Deprecated, use GetSupportedCurrency(\"USDC\") or directly SupportedCurrencies[\"USDC\"]")]
        public SupportedTransactionCurrency? Usdc
        {
            get
            {
                return GetSupportedCurrency("USDC");
            }
        }

        [Obsolete("Deprecated, use GetSupportedCurrency(\"GUSD\") or directly SupportedCurrencies[\"GUSD\"]")]
        public SupportedTransactionCurrency? Gusd
        {
            get
            {
                return GetSupportedCurrency("GUSD");
            }
        }

        [Obsolete("Deprecated, use GetSupportedCurrency(\"PAX\") or directly SupportedCurrencies[\"PAX\"]")]
        public SupportedTransactionCurrency? Pax
        {
            get
            {
                return GetSupportedCurrency("PAX");
            }
        }
    }
}