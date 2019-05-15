using System;
using Xunit;
using BitPayAPI;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BitPayAPI.Exceptions;
using BitPayAPI.Models;
using BitPayAPI.Models.Invoice;
using Microsoft.Extensions.Configuration;

namespace BitPayXUnitTest
{
    public class Tests
    {
        // This is the BitPay object we're going to use through all the tests
        private BitPay _bitpay;
        // The pairing code generated in your BitPay account -
        // https://test.bitpay.com/dashboard/merchant/api-tokens
        // This is the POS Pairing Code
        private static readonly string PairingCode = "hWR3b8L";

        // Your favourite client name
        private static readonly string ClientName = "BitPay .Net Client v2.0.1904 Tester on " + Environment.MachineName;
        
        // Define the date range for fetching results during the test
        private static DateTime today = DateTime.Now;
        private static DateTime tomorrow = today.AddDays(1);
        private static DateTime yesterday = today.AddDays(-1);

        // Will store one of the generated invoices during the test
        // so it can be paid manually in order to pass the ledger tests
        
        public Tests()
        {
            // JSON minified with the BitPay configuration as in the required configuration file
            // and parsed into a IConfiguration object
            var json = "{\"BitPayConfiguration\": {\"Environment\": \"Test\",\"EnvConfig\": {\"Test\": {\"ClientDescription\": \"nettest070519\",\"ApiUrl\": \"https://test.bitpay.com/\",\"ApiVersion\": \"2.0.0\",\"PrivateKeyPath\": \"bitpay_private_test.key\",\"ApiTokens\": {\"pos\": \"BDP91QpWyWKJHyKU6KNPK761uThySwjX9xZs6kp53rei\",\"merchant\": \"2dPvsuvCVx2am6aeJHzYJB6dBbGtYoCpNbfT9HkULeNG\",\"payroll\": \"EbZxUi7RBZt2y3jx3A41rASKQzJi17xzxJQPuAwGNWxd\"}},\"Prod\": {\"ClientDescription\": \"\",\"ApiUrl\": \"https://bitpay.com/\",\"ApiVersion\": \"2.0.0\",\"PrivateKeyPath\": \"\",\"ApiTokens\": {\"pos\": \"\",\"merchant\": \"\",\"payroll\": \"\"}} }}}";
            var memoryJsonFile = new MemoryFileInfo("config.json", Encoding.UTF8.GetBytes(json), DateTimeOffset.Now);
            var memoryFileProvider = new MockFileProvider(memoryJsonFile);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(memoryFileProvider, "config.json", false, false)
                .Build();
            
            // Initialize the BitPay object to be used in the following tests
            _bitpay = new BitPay(configuration);

            // If the client doesn't have a POS token yet, fetch one.
            // For the Merchant and Payroll Facades, see below, in their corresponding tests
            if (!_bitpay.tokenExist(Facade.PointOfSale)) {
                _bitpay.AuthorizeClient(PairingCode);
            }

            // ledgers require the Merchant Facade
            if (!_bitpay.tokenExist(Facade.Merchant)) {
                // get a pairing code for the merchant facade for this client
                var pcode = _bitpay.RequestClientAuthorization(Facade.Merchant).Result;
                /* We can't continue. Please make sure you write down this pairing code, then goto
                    your BitPay account in the API Tokens section 
                    https://test.bitpay.com/dashboard/merchant/api-tokens    
                    and paste it into the search field.
                    It should get you to a page to approve it. After you approve it, run the tests
                    again and they should pass.
                 */
                throw new BitPayException("Please approve the pairing code " + pcode + " in your account.");
            }

            // ledgers require the Payroll Facade
            if (!_bitpay.tokenExist(Facade.Payroll)) {
                // get a pairing code for the merchant facade for this client
                var pcode = _bitpay.RequestClientAuthorization(Facade.Payroll).Result;
                /* We can't continue. Please make sure you write down this pairing code, then goto
                    your BitPay account in the API Tokens section 
                    https://test.bitpay.com/dashboard/merchant/api-tokens    
                    and paste it into the search field.
                    It should get you to a page to approve it. After you approve it, run the tests
                    again and they should pass.
                 */
                throw new BitPayException("Please approve the pairing code " + pcode + " in your account.");
            }
        }
        
        [Fact]
        public async Task TestShouldGetInvoiceId() 
        {
            // create an invoice and make sure we receive an id - which means it has been successfully submitted
            var invoice = new Invoice(50.0, "USD");
            var basicInvoice = await _bitpay.CreateInvoice(invoice);
            Assert.NotNull(basicInvoice.Id);
        }

        [Fact]
        public async Task TestShouldGetInvoiceUrl() {
            // create an invoice and make sure we receive an invoice url - which means we can check it online
            var basicInvoice = await _bitpay.CreateInvoice(new Invoice(10.0, "USD"));
            Assert.NotNull(basicInvoice.Url);
        }

        [Fact]
        public async Task TestShouldGetInvoiceStatus() {
            // create an invoice and make sure we receive a correct invoice status (new)
            var basicInvoice = await _bitpay.CreateInvoice(new Invoice(10.0, "USD"));
            Assert.Equal(Invoice.StatusNew, basicInvoice.Status);
        }

        [Fact]
        public async Task TestShouldGetInvoiceBtcPrice() {
            // create an invoice and make sure we receive values for the Bitcoin Cash and Bitcoin fields, respectively
            var basicInvoice = await _bitpay.CreateInvoice(new Invoice(10.0, "USD"));
            Assert.NotNull(basicInvoice.PaymentSubtotals.Btc);
            Assert.NotNull(basicInvoice.PaymentSubtotals.Bch);
        }

        [Fact]
        public async Task TestShouldCreateInvoiceOneTenthBtc() {
            // create an invoice and make sure we receive the correct price value back (under 1 BTC)
            var invoice = await _bitpay.CreateInvoice(new Invoice(0.1, "BTC"));
            Assert.Equal(0.1, invoice.Price);
        }

        [Fact]
        public async Task TestShouldCreateInvoice100Usd() {
            // create an invoice and make sure we receive the correct price value back (USD)
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, "USD"));
            Assert.Equal(100.0, invoice.Price);
        }

        [Fact]
        public async Task TestShouldCreateInvoice100Eur() {
            // create an invoice and make sure we receive the correct price value back (EUR)
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, "EUR"));
            Assert.Equal(100.0, invoice.Price);
        }

        [Fact]
        public async Task TestShouldGetInvoice() {
            // create an invoice then retrieve it through the get method - they should match
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, "EUR"));
            var retrievedInvoice = await _bitpay.GetInvoice(invoice.Id);
            Assert.Equal(invoice.Id, retrievedInvoice.Id);
        }
        
        [Fact]
        public async Task TestShouldGetInvoiceNoSigned() {
            // create an invoice then retrieve it through the get method - they should match
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, "EUR"), signRequest: false);
            var retrievedInvoice = await _bitpay.GetInvoice(invoice.Id, Facade.PointOfSale, false);
            Assert.Equal(invoice.Id, retrievedInvoice.Id);
        }

        [Fact]
        public async Task TestShouldCreateInvoiceWithAdditionalParams() {
            // create an invoice and make sure we receive the correct fields values back
            var invoice = new Invoice(100.0, "USD") {
                BuyerName = "Satoshi",
                PosData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"
            };
            invoice = await _bitpay.CreateInvoice(invoice);
            Assert.Equal(Invoice.StatusNew, invoice.Status);
            Assert.Equal("Satoshi", invoice.BuyerName);
            Assert.Equal("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", invoice.PosData);
        }

        [Fact]
        public async Task TestShouldGetExchangeRates() {
            // get the exchange rates
            var rates = await _bitpay.GetRates();
            Assert.NotNull(rates.GetRates());
        }

        [Fact]
        public async Task TestShouldGetUsdExchangeRate() {
            // get the exchange rates and check the USD value
            var rates = await _bitpay.GetRates();
            Assert.True(rates.GetRate("USD") != 0, "Exchange rate not retrieved: USD");
        }

        [Fact]
        public async Task TestShouldGetEurExchangeRate() {
            // get the exchange rates and check the EUR value
            var rates = await _bitpay.GetRates();
            Assert.True(rates.GetRate("EUR") != 0, "Exchange rate not retrieved: EUR");
        }

        [Fact]
        public async Task TestShouldGetCnyExchangeRate() {
            // get the exchange rates and check the CNY value
            var rates = await _bitpay.GetRates();
            Assert.True(rates.GetRate("CNY") != 0, "Exchange rate not retrieved: CNY");
        }

        [Fact]
        public async Task TestShouldUpdateExchangeRates() {
            // check the exchange rates after update
            var rates = await _bitpay.GetRates();
            await rates.Update();
            Assert.NotNull(rates.GetRates());
        }

        [Fact]
        public async Task TestShouldGetInvoiceIdOne() {
            // create an invoice and get it by its id
            var invoice = await _bitpay.CreateInvoice(new Invoice(1.0, "USD"), Facade.Merchant);
            invoice = await _bitpay.GetInvoice(invoice.Id);
            Assert.NotNull(invoice.Id);
        }

        [Fact]
        public async Task TestShouldGetInvoices() {
            // get invoices between two dates
            var invoices = await _bitpay.GetInvoices(yesterday, tomorrow);
            Assert.True(invoices.Count > 0, "No invoices retrieved");
        }

        [Fact]
        public async Task TestShouldGetLedgerBtc() {
            
            // make sure we get a ledger with a not null Entries property
            var ledger = await _bitpay.GetLedger(Ledger.LedgerBtc, yesterday, tomorrow);
            Assert.NotNull(ledger);
            Assert.NotNull(ledger.Entries);

        }

        [Fact]
        public async Task TestShouldGetLedgerUsd() {
            // Please see the comments from the GetBtcLedger concerning the Merchant facade

            // make sure we get a ledger with a not null Entries property
            var ledger = await _bitpay.GetLedger(Ledger.LedgerUsd, yesterday, tomorrow);
            Assert.NotNull(ledger);
            Assert.NotNull(ledger.Entries);

        }

        [Fact]
        public async Task TestShouldSubmitPayoutBatch() {

            var date = DateTime.Now;
            var threeDaysFromNow = date.AddDays(3);

            var effectiveDate = threeDaysFromNow;
            var reference = "My test batch";
            var bankTransferId = "My bank transfer id";
            var currency = "USD";
            var instructions = new List<PayoutInstruction>() {
                new PayoutInstruction(100.0, "mtHDtQtkEkRRB5mgeWpLhALsSbga3iZV6u", "Alice"),
                new PayoutInstruction(200.0, "mvR4Xj7MYT7GJcL93xAQbSZ2p4eHJV5F7A", "Bob")
            };

            var batch = new PayoutBatch(currency, effectiveDate, bankTransferId, reference, instructions);
            batch = await _bitpay.SubmitPayoutBatch(batch);

            Assert.NotNull(batch.Id);
            Assert.True(batch.Instructions.Count == 2);
        }

        [Fact]
        public async Task TestShouldSubmitGetAndDeletePayoutBatch() {

            /*
               Unfortunately at the time of this writing the Payroll facade is not available through the API
               so this test will always fail - since you can't approve the Payroll pairing code
             */

            var date = DateTime.Now;
            var threeDaysFromNow = date.AddDays(3);

            var effectiveDate = threeDaysFromNow;
            var reference = "My test batch";
            var bankTransferId = "My bank transfer id";
            var currency = "USD";
            var instructions = new List<PayoutInstruction>() {
                new PayoutInstruction(100.0, "mtHDtQtkEkRRB5mgeWpLhALsSbga3iZV6u", "Alice"),
                new PayoutInstruction(200.0, "mvR4Xj7MYT7GJcL93xAQbSZ2p4eHJV5F7A", "Bob")
            };

            var batch0 = new PayoutBatch(currency, effectiveDate, bankTransferId, reference, instructions);
            batch0 = await _bitpay.SubmitPayoutBatch(batch0);

            Assert.NotNull(batch0.Id);
            Assert.True(batch0.Instructions.Count == 2);

            var batch1 = await _bitpay.GetPayoutBatch(batch0.Id);

            Assert.NotNull(batch1.Id);
            Assert.True(batch1.Instructions.Count == 2);

            await _bitpay.CancelPayoutBatch(batch0.Id);

        }
    }
}