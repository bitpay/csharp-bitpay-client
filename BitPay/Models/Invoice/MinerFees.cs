namespace BitPaySDK.Models.Invoice
{
    public class MinerFees
    {
        public MinerFeesItem Btc { get; set; }
        public MinerFeesItem Bch { get; set; }
        public MinerFeesItem Eth { get; set; }
    }
}