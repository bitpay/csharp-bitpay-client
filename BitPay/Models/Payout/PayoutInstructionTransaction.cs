using System;

namespace BitPay.Models.Payout
{
    public class PayoutInstructionTransaction
    {
        public string Txid { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}