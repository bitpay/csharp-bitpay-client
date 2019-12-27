namespace BitPaySDK.Models.Invoice
{
    public class PaymentCodes
    {
        public PaymentCode Btc { get; set; }
        public PaymentCode Bch { get; set; }
        public PaymentCode Eth { get; set; }
        public PaymentCode Usdc { get; set; }
        public PaymentCode Gusd { get; set; }
        public PaymentCode Pax { get; set; }
    }
}