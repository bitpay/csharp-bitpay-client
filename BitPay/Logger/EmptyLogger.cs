// Copyright (c) 2019 BitPay.
// All rights reserved.

namespace BitPay.Logger
{
    public class EmptyLogger : IBitPayLogger
    {
        public void LogRequest(string method, string endpoint, string? json)
        {
        }

        public void LogResponse(string method, string endpoint, string? json)
        {
        }

        public void LogError(string message)
        {
        }
    }
}