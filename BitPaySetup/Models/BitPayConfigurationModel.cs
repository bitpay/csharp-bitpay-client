// Copyright (c) 2019 BitPay.
// All rights reserved.

namespace BitPaySetup.Models
{
    public class BitPayConfigurationModel
    {
        public BitPayConfigurationModel()
        {
            BitPayConfiguration = new BitPayConfiguration();
        }

        public BitPayConfiguration BitPayConfiguration { get; set; }
    }

    public class ApiTokens
    {
        public ApiTokens()
        {
            merchant = "";
            payout = "";
        }

        public string? merchant { get; set; }
        public string? payout { get; set; }
    }

    public class Test
    {
        public Test()
        {
            PrivateKeyPath = "";
            PrivateKey = "";
            ApiTokens = new ApiTokens();
        }

        public string PrivateKeyPath { get; set; }

        public string PrivateKey { get; set; }
        public ApiTokens ApiTokens { get; set; }
    }

    public class Prod
    {
        public Prod()
        {
            PrivateKeyPath = "";
            PrivateKey = "";
            ApiTokens = new ApiTokens();
        }
        public string PrivateKeyPath { get; set; }
        public string PrivateKey { get; set; }

        public ApiTokens ApiTokens { get; set; }
    }

    public class EnvConfig
    {
        public EnvConfig()
        {
            Test = new Test();
            Prod = new Prod();
        }

        public Test Test { get; set; }
        public Prod Prod { get; set; }
    }

    public class BitPayConfiguration
    {
        public BitPayConfiguration()
        {
            Environment = "";
            EnvConfig = new EnvConfig();
        }

        public string Environment { get; set; }
        public EnvConfig EnvConfig { get; set; }
    }
}