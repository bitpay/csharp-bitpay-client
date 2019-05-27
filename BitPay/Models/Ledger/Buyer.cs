using Newtonsoft.Json;

namespace BitPayAPI.Models.Ledger
{
    /// <summary>
    ///     Provides an interface to a buyer.
    /// </summary>
    public class Buyer
    {
        [JsonProperty(PropertyName = "buyerName")] public string Name { get; set; }
        
        [JsonProperty(PropertyName = "buyerAddress1")] public string Address1 { get; set; }
        
        [JsonProperty(PropertyName = "buyerAddress2")] public string Address2 { get; set; }
        
        [JsonProperty(PropertyName = "buyerLocality")] public string Locality { get; set; }
        
        [JsonProperty(PropertyName = "buyerRegion")] public string Region { get; set; }
        
        [JsonProperty(PropertyName = "buyerPostalCode")] public string PostalCode { get; set; }
        
        [JsonProperty(PropertyName = "buyerCountry")] public string Country { get; set; }
        
        [JsonProperty(PropertyName = "buyerPhone")] public string Phone { get; set; }
        
        [JsonProperty(PropertyName = "buyerNotify")] public string Notify { get; set; }
        
        [JsonProperty(PropertyName = "buyerState")] public string State { get; set; }
        
        [JsonProperty(PropertyName = "buyerZip")] public string Zip { get; set; }
        
        [JsonProperty(PropertyName = "buyerCity")] public string City { get; set; }

        
        
        
//        [JsonProperty(PropertyName = "name")] public string Name { get; set; }
//
//        [JsonProperty(PropertyName = "address1")]
//        public string Address1 { get; set; }
//
//        [JsonProperty(PropertyName = "address2")]
//        public string Address2 { get; set; }
//
//        [JsonProperty(PropertyName = "city")] public string City { get; set; }
//
//        [JsonProperty(PropertyName = "state")] public string State { get; set; }
//
//        [JsonProperty(PropertyName = "zip")] public string zip { get; set; }
//
//        [JsonProperty(PropertyName = "country")]
//        public string country { get; set; }
//
//        [JsonProperty(PropertyName = "email")] public string email { get; set; }
//
//        [JsonProperty(PropertyName = "phone")] public string phone { get; set; }
    }
}