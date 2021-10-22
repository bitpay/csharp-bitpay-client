using System;
using Newtonsoft.Json;

namespace BitPaySDK.Models.Invoice
{
   public class ItemizedDetails
    {
        public int? Amount { get; set; }

        public string Description { get; set; }

        [JsonProperty(PropertyName = "isFee")]
        public bool? IsFee { get; set; }

        public bool ShouldSerializeAmount()
        {
            return false;
        }

        public bool ShouldSerializeDescription()
        {
            return !string.IsNullOrEmpty(Description);
        }

        public bool ShouldSerializeIsFee()
        {
            return false;
        }
    }
}
