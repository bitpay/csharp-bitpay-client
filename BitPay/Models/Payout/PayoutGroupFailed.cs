// Copyright (c) 2019 BitPay.
// All rights reserved.

namespace BitPay.Models.Payout;

public class PayoutGroupFailed
{
    public string ErrMessage { get; }
    public string? PayoutId { get; }
    public string? Payee { get; }

    public PayoutGroupFailed(string errMessage, string? payoutId = null, string? payee = null)
    {
        ErrMessage = errMessage;
        PayoutId = payoutId;
        Payee = payee;
    }
}