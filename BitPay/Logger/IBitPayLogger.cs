// Copyright (c) 2019 BitPay.
// All rights reserved.

namespace BitPay.Logger
{
    public interface IBitPayLogger
    {
        void LogRequest(
            string method,
            string endpoint,
            string? json
        );

        void LogResponse(
            string method,
            string endpoint,
            string? json
        );

        void LogError(string message);
    }
}