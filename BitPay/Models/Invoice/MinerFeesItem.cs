// Copyright (c) 2019 BitPay.
// All rights reserved.

namespace BitPay.Models.Invoice
{
    public class MinerFeesItem
    {
        public decimal SatoshisPerByte { get; set; }
        public decimal TotalFee { get; set; }
        public decimal? FiatAmount { get; set; }
    }
}
