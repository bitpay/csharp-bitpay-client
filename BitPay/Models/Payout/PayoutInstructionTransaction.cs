// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;

namespace BitPay.Models.Payout
{
    public class PayoutInstructionTransaction
    {
        public string Txid { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public PayoutInstructionTransaction(string txid, decimal amount, DateTime date)
        {
            Txid = txid;
            Amount = amount;
            Date = date;
        }
    }
}