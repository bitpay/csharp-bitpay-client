```
using System.Collections.Generic;

namespace Examples.Payout
{
    public class PayoutRequests : Requests
    {
        public void CreatePayout()
        {
            var client = base.CreateClient();

            var payoutToCreate = new Models.Payout.Payout(12.23M, "USD", "GBP")
            {
                NotificationEmail = "my@email.com", NotificationUrl = "https://my-url.com"
            };

            var payout = client.SubmitPayout(payoutToCreate);
            
            var payouts = client.SubmitPayouts(new List<Models.Payout.Payout> {payoutToCreate});
        }
        
        public void GetPayout()
        {
            var client = base.CreateClient();

            var payout = client.GetPayout("somePayoutId");
            
            var parameters = new Dictionary<string, dynamic?>
            {
                {"dateStart", "2023-08-14"},
                {"dateEnd", "2023-08-17"}
            };
            var payouts = client.GetPayouts(parameters);
        }
        
        public void CancelPayout()
        {
            var client = base.CreateClient();

            var cancelPayout = client.CancelPayout("somePayoutId");

            // var payoutGroupId = payout.GroupId;
            var cancelPayouts = client.CancelPayouts("payoutGroupId");
        }
        
        public void RequestPayoutWebhookToBeResent()
        {
            var client = base.CreateClient();

            var result = client.RequestPayoutNotification("somePayoutId");
        }
    }
}
```