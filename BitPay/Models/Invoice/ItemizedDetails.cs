// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
   public class ItemizedDetails
    {
        [JsonProperty(PropertyName = "amount")]
        public decimal? Amount { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string? Description { get; set; }

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
