namespace BitPaySDK
{
    public class Env
    {
        public const string Test = "Test";
        public const string Prod = "Prod";
        public const string TestUrl = "https://test.bitpay.com/";
        public const string ProdUrl = "https://bitpay.com/";
        public const string BitpayApiVersion = "2.0.0";
        public const string BitpayPluginInfo = "BitPay_DotNet_Client_v3.3.2003";
        public const string BitpayApiFrame = "custom";
        public const string BitpayApiFrameVersion = "1.0.0";
        public class Tokens
        {
            public string Merchant { get; set; }
            public string Payout { get; set; }
        }
    }
}