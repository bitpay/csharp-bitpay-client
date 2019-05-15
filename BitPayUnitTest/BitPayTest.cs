using BitPayAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BitPayAPI.Exceptions;
using BitPayAPI.Models;
using BitPayAPI.Models.Invoice;
using Microsoft.Extensions.Configuration;

namespace BitPayUnitTest {

    [TestClass]
    public class BitPayTest {
        // This is the BitPay object we're going to use through all the tests
        private BitPay _bitpay;
        // The pairing code generated in your BitPay account -
        // https://test.bitpay.com/dashboard/merchant/api-tokens
        // This is the POS Pairing Code
        private static readonly string PairingCode = "GHXnenG";

        // Your favourite client name
        private static readonly string ClientName = "BitPay .Net Client v2.0.1904 Tester on " + Environment.MachineName;
        
        // Define the date range for fetching results during the test
        private static DateTime today = DateTime.Now;
        private static DateTime tomorrow = today.AddDays(1);
        private static DateTime yesterday = today.AddDays(-1);

        // Will store one of the generated invoices during the test
        // so it can be paid manually in order to pass the ledger tests


        [TestInitialize]
        public void Init() {

            // JSON minified with the BitPay configuration as in the required configuration file
            // and parsed into a IConfiguration object
            var json = "{\"BitPayConfiguration\":{\"Environment\":\"Test\",\"EnvConfig\":{\"Test\":{\"ClientDescription\":\"" + ClientName + "\",\"ApiUrl\":\"https://test.bitpay.com/\",\"ApiVersion\":\"2.0.0\",\"PrivateKeyPath\":\"sec/bitpay_test_private.key\",\"ApiTokens\":{\"pos\":\"FrbBsxHFkoTbzJPDe6vzBghJzMvDe1nbGUJ3M6n5MHQd\",\"merchant\":\"EZYmyjSaUXh6NcF7Ej9g7dizhhsW2eRvWT29W6CG1omT\",\"payroll\":\"DjyLfN2JDeFoHgUV9Xpx3kvLpA5G2emiyFxUv1q9CREt\"}},\"Prod\":{\"ClientDescription\":\"\",\"ApiUrl\":\"https://bitpay.com/\",\"ApiVersion\":\"2.0.0\",\"PrivateKeyPath\":\"\",\"ApiTokens\":{\"pos\":\"\",\"merchant\":\"\",\"payroll\":\"\"}}}}}";
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

        [TestMethod]
        public async Task TestShouldGetInvoiceId() {
            // create an invoice and make sure we receive an id - which means it has been successfully submitted
            var invoice = new Invoice(50.0, "USD");
            var basicInvoice = await _bitpay.CreateInvoice(invoice);
            Assert.IsNotNull(basicInvoice.Id, "Invoice created with id=NULL");
        }

        [TestMethod]
        public async Task TestShouldGetInvoiceUrl() {
            // create an invoice and make sure we receive an invoice url - which means we can check it online
            var basicInvoice = await _bitpay.CreateInvoice(new Invoice(10.0, "USD"));
            Assert.IsNotNull(basicInvoice.Url, "Invoice created with url=NULL");
        }

        [TestMethod]
        public async Task TestShouldGetInvoiceStatus() {
            // create an invoice and make sure we receive a correct invoice status (new)
            var basicInvoice = await _bitpay.CreateInvoice(new Invoice(10.0, "USD"));
            Assert.AreEqual(Invoice.StatusNew, basicInvoice.Status, "Status is incorrect");
        }

        [TestMethod]
        public async Task TestShouldGetInvoiceBtcPrice() {
            // create an invoice and make sure we receive values for the Bitcoin Cash and Bitcoin fields, respectively
            var basicInvoice = await _bitpay.CreateInvoice(new Invoice(10.0, "USD"));
            Assert.IsNotNull(basicInvoice.PaymentSubtotals.Btc, "Invoice created with PaymentSubtotals.Btc=NULL");
            Assert.IsNotNull(basicInvoice.PaymentSubtotals.Bch, "Invoice created with PaymentSubtotals.Bch=NULL");
        }

        [TestMethod]
        public async Task TestShouldCreateInvoiceOneTenthBtc() {
            // create an invoice and make sure we receive the correct price value back (under 1 BTC)
            var invoice = await _bitpay.CreateInvoice(new Invoice(0.1, "BTC"));
            Assert.AreEqual(0.1, invoice.Price, "Invoice not created correctly: 0.1BTC");
        }

        [TestMethod]
        public async Task TestShouldCreateInvoice100Usd() {
            // create an invoice and make sure we receive the correct price value back (USD)
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, "USD"));
            Assert.AreEqual(100.0, invoice.Price, "Invoice not created correctly: 100USD");
        }

        [TestMethod]
        public async Task TestShouldCreateInvoice100Eur() {
            // create an invoice and make sure we receive the correct price value back (EUR)
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, "EUR"));
            Assert.AreEqual(100.0, invoice.Price, "Invoice not created correctly: 100EUR");
        }

        [TestMethod]
        public async Task TestShouldGetInvoice() {
            // create an invoice then retrieve it through the get method - they should match
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, "EUR"));
            var retrievedInvoice = await _bitpay.GetInvoice(invoice.Id);
            Assert.AreEqual(invoice.Id, retrievedInvoice.Id, "Expected invoice not retrieved");
        }
        
        [TestMethod]
        public async Task TestShouldGetInvoiceNoSigned() {
            // create an invoice without signature then retrieve it through the get method - they should match
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, "EUR"), signRequest: false);
            var retrievedInvoice = await _bitpay.GetInvoice(invoice.Id, Facade.PointOfSale, false);
            Assert.Equal(invoice.Id, retrievedInvoice.Id);
        }

        [TestMethod]
        public async Task TestShouldCreateInvoiceWithAdditionalParams() {
            // create an invoice and make sure we receive the correct fields values back
            var invoice = new Invoice(100.0, "USD") {
                BuyerName = "Satoshi",
                PosData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"
            };
            invoice = await _bitpay.CreateInvoice(invoice);
            Assert.AreEqual(Invoice.StatusNew, invoice.Status, "Status is incorrect");
            Assert.AreEqual("Satoshi", invoice.BuyerName, "BuyerName is incorrect");
            Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", invoice.PosData, "PosData is incorrect");
        }

        [TestMethod]
        public async Task TestShouldGetExchangeRates() {
            // get the exchange rates
            var rates = await _bitpay.GetRates();
            Assert.IsNotNull(rates.GetRates(), "Exchange rates not retrieved");
        }

        [TestMethod]
        public async Task TestShouldGetUsdExchangeRate() {
            // get the exchange rates and check the USD value
            var rates = await _bitpay.GetRates();
            Assert.IsTrue(rates.GetRate("USD") != 0, "Exchange rate not retrieved: USD");
        }

        [TestMethod]
        public async Task TestShouldGetEurExchangeRate() {
            // get the exchange rates and check the EUR value
            var rates = await _bitpay.GetRates();
            Assert.IsTrue(rates.GetRate("EUR") != 0, "Exchange rate not retrieved: EUR");
        }

        [TestMethod]
        public async Task TestShouldGetCnyExchangeRate() {
            // get the exchange rates and check the CNY value
            var rates = await _bitpay.GetRates();
            Assert.IsTrue(rates.GetRate("CNY") != 0, "Exchange rate not retrieved: CNY");
        }

        [TestMethod]
        public async Task TestShouldUpdateExchangeRates() {
            // check the exchange rates after update
            var rates = await _bitpay.GetRates();
            await rates.Update();
            Assert.IsNotNull(rates.GetRates(), "Exchange rates not retrieved after update");
        }

        [TestMethod]
        public async Task TestShouldGetInvoiceIdOne() {
            // create an invoice and get it by its id
            var invoice = await _bitpay.CreateInvoice(new Invoice(1.0, "USD"), Facade.Merchant);
            invoice = await _bitpay.GetInvoice(invoice.Id);
            Assert.IsNotNull(invoice.Id, "Invoice created with id=NULL");
        }

        [TestMethod]
        public async Task TestShouldGetInvoices() {
            // get invoices between two dates
            var invoices = await _bitpay.GetInvoices(yesterday, tomorrow);
            Assert.IsTrue(invoices.Count > 0, "No invoices retrieved");
        }

        [TestMethod]
        public async Task TestShouldGetBtcLedger() {

            // make sure we get a ledger with a not null Entries property
            var ledger = await _bitpay.GetLedger(Ledger.LedgerBtc, yesterday, tomorrow);
            Assert.IsNotNull(ledger);
            Assert.IsNotNull(ledger.Entries);

            // if you know you have entries in your ledger (check your BitPay account), then you can also check them here
            // Assert.IsTrue(ledger.Entries.Any(), "No entries returned for the ledger");

        }

        [TestMethod]
        public async Task TestShouldGetUsdLedger() {

            // Please see the comments from the GetBtcLedger concerning the Merchant facade

            // make sure we get a ledger with a not null Entries property
            var ledger = await _bitpay.GetLedger(Ledger.LedgerUsd, yesterday, tomorrow);
            Assert.IsNotNull(ledger);
            Assert.IsNotNull(ledger.Entries);

            // if you know you have entries in your ledger, then you can also 
            // Assert.IsTrue(ledger.Entries.Any(), "No entries returned for the ledger");

        }

        [TestMethod]
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

            Assert.IsNotNull(batch.Id, "Batch created with id=NULL");
            Assert.IsTrue(batch.Instructions.Count == 2);
        }

        [TestMethod]
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

            Assert.IsNotNull(batch0.Id, "Batch (0) created with id=NULL");
            Assert.IsTrue(batch0.Instructions.Count == 2);

            var batch1 = await _bitpay.GetPayoutBatch(batch0.Id);

            Assert.IsNotNull(batch1.Id, "Batch (1) created with id=NULL");
            Assert.IsTrue(batch1.Instructions.Count == 2);

            await _bitpay.CancelPayoutBatch(batch0.Id);

        }

    }
}
