// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;

namespace BitPay.Models.Settlement
{
    public class InvoiceData
    {
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string TransactionCurrency { get; set; }
        public decimal OverPaidAmount { get; set; }
        public Dictionary<string, decimal> PayoutPercentage { get; set; }
        public decimal BtcPrice { get; set; }
        public RefundInfo? RefundInfo { get; set; }

        public InvoiceData(
            string orderId,
            DateTime date,
            decimal price,
            string currency,
            string transactionCurrency,
            decimal overPaidAmount,
            Dictionary<string, decimal> payoutPercentage,
            decimal btcPrice
        )
        {
            OrderId = orderId;
            Date = date;
            Price = price;
            Currency = currency;
            TransactionCurrency = transactionCurrency;
            OverPaidAmount = overPaidAmount;
            PayoutPercentage = payoutPercentage;
            BtcPrice = btcPrice;
        }
    }
}