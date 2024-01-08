```
using System.Collections.Generic;

using BitPay.Models.Payout;

namespace Examples.Payout
{
    public class RecipientRequests : Requests
    {
        public void InviteRecipients()
        {
            var client = base.CreateClient();

            var payoutRecipient = new PayoutRecipient("some@email.com", null);

            var result = client.SubmitPayoutRecipients(
                new PayoutRecipients(
                    new List<PayoutRecipient> {payoutRecipient}
            ));
        }
       
        public void GetRecipient()
        {
            var client = base.CreateClient();

            var result = client.GetPayoutRecipient("recipientId");
        }
       
        public void UpdateRecipient()
        {
            var client = base.CreateClient();
            
            var payoutRecipient = new PayoutRecipient("some@email.com", null);

            var result = client.UpdatePayoutRecipient("someRecipientId", payoutRecipient);
        }
       
        public void RemoveRecipient()
        {
            var client = base.CreateClient();

            var result = client.DeletePayoutRecipient("someRecipientId");
        }
    }
}
```