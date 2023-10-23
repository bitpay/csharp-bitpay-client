// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;

namespace BitPay.Logger
{
    public class LoggerProvider
    {
        private static IBitPayLogger? s_logger;

        private LoggerProvider()
        {
        }

        public static IBitPayLogger GetLogger()
        {
            if (s_logger == null)
            {
                s_logger = new EmptyLogger();
            }

            return s_logger;
        }

        public static void SetLogger(IBitPayLogger logger)
        {
            s_logger = logger;
        }
    }
}