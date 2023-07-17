// Copyright (c) 2019 BitPay.
// All rights reserved.

namespace BitPay.Models.Settlement
{
    public class WithHoldings
    {
        public decimal Amount { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string? Notes { get; set; }
        
        public WithHoldings(decimal amount, string code, string description)
        {
            Amount = amount;
            Code = code;
            Description = description;
        }
    }
}