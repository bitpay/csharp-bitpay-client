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
            pos = "";
            merchant = "";
            payroll = "";
        }

        public string pos { get; set; }
        public string merchant { get; set; }
        public string payroll { get; set; }
    }

    public class Test
    {
        public Test()
        {
            ClientDescription = "";
            ApiUrl = "https://test.bitpay.com/";
            ApiVersion = "2.0.0";
            PrivateKeyPath = "";
            ApiTokens = new ApiTokens();
        }

        public string ClientDescription { get; set; }
        public string ApiUrl { get; set; }
        public string ApiVersion { get; set; }
        public string PrivateKeyPath { get; set; }
        public ApiTokens ApiTokens { get; set; }
    }

    public class Prod
    {
        public Prod()
        {
            ClientDescription = "";
            ApiUrl = "https://bitpay.com/";
            ApiVersion = "2.0.0";
            PrivateKeyPath = "";
            ApiTokens = new ApiTokens();
        }

        public string ClientDescription { get; set; }
        public string ApiUrl { get; set; }
        public string ApiVersion { get; set; }
        public string PrivateKeyPath { get; set; }
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