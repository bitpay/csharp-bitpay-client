using System;

namespace BitPayAPI
{
    public class SettlementLedgerEntry
    {
        public int Code { get; set; }
        public string InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public InvoiceData InvoiceData { get; set; }
    }
}