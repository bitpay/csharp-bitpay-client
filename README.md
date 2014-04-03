©2013,2014 BITPAY, INC.

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


Installation
------------
Import these files into your custom shopping cart implementation project.


Configuration
-------------
1. Create an API key at bitpay.com by clicking My Account > API Access Keys > Add New API Key


Usage
-----
1. See API documentation at https://bitpay.com/bitcoin-payment-gateway-api


Troubleshooting
---------------
The official BitPay API documentation should always be your first reference for development:
https://bitpay.com/downloads/bitpayApi.pdf

1. Verify that your "notificationURL" for the invoice is "https://" (not "http://")
2. Ensure a valid SSL certificate is installed on your server. Also ensure your root CA cert is
   updated. If your CA cert is not current, you will see curl SSL verification errors.
3. Verify that your callback handler at the "notificationURL" is properly receiving POSTs. You
   can verify this by POSTing your own messages to the server from a tool like Chrome Postman.
4. Verify that the POST data received is properly parsed and that the logic that updates the
   order status on the merchants web server is correct.
5. Verify that the merchants web server is not blocking POSTs from servers it may not
   recognize. Double check this on the firewall as well, if one is being used.
6. Use the logging functionality to log errors during development. If you contact BitPay support,
   they will ask to see the log file to help diagnose any problems.
7. Check the version of this PHP library agains the official repository to ensure you are using
   the latest version. Your issue might have been addressed in a newer version of the library.
8. If all else fails, send an email describing your issue *in detail* to support@bitpay.com


Change Log
----------
Version 1
  - Initial version, Supported with Visual Studio 2012, .Net 4.5

Version 1.1, rich@bitpay.com
  - Improved documentation
