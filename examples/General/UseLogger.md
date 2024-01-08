```
using BitPay;
using BitPay.Logger;
using BitPay.Models.Invoice;

namespace Examples
{
    public class UseLogger
    {
        public void execute()
        {
            IBitPayLogger logger = new SomeLogger();
            LoggerProvider.SetLogger(logger);

            Client client = new Client(new PosToken("someToken"));
            Invoice invoice = client.GetInvoice("someInvoiceId").Result;
        }
    }

    public class SomeLogger : IBitPayLogger
    {
        public void LogRequest(string method, string endpoint, string? json)
        {
            // some implementation
        }

        public void LogResponse(string method, string endpoint, string? json)
        {
            // some implementation
        }

        public void LogError(string message)
        {
            // some implementation
        }
    }
}
```