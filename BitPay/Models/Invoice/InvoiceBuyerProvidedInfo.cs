using Newtonsoft.Json;

namespace BitPaySDK.Models.Invoice
{
    public class InvoiceBuyerProvidedInfo
    {

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public string SelectedWallet { get; set; }

        public string Sms { get; set; }

        [JsonProperty(PropertyName = "smsVerified")]
        public bool SmsVerified { get; set; }

        public string SetSelectedTransactionCurrency { get; set; }
    }
}
