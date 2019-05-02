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

```c#
// Let the SDK store the private key on the clients local file system.
BitPay bitpay = new BitPay();
```

```c#
// Create the private key using the SDK, store it as required, and inject the private key into the SDK.
ECKey key = KeyUtils.createEcKey();
this.bitpay = new BitPay(key);
```

```c#
// Create the private key external to the SDK, store it in a file, and inject the private key into the SDK.
String privateKey = KeyUtils.getKeyStringFromFile(privateKeyFile);
ECKey key = KeyUtils.createEcKeyFromHexString(privateKey);
this.bitpay = new BitPay(key);
```

### Pair your client with BitPay

Your client must be paired with the BitPay server.  The pairing initializes authentication and authorization for your client to communicate with BitPay for your specific merchant account.  There are two pairing modes available; client initiated and server initiated.

### Client initiated pairing

Pairing is accomplished by having your client request a pairing code from the BitPay server.  The pairing code is then entered into the BitPay merchant dashboard for the desired merchant.  Your interactive authentication at https://bitpay.com/login provides the authentication needed to create finalize the client-server pairing request.

```c#
String clientName = "server 1";
BitPay bitpay = new BitPay(clientName);        
        
if (!bitpay.clientIsAuthorized(BitPay.FACADE_POS))
{
  // Get POS facade authorization code.
  String pairingCode = bitpay.requestClientAuthorization(BitPay.FACADE_POS);
  
  // Signal the device operator that this client needs to be paired with a merchant account.
  System.Diagnostics.Debug.WriteLine("Info: Pair this client with your merchant account using the pairing code: " + pairingCode);
  throw new BitPayException("Error: client is not authorized for POS facade.");
  
  // At this point you need to go to your account in the [api-tokens section](https://bitpay.com/dashboard/merchant/api-tokens) , search for the resulted pairing code and approve it. After this step, you can safely start making requests for this facade.
}
```

### Server initiated pairing

Pairing is accomplished by obtaining a pairing code from the BitPay server.  The pairing code is then injected into your client (typically during client initialization/configuration).  Your interactive authentication at https://bitpay.com/login provides the authentication needed to create finalize the client-server pairing request.

```c#
// Obtain a pairingCode from your BitPay account administrator. 
String pairingCode = "xxxxxxx";
String clientName = "server 1";
BitPay bitpay = new BitPay(clientName);

// Is this client already authorized to use the POS facade?
if (!bitpay.clientIsAuthorized(BitPay.FACADE_POS))
{
  // Get POS facade authorization.
  bitpay.authorizeClient(pairingCode);
}	
```

### Create an invoice

```c#
Invoice invoice = bitpay.createInvoice(100.0m, "USD");

String invoiceUrl = invoice.getURL();

String status = invoice.getStatus();
```

### Create an invoice (extended)

You can add optional attributes to the invoice.  Atributes that are not set are ignored or given default values.
```c#
Invoice invoice = new Invoice(100.0m, "USD");
invoice.BuyerName = "Satoshi";
invoice.BuyerEmail = "satoshi@bitpay.com";
invoice.FullNotifications = true;
invoice.NotificationEmail = "satoshi@bitpay.com";
invoice.PosData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

invoice = this.bitpay.createInvoice(invoice);
```

### Retreive an invoice

```c#
Invoice invoice = bitpay.getInvoice(invoice.getId());
```

### Get exchange rates

You can retrieve BitPay's [BBB exchange rates](https://bitpay.com/bitcoin-exchange-rates).

```c#
Rates rates = this.bitpay.getRates();

double rate = rates.getRate("USD");

rates.update();
```

See also the tests project for more examples of API calls.
