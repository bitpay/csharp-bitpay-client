namespace BitPayAPI
{
    public class PayoutInfo
    {
        public string Name { get; set; }
        public string Account { get; set; }
        public string Routing { get; set; }
        public string MerchantEIN { get; set; }
        public string Label { get; set; }
        public string BankCountry { get; set; }
    }
}