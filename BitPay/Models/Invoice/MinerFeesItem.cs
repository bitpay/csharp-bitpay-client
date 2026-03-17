// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class MinerFeesItem
    {
        [JsonProperty(PropertyName = "satoshisPerByte")]
        public decimal? SatoshisPerByte { get; set; }
        
        [JsonProperty(PropertyName = "totalFee")]
        public int? TotalFee { get; set; }
        
        [JsonProperty(PropertyName = "fiatAmount")]
        public decimal? FiatAmount { get; set; }
        
        public bool ShouldSerializeFiatAmount()
        {
            return FiatAmount.HasValue;
        }
        
        public bool ShouldSerializeSatoshisPerByte()
        {
            return FiatAmount.HasValue;
        }
        
        public bool ShouldSerializeTotalFee()
        {
            return FiatAmount.HasValue;
        }
    }
}
