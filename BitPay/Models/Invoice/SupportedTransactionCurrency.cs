// Copyright (c) 2019 BitPay.
// All rights reserved.

namespace BitPay.Models.Invoice
{
    public class SupportedTransactionCurrency
    {
        public bool Enabled { get; set; }
        
        public SupportedTransactionCurrency(bool enabled)
        {
            Enabled = enabled;
        }
    }
}