using System;

namespace BitPaySDK.Models.Invoice
{
    [Obsolete("PaymentCode will be deprecated on version 4.0", false)]  //TODO remove on version 4.0
    public class PaymentCode
    {
        public string Bip72b { get; set; }
        public string Bip73 { get; set; }
        public string Eip681 { get; set; }
        public string Eip681b { get; set; }
    }
}