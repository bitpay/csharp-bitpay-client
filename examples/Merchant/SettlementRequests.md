```
using System.Collections.Generic;

using BitPay.Models.Settlement;

namespace Examples.Merchant
{
    public class SettlementRequests : Requests
    {
        public void GetSettlement()
        {
            var client = base.CreateClient();

            var settlement = client.GetSettlement("someSettlementId");
            
            var parameters = new Dictionary<string, dynamic?>
            {
                {"currency", "USD"},
                {"dateStart", "2023-08-14"},
                {"dateEnd", "2023-08-17"}
            };
            var settlements = client.GetSettlements(parameters);
        }

        public void FetchReconciliationReport()
        {
            var client = base.CreateClient();

            var settlement = new Settlement();

            var result = client.GetSettlementReconciliationReport("settlementId", "settlementToken");
        }
    }
}
```