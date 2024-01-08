```
using System;
using System.Collections.Generic;

using BitPay.Models.Invoice;

namespace Examples.Merchant
{
    public class InvoiceRequests : Requests
    {
        public void CreateInvoice()
        {
            var client = base.CreateClient();
            
            var buyer = new Buyer
            {
                Name = "Test",
                Email = "test@email.com",
                Address1 = "168 General Grove",
                Country = "AD",
                Locality = "Port Horizon",
                Notify = true,
                Phone = "+990123456789",
                PostalCode = "KY7 1TH",
                Region = "New Port"
            };

            var invoice = new Invoice(12.34M, "USD")
            {
                FullNotifications = true,
                ExtendedNotifications = true,
                NotificationUrl = "https://test/lJnJg9WW7MtG9GZlPVdj",
                RedirectUrl = "https://test/lJnJg9WW7MtG9GZlPVdj",
                NotificationEmail = "my@email.com",
                Buyer = buyer
            };

            var result = client.CreateInvoice(invoice);
        }
        
        public void GetInvoice()
        {
            var client = base.CreateClient();

            var invoiceById = client.GetInvoice("myInvoiceId");

            var invoiceByGuid = client.GetInvoiceByGuid("someGuid");  // we can add a GUID during the invoice creation

            var today = DateTime.Now;
            var invoices = client.GetInvoices(today.AddDays(-1), today);
        }
        
        public void UpdateInvoice()
        {
            var client = base.CreateClient();
            var parameters = new Dictionary<string, dynamic?> {{"buyerEmail", "some@email.com"}};

            var result = client.UpdateInvoice("someId", parameters);
        }
        
        public void CancelInvoice()
        {
            var client = base.CreateClient();

            var cancel = client.CancelInvoice("someId");

            var cancelByGuid = client.CancelInvoiceByGuid("someGuid");
        }
        
        public void RequestInvoiceWebhookToBeResent()
        {
            var client = base.CreateClient();

            var result = client.RequestInvoiceWebhookToBeResent("someInvoiceId");
        }
    }
}
```