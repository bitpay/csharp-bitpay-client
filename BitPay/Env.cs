namespace BitPaySDK
{
    public class Env
    {
        public const string Test = "Test";
        public const string Prod = "Prod";
        public const string TestUrl = "https://test.bitpay.com/";
        public const string ProdUrl = "https://bitpay.com/";
        public const string BitpayApiVersion = "2.0.0";
        public const string BitpayPluginInfo = "BitPay_DotNet_Client_v3.1.1912";
        public class Tokens
        {
            public string POS { get; set; }
            public string Merchant { get; set; }
            public string Payout { get; set; }
        }
    }
}