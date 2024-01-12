```
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using BitPay.Models.Bill;

namespace Examples.Merchant
{
    public class BillRequests : Requests
    {
        public void CreateBill()
        {
            var client = base.CreateClient();

            var bill = new Bill("USD", "someEmail@email.com", new List<Item>(), null) 
                {Name = "SomeName", Address1 = "SomeAddress", City = "SomeCity"};

            var createdBill = client.CreateBill(bill);
        }

        public void GetBill()
        {
            var client = base.CreateClient();

            var bill = client.GetBill("someBillId");

            var bills = client.GetBills("draft");
        }
        
        public void UpdateBill()
        {
            var client = base.CreateClient();

            var items = new List<Item>();
            var item = new Item(12.23M, 3);
            items.Add(item);
            
            var bill = new Bill("USD", "someEmail@email.com", items, null) 
                {Name = "SomeName", Address1 = "SomeAddress", City = "SomeCity"};

            var result = client.UpdateBill(bill, "someId");
        }
        
        public void DeliverBillViaEmail()
        {
            var client = base.CreateClient();

            var result = client.DeliverBill("someBillId", "myBillToken");
        }
    }
}
```