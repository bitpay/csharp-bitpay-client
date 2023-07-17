// Copyright (c) 2019 BitPay.
// All rights reserved.

namespace BitPay.Models.Settlement
{
    public class RefundAmount
    {
        //Need to add other currency codes
        public decimal Btc { get; set; }
        public decimal Usd { get; set; }
        public decimal Eur { get; set; }
    }
}