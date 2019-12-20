## Using the BitPay.Net Setup utility

This utility will guide you through all needed steps in order to create your private key and authentication tokens to work with the BitPay Client and BitPay server.

The output of this utility results on a JSON file which contains the complete definition of your environment, being able to choose between Testing or Production environments, create/update API authentication tokens and client names, generate private keys adn configure the targeted version and endpoint of the BitPay API.

### Dependencies

You must have a BitPay merchant account to use this utility.  It's free to [sign-up for a BitPay merchant account](https://bitpay.com/start).
For Testing purposes you can create a testing merchant account [Here](https://test.bitpay.com/start)

### First things first

**IMPORTANT:** If this is the first time you set up your client, follow the below steps in order.

### Selecting the environment file name and target

Once you open the utility you will be asked to provide the path and filename of the new (or existing) environment file.

![alt Select/create env file](https://raw.githubusercontent.com/bitpay/csharp-bitpay-client/master/screenshots/utility-setup-init.png)

Then the environment that you are configuring for.

![alt Select environment](https://raw.githubusercontent.com/bitpay/csharp-bitpay-client/master/screenshots/utility-setup-env.png)

After selecting the environment, the following message will show up. All notifications will show on top of the menu so, please, read carefully.

> IMPORTANT: If this is the first time you set up your client, follow the below steps in order.

### BitPay.Net Setup utility main menu

![alt Main menu](https://raw.githubusercontent.com/bitpay/csharp-bitpay-client/master/screenshots/utility-setup-menu.png)

### Select/Create a private key

**Generate Private Key** option in the main menu

**IMPORTANT:** This Private Key is used to sign every request to the BitPay API and it can not be shared or exposed under any circumstances.

The first option in the menu will ask you to provide the path and filename to a existing private key file, if you did not create a private key yet, the new key will be stored in the given path with the given name.

If you press Enter without entering a file path, the private key will be generated in the root of the BitPay Setup Utility with the following name:

- "bitpay_private_test.key" if the current targeted environment is Testing
- "bitpay_private_prod.key" if the current targeted environment is Production

A notification will be shown with the public key of the generated private key and this means that the key has been successfully stored.

### Giving a name to the client

**Set client description for environment** option in the main menu

This step is not mandatory as you will be asked to provide a name once you approve a new pairing code in your BitPay account if you did not defined one before.
This name will help you to identify your client in the API tokens overview in your BitPay account.

### Request a pairing code

**Get pairing code and token** option in the main menu

Here you can request a pairing code for each facade your account is configured for.

![alt Select facade](https://raw.githubusercontent.com/bitpay/csharp-bitpay-client/master/screenshots/utility-setup-facade.png)

Meanwhile a new pairing code is generated, the BitPay.Net Setup utility will ask you to approve it in your BitPay account. It will also store the paired token in the environment file.

![alt Approve pairing code](https://raw.githubusercontent.com/bitpay/csharp-bitpay-client/master/screenshots/utility-setup-pair.png)

### Change the targeted environment

You can move to a different environment to configure it by repeating the process.

### Setup completed

Once you have gone through all steps, the environment file is ready to be used with the BitPay client and it is located in the file path that you entered in the beginning of this setup or in the root os the BitPay.Net Setup Utility with the name "BitPay.config.json" if you did not enter one.

### Environment configuration file explained

```json
{
  "BitPayConfiguration": {
    "Environment": "Prod",
    "EnvConfig": {
      "Test": {
        "PrivateKeyPath": "/full-directory/bitpay_private_test.key",
        "ApiTokens": {
          "merchant": "AjrKe6WZs6D3D37vkYM3Q1AED4TU4yZGs99BtDzcJuT7",
          "payroll": "wZ4Q7GZdETCiz3mVVFVitFWDcdp6bppEVRYcBTxjkGg"
        }
      },
      "Prod": {
        "PrivateKeyPath": "/full-directory/bitpay_private_prod.key",
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

- **Environment:** This value points the client on the direction of the **targeted environment**, which can be set to either **Test** or **Prod**.
- **EnvConfig:** Holds the configuration of each available environment.
- **PrivateKeyPath:** Points to the path where the private key file is located.**
- **ApiTokens:** Holds the token for each one of the facades that you are allowed to use.
<br/><sub><sub>* Do not change unless BitPay asks you to </sub></sub>
<br/><sub><sub>** Private Key should be securely stored </sub></sub>