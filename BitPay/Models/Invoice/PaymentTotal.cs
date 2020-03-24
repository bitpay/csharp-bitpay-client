using System;

namespace BitPaySDK.Models.Invoice
{
    [Obsolete("PaymentTotal will be deprecated on version 4.0", false)]  //TODO remove on version 4.0
    public class PaymentTotal
    {
        public double Btc { get; set; }
        public double Bch { get; set; }
        public double Eth { get; set; }
        public double Usdc { get; set; }
        public double Gusd { get; set; }
        public double Pax { get; set; }
    }
}