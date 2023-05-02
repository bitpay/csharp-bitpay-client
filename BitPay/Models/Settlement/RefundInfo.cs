// Copyright (c) 2019 BitPay.
// All rights reserved.

namespace BitPay.Models.Settlement
{
    public class RefundInfo
    {
        public string SupportRequest { get; set; }
        public string Currency { get; set; }
        public RefundAmount Amounts { get; set; }
        
        public RefundInfo(string supportRequest, string currency, RefundAmount amounts)
        {
            SupportRequest = supportRequest;
            Currency = currency;
            Amounts = amounts;
        }
    }
}