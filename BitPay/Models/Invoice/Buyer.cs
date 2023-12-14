// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class Buyer
    {
   
        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "address1")]
        public string? Address1 { get; set; }

        [JsonProperty(PropertyName = "address2")]
        public string? Address2 { get; set; }

        [JsonProperty(PropertyName = "locality")]
        public string? Locality { get; set; }

        [JsonProperty(PropertyName = "region")]
        public string? Region { get; set; }

        [JsonProperty(PropertyName = "postalCode")]
        public string? PostalCode { get; set; }
        
        [JsonProperty(PropertyName = "country")]
        public string? Country { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string? Email { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string? Phone { get; set; }

        [JsonProperty(PropertyName = "notify")]
        public bool? Notify { get; set; }
        
        public bool ShouldSerializeName()
        {
            return !string.IsNullOrEmpty(Name);
        }
        
        public bool ShouldSerializeAddress1()
        {
            return !string.IsNullOrEmpty(Address1);
        }
        
        public bool ShouldSerializeAddress2()
        {
            return !string.IsNullOrEmpty(Address2);
        }
        
        public bool ShouldSerializeLocality()
        {
            return !string.IsNullOrEmpty(Locality);
        }
        
        public bool ShouldSerializeRegion()
        {
            return !string.IsNullOrEmpty(Region);
        }
        
        public bool ShouldSerializePostalCode()
        {
            return !string.IsNullOrEmpty(PostalCode);
        }
        
        public bool ShouldSerializeCountry()
        {
            return !string.IsNullOrEmpty(Country);
        }
        
        public bool ShouldSerializeEmail()
        {
            return !string.IsNullOrEmpty(Email);
        }
        
        public bool ShouldSerializePhone()
        {
            return !string.IsNullOrEmpty(Phone);
        }
        
        public bool ShouldSerializeNotify()
        {
            return Notify.GetValueOrDefault(false);
        }
    }
}
