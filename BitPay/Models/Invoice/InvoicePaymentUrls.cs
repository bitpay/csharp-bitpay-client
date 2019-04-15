namespace BitPayAPI.Models.Invoice
{
    /// <summary>
    ///     Invoice payment URLs identified by BIP format.
    /// </summary>
    public class InvoicePaymentUrls
    {
        public string BIP21 { get; set; }
        public string BIP72 { get; set; }
        public string BIP72b { get; set; }
        public string BIP73 { get; set; }
    }
}