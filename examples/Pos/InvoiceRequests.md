```
using System;
using System.Collections.Generic;

using BitPay.Models.Invoice;

namespace Examples.Pos
{
    public class InvoiceRequests
    {
        public void CreateInvoice()
        {
            var client = GetClient();
            
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
            var client = GetClient();

            var invoice = client.GetInvoice("myInvoiceId");
        }
        
        private static Client GetClient()
        {
            return new(new PosToken("someToken"));
        }
    }
}
```