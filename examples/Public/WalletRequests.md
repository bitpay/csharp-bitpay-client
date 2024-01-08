```
namespace Examples.Public
{
    public class WalletRequests
    {
        public void GetSupportedWallets()
        {
            var client = new Client(new PosToken("anyToken"));

            var result = client.GetSupportedWallets();
        }
    }
}
```