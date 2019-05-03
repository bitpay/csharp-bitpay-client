## Using the BitPay C# library

This SDK provides a convenient abstraction of BitPay's [cryptographically-secure API](https://bitpay.com/api) and allows payment gateway developers to focus on payment flow/e-commerce integration rather than on the specific details of client-server interaction using the API.  This SDK optionally provides the flexibility for developers to have control over important details, including the handling of private keys needed for client-server communication.

This SDK implements BitPay's remote client authentication and authorization strategy.  No private or shared-secret information is ever transmitted over the wire.

### Dependencies

You must have a BitPay merchant account to use this SDK.  It's free to [sign-up for a BitPay merchant account](https://bitpay.com/start).


### Handling your client private key

Each client paired with the BitPay server requires a ECDSA key.  This key provides the security mechanism for all client interaction with the BitPay server. The public key is used to derive the specific client identity that is displayed on your BitPay dashboard.  The public key is also used for securely signing all API requests from the client.  See the [BitPay API](https://bitpay.com/api) for more information.

The private key should be stored in the client environment such that it cannot be compromised.  If your private key is compromised you should revoke the compromised client identity from the BitPay server and re-pair your client, see the [API tokens](https://bitpay.com/api-tokens) for more information.

The [BitPay.Net Setup utility](https://github.com/bitpay/csharp-bitpay-client/releases/download/v2.0.1904/BitPay.Net_Setup_utility.zip) helps to generate the private key, as well as a environment file formatted in JSON which contains all configuration requirements, that should be stored in the client local file system. It is not recommended to transmit the private key over any public or unsecure networks.

Follow the guide [BitPay.Net Setup utility guide](https://github.com/bitpay/csharp-bitpay-client/blob/master/BitPaySetup/README.md) that assist you to create the environment file which you will be able to modify it, either manually or by using the BitPay.Net Setup utility, later on by asking you to provide the path to your existing JSON file.

### Initializing your BitPay client

Once you have the environment file (JSON previously generated) you can initialize the client on two different ways:

```c#
// Provide the full path to the env file which you have previously stored securely.

BitPay bitpay = new BitPay("BitPay.config.json");
```

```c#
// Provide a IConfiguration Interface containing the same structure as in the json file.

var configuration = new ConfigurationBuilder()
    .AddJsonFile("BitPay.config.json", false, false)
    .Build();
    
BitPay bitpay = new BitPay(configuration);
```

### Pair your client with BitPay

Your client must be paired with the BitPay server.  The pairing initializes authentication and authorization for your client to communicate with BitPay for your specific merchant account.

Pairing is accomplished by having the BitPay.Net Setup utility request a pairing code from the BitPay server.
Meanwhile a new pairing code is generated, the BitPay.Net Setup utility will ask you to activate it in your BitPay account. It will also store the paired token in the environment file.

The pairing code is then entered into the BitPay merchant dashboard for the desired merchant.  Your interactive authentication at https://bitpay.com/login provides the authentication needed to create finalize the client-server pairing request.

### Create an invoice

```c#
Invoice invoice = bitpay.createInvoice(100.0, "USD");

String invoiceUrl = invoice.getURL();

String status = invoice.getStatus();
```

### Create an invoice (extended)

You can add optional attributes to the invoice.  Atributes that are not set are ignored or given default values.
```c#
Invoice invoice = new Invoice(100.0, "USD");
invoice.BuyerName = "Satoshi";
invoice.BuyerEmail = "satoshi@example.com";
invoice.FullNotifications = true;
invoice.NotificationEmail = "satoshi@example.com";
invoice.PosData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

invoice = this.bitpay.createInvoice(invoice);
```

### Retreive an invoice

```c#
Invoice invoice = bitpay.getInvoice(invoice.getId());
```

### Get exchange rates

You can retrieve BitPay's [BBB exchange rates](https://bitpay.com/exchange-rates).

```c#
Rates rates = this.bitpay.getRates();

double rate = rates.getRate("USD");

rates.update();
```

See also the tests project for more examples of API calls.
