using System;

namespace BitPaySDK.Models.Settlement
{
    public class InvoiceData
    {
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public decimal OverPaidAmount { get; set; }
        public double PayoutPercentage { get; set; }
        public decimal BtcPrice { get; set; }
        public RefundInfo RefundInfo { get; set; }
    }
}