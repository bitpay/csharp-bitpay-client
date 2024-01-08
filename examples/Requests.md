```
using System;

namespace Examples
{
    public class Requests
    {
        protected Client CreateClient()
        {
            String privateKey =
                "75371435315047800683080420474719166774492308988314944856528163960396135344086";
            String merchantToken = "merchantToken";
            String payoutToken = "payoutToken";
            AccessTokens tokens = new AccessTokens();
            tokens.AddMerchant(merchantToken);
            tokens.AddPayout(payoutToken);
            
            return new Client(new PrivateKey(privateKey), tokens, Environment.Test);
        }
    }
}
```