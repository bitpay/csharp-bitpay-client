using System;

namespace BitPaySDK.Models.Payout
{
    public class PayoutInstructionTransaction
    {
        public string Txid { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
    }
}