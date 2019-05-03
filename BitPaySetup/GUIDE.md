## Using the BitPay.Net Setup utility

This utility will guide you through all needed steps in order to create your private key and authentication tokens to work with the BitPay Client and BitPay server.

The output of this utility results on a JSON file which contains the complete definition of your environment, being able to choose between Testing or Production environments, create/update API authentication tokens and client names, generate private keys adn configure the target version and endpoint of the BitPay API.

**Env configuration example:**
```json
{
  "BitPayConfiguration": {
    "Environment": "Prod",
    "EnvConfig": {
      "Test": {
        "ClientDescription": "Testing client",
        "ApiUrl": "https://test.bitpay.com/",
        "ApiVersion": "2.0.0",
        "PrivateKeyPath": "bitpay_private_test.key",
        "ApiTokens": {
          "pos": "6KrZJk6yY69r67bnHmMymCk4ch9NrWB6pf8BjPHZrciU",
          "merchant": "AjrKe6WZs6D3D37vkYM3Q1AED4TU4yZGs99BtDzcJuT7",
          "payroll": "wZ4Q7GZdETCiz3mVVFVitFWDcdp6bppEVRYcBTxjkGg"
        }
      },
      "Prod": {
        "ClientDescription": "Production client",
        "ApiUrl": "https://bitpay.com/",
        "ApiVersion": "2.0.0",
        "PrivateKeyPath": "bitpay_private_prod.key",
        "ApiTokens": {
          "pos": "AgwE4UPVBwZwcZAWiNLPQewD4AKMCzTQxjTsDTHt7esi",
          "merchant": "9PLzuhu6X2wRNqDWVzot7V",
          "payroll": "9hYxJgUUnmq5hXGC6rR4CF"
        }
      }
    }
  }
}
```

### Dependencies

You must have a BitPay merchant account to use this utility.  It's free to [sign-up for a BitPay merchant account](https://bitpay.com/start).
For Testing purposes you can create a testing merchant account [Here](https://test.bitpay.com/start)

### First things first

IMPORTANT: If this is the first time you set up your client, follow the below steps in order.
``
### Selecting the environment file name

Once you open the utility you will be asked to provide the path and filename of the new (or existing) environment file.

![alt Select/create env file](https://raw.githubusercontent.com/bitpay/csharp-bitpay-client/master/screenshots/utility-setup-init)

Then the environment that you are configuring for.

![alt Select environment](https://raw.githubusercontent.com/bitpay/csharp-bitpay-client/master/screenshots/utility-setup-env)

After selecting the environment, the following message will show up. All notifications will show on top of the menu so, please, read carefully.

> IMPORTANT: If this is the first time you set up your client, follow the below steps in order.