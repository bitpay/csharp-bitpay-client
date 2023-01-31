namespace BitPay.Models.Invoice
{
    public class InvoiceBuyerProvidedInfo
    {

        public string Name { get; set; }
        
        public string Sms { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public string SelectedWallet { get; set; }

        public string SelectedTransactionCurrency { get; set; }
    }
}
