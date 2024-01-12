```
using BitPay.Models.Invoice;

namespace Examples.Merchant
{
    public class RefundRequests : Requests
    {
        public void CreateRefund()
        {
            var client = base.CreateClient();

            var refund = new Refund("myInvoiceId", 11.23M) { Currency = "USD"};

            var result = client.CreateRefund(refund);
        }
        
        public void UpdateRefund()
        {
            var client = base.CreateClient();

            var updateByRefund = client.UpdateRefund("someId", "created");
            
            var updateByRefundByGuid = client.UpdateRefundByGuid("someGuid", "created");
        }
        
        public void GetRefund()
        {
            var client = base.CreateClient();

            var refund = client.GetRefund("someRefundId");

            var refundByGuid = client.GetRefundByGuid("someGuid");
        }
        
        public void CancelRefund()
        {
            var client = base.CreateClient();

            var cancelRefund = client.CancelRefund("someRefundId");

            var cancelRefundByGuid = client.CancelRefundByGuid("someGuid");
        }
        
        public void RequestRefundNotificationToBeResent()
        {
            var client = base.CreateClient();

            var result = client.SendRefundNotification("someRefundId");
        }
    }
}
```