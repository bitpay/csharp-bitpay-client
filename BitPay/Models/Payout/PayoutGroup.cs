// Copyright (c) 2019 BitPay.
// All rights reserved.

using System.Collections.Generic;

using Newtonsoft.Json;

namespace BitPay.Models.Payout;

public class PayoutGroup
{
    [JsonProperty(PropertyName = "completed")]
    public List<Payout> Payouts { get; private set; } = new();
    public List<PayoutGroupFailed> Failed { get; } = new();
    
    [JsonProperty(PropertyName = "cancelled")]
    private List<Payout> Cancelled { set { Payouts = value; } }
}