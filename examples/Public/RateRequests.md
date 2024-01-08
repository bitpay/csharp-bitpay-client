```
namespace Examples.Public
{
    public class RateRequests
    {
        public void GetRate()
        {
            var client = new Client(new PosToken("anyToken"));

            var allRates = client.GetRates();

            var currencyRates = client.GetRates("BTC");

            var currencyPairRate = client.GetRate("BTC", "USD");
        }
    }
}
```