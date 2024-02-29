// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class InvoiceRefundAddress
    {
        public InvoiceRefundAddress(string type, DateTime date)
        {
            Type = type;
            Date = date;
        }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "tag")]
        public int? Tag { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string? Email { get; set; }
    }
}