﻿namespace BitPay.Models.Invoice
{
    public class MinerFeesItem
    {
        public double SatoshisPerByte { get; set; }
        public double TotalFee { get; set; }
        public double? FiatAmount { get; set; }
    }
}
