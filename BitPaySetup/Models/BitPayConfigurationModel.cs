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
            payroll = "";
        }

        public string merchant { get; set; }
        public string payroll { get; set; }
    }

    public class Test
    {
        public Test()
        {
            PrivateKeyPath = "";
            ApiTokens = new ApiTokens();
        }

        public string PrivateKeyPath { get; set; }
        public ApiTokens ApiTokens { get; set; }
    }

    public class Prod
    {
        public Prod()
        {
            PrivateKeyPath = "";
            ApiTokens = new ApiTokens();
        }
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