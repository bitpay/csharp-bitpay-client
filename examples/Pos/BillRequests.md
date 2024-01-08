```
using System.Collections.Generic;

using BitPay.Models.Bill;

namespace Examples.Pos
{
    public class BillRequests
    {
        public void CreateBill()
        {
            var client = GetClient();

            var bill = new Bill("USD", "someEmail@email.com", new List<Item>(), null) 
                {Name = "SomeName", Address1 = "SomeAddress", City = "SomeCity"};

            var createdBill = client.CreateBill(bill);
        }

        public void GetBill()
        {
            var client = GetClient();

            var bill = client.GetBill("someBillId");

            var bills = client.GetBills("draft");
        }
        
        public void DeliverBillViaEmail()
        {
            var client = GetClient();

            var result = client.DeliverBill("someBillId", "myBillToken");
        }
        
        private static Client GetClient()
        {
            return new(new PosToken("someToken"));
        }
    }
}
```