// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class MinerFees
    {
        [JsonProperty(PropertyName = "BTC")]
        public MinerFeesItem? Btc { get; set; }
        
        [JsonProperty(PropertyName = "BCH")]
        public MinerFeesItem? Bch { get; set; }
        
        [JsonProperty(PropertyName = "ETH")]
        public MinerFeesItem? Eth { get; set; }
        
        [JsonProperty(PropertyName = "USDC")]
        public MinerFeesItem? Usdc { get; set; }
        
        [JsonProperty(PropertyName = "GUSD")]
        public MinerFeesItem? Gusd { get; set; }
        
        [JsonProperty(PropertyName = "PAX")]
        public MinerFeesItem? Pax { get; set; }
        
        [JsonProperty(PropertyName = "BUSD")]
        public MinerFeesItem? Busd { get; set; }
        
        [JsonProperty(PropertyName = "XRP")]
        public MinerFeesItem? Xrp { get; set; }
        
        [JsonProperty(PropertyName = "DOGE")]
        public MinerFeesItem? Doge { get; set; }
        
        [JsonProperty(PropertyName = "LTC")]
        public MinerFeesItem? Ltc { get; set; }
        
        [JsonProperty(PropertyName = "DAI")]
        public MinerFeesItem? Dai { get; set; }
        
        [JsonProperty(PropertyName = "WBTC")]
        public MinerFeesItem? Wbtc { get; set; }
        
        [JsonProperty(PropertyName = "MATIC")]
        public MinerFeesItem? Shib { get; set; }
        
        [JsonProperty(PropertyName = "USDC_m")]
        public MinerFeesItem? Usdcm { get; set; }
        
        public bool ShouldSerializeBch()
        {
            return (Bch != null);
        }
        
        public bool ShouldSerializeBtc()
        {
            return (Btc != null);
        }
        
        public bool ShouldSerializeBusd()
        {
            return (Busd != null);
        }
        
        public bool ShouldSerializeDai()
        {
            return (Dai != null);
        }
        
        public bool ShouldSerializeDoge()
        {
            return (Doge != null);
        }
        
        public bool ShouldSerializeEth()
        {
            return (Eth != null);
        }
        
        public bool ShouldSerializeGusd()
        {
            return (Gusd != null);
        }
        
        public bool ShouldSerializeLtc()
        {
            return (Ltc != null);
        }
        
        public bool ShouldSerializePax()
        {
            return (Pax != null);
        }
        
        public bool ShouldSerializeShib()
        {
            return (Shib != null);
        }
        
        public bool ShouldSerializeUsdc()
        {
            return (Usdc != null);
        }
        
        public bool ShouldSerializeUsdcm()
        {
            return (Usdcm != null);
        }
        
        public bool ShouldSerializeWbtc()
        {
            return (Wbtc != null);
        }
        
        public bool ShouldSerializeXrp()
        {
            return (Xrp != null);
        }
    }
}
