// Copyright (c) 2019 BitPay.
// All rights reserved.

using BitPay.Logger;

namespace BitPayUnitTest.Logger
{
    public class LoggerProviderTest
    {
        [Fact]
        public void it_should_get_default_logger()
        {
            Assert.IsType<EmptyLogger>(LoggerProvider.GetLogger());
        }

        [Fact]
        public void it_should_set_logger()
        {
            var testLogger = new TestLogger();
            LoggerProvider.SetLogger(testLogger);
            
            Assert.Same(testLogger, LoggerProvider.GetLogger());
        }
    }

    class TestLogger : IBitPayLogger
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