// Copyright (c) 2019 BitPay.
// All rights reserved.

namespace BitPay.Models.Invoice
{
    public class SupportedTransactionCurrencies
    {
        public SupportedTransactionCurrency Btc { get; set; }
        public SupportedTransactionCurrency Bch { get; set; }
        public SupportedTransactionCurrency Eth { get; set; }
        public SupportedTransactionCurrency Usdc { get; set; }
        public SupportedTransactionCurrency Gusd { get; set; }
        public SupportedTransactionCurrency Pax { get; set; }
    }
}