```
using System;
using System.Buffers.Text;

namespace Examples.Merchant
{
    public class LedgerRequests : Requests
    {
        public void GetLedgers()
        {
            var client = base.CreateClient();

            var result = client.GetLedgers();
        }

        public void GetLedgerEntries()
        {
            var client = base.CreateClient();
            
            var today = DateTime.Now;

            var result = client.GetLedgerEntries("USD", today.AddDays(-1), today);
        }
    }
}
```