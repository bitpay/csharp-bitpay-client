// Copyright (c) 2019 BitPay.
// All rights reserved.

namespace BitPay.Models.Invoice
{
    public static class RefundStatus
    {
        public const string Preview = "preview";
        public const string Created = "created";
        public const string Pending = "pending";
        public const string Canceled = "canceled";
        public const string Success = "success";
        public const string Failure = "failure";
    }
}