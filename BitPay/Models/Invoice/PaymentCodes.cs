using System;

namespace BitPaySDK.Models.Invoice
{
    [Obsolete("PaymentCodes will be deprecated on version 4.0", false)]  //TODO remove on version 4.0
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