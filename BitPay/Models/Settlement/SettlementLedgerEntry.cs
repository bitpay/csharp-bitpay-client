﻿// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;

namespace BitPay.Models.Settlement
{
    public class SettlementLedgerEntry
    {
        public int Code { get; set; }
        public string? InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Description { get; set; }
        public string? Reference { get; set; }
        public InvoiceData? InvoiceData { get; set; }

        public SettlementLedgerEntry(int code, decimal amount, DateTime timestamp)
        {
            Code = code;
            Amount = amount;
            Timestamp = timestamp;
        }
    }
}