©2013,2014 BITPAY, INC.

The MIT License

Permission is hereby granted to any person obtaining a copy of this software
and associated documentation for use and/or modification in association with
the bitpay.com service.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.


Bitcoin C# payment library using the bitpay.com service.

This library implementation is provided as a sample and may not be complete or fit for your implementation.  This library may or may not implement the complete BitPay API specification.


Installation
------------
Import these files into your C# implementation solution or project, or create a redistributable library file from this sample library for import into your projects.


Configuration
-------------

BitPayTest:
1. Create an API pairing code at https://bitpay.com/api-tokens.  Click "Add new token.
2. Copy the pairing code into BitPayTest.cs "pairingCode".

BitPayTest2:
1. Run the test.  The test will fail but produce an output message with a pairing code.
2. Access your BitPay dashboard at https://bitpay.com/api-tokens.
3. Enter the pairing code output from (1) and approve it for use.
4. Rerun the test.


Usage
-----
1. See API documentation at https://bitpay.com/api


Troubleshooting
---------------
The official BitPay API documentation should always be your first reference for development:
https://bitpay.com/api

1. Verify that your "notificationURL" for the invoice is "https://" (not "http://")
2. Ensure a valid SSL certificate is installed on your server. Also ensure your root CA cert is
   updated.
3. Verify that your callback handler at the "notificationURL" is properly receiving POSTs. You
   can verify this by POSTing your own messages to the server from a tool like Chrome Postman.
4. Verify that the POST data received is properly parsed and that the logic that updates the
   order status on the merchants web server is correct.
5. Verify that the merchants web server is not blocking POSTs from servers it may not
   recognize. Double check this on the firewall as well, if one is being used.
6. Use the logging functionality to log errors during development. If you contact BitPay support,
   they will ask to see the log file to help diagnose any problems.
7. Check the version of this library against the official repository to ensure you are using
   the latest version. Your issue might have been addressed in a newer version of the library.
8. If all else fails, send an email describing your issue *in detail* to support@bitpay.com


Change Log
----------
Version 2.0.0, andy@bitpay.com
  - Updated to match BitPay Cryptographically Secure API specification at bitpay.com/api

Version 1.1.0, rich@bitpay.com
  - Improved documentation

Version 1.0.0
  - Initial version, Supported with Visual Studio 2012, .Net 4.5
