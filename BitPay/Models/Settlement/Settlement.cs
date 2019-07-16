using System;
using System.Collections.Generic;

namespace BitPaySDK.Models.Settlement
{
    public class Settlement
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Currency { get; set; }
        public PayoutInfo PayoutInfo { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateExecuted { get; set; }
        public DateTime DateCompleted { get; set; }
        public DateTime OpeningDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal LedgerEntriesSum { get; set; }
        public List<WithHoldings> WithHoldings { get; set; }
        public decimal WithHoldingsSum { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SettlementLedgerEntry> LedgerEntries { get; set; }
        public string Token { get; set; }
    }
}