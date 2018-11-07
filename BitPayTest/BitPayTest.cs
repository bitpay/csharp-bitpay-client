using BitPayAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitPayAPI.Exceptions;
using BitPayAPI.Models;
using BitPayAPI.Models.Invoice;

namespace BitPayTest {

    [TestClass]
    public class BitPayTest {

        // This is the BitPay object we're going to use through all the tests
        private BitPay _bitpay;

        // The pairing code generated in your BitPay account -
        // https://test.bitpay.com/dashboard/merchant/api-tokens
        // This is the POS Pairing Code
        private static readonly string PairingCode = "GiAXLaT"; 

        // Your favourite client name
        private static readonly string ClientName = "BitPay C# Library Tester on " + Environment.MachineName;

        // The URL to test against
        private static readonly string BitpayTestUrl = "https://test.bitpay.com/";


        [TestInitialize]
        public void Init() {

            // Initialize the BitPay object to be used in the following tests
            _bitpay = new BitPay(ClientName, BitpayTestUrl);

            // If the client doesn't have a POS token yet, fetch one.
            // For the Merchant and Payroll Facades, see below, in their corresponding tests
            if (!_bitpay.ClientIsAuthorized(BitPay.FacadePos)) {
                _bitpay.AuthorizeClient(PairingCode).Wait();
            }

            // ledgers require the Merchant Facade
            if (!_bitpay.ClientIsAuthorized(BitPay.FacadeMerchant)) {
                // get a pairing code for the merchant facade for this client
                var pcode = _bitpay.RequestClientAuthorization(BitPay.FacadeMerchant).Result;
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
            Assert.AreEqual(0.1, invoice.Price, 0.0000001, "Invoice not created correctly: 0.1BTC");
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
        public async Task TestShouldCreateInvoiceWithAdditionalParams() {
            // create an invoice and make sure we receive the correct fields values back
            var invoice = new Invoice(100.0, "USD") {
                BuyerName = "Satoshi",
                BuyerEmail = "satoshi@bitpay.com",
                FullNotifications = true,
                NotificationEmail = "satoshi@bitpay.com",
                PosData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"
            };
            invoice = await _bitpay.CreateInvoice(invoice);
            Assert.AreEqual(Invoice.StatusNew, invoice.Status, "Status is incorrect");
            Assert.AreEqual("Satoshi", invoice.BuyerName, "BuyerName is incorrect");
            Assert.AreEqual("satoshi@bitpay.com", invoice.BuyerEmail, "BuyerEmail is incorrect");
            Assert.AreEqual(true, invoice.FullNotifications, "FullNotifications is incorrect");
            Assert.AreEqual("satoshi@bitpay.com", invoice.NotificationEmail, "NotificationEmail is incorrect");
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
            var invoice = await _bitpay.CreateInvoice(new Invoice(1.0, "USD"), BitPay.FacadeMerchant);
            invoice = await _bitpay.GetInvoice(invoice.Id);
            Assert.IsNotNull(invoice.Id, "Invoice created with id=NULL");
        }

        [TestMethod]
        public async Task TestShouldGetInvoices() {
            // get invoices between two dates
            var invoices = await _bitpay.GetInvoices(new DateTime(2018, 8, 1), new DateTime(2018, 11, 30));
            Assert.IsTrue(invoices.Count > 0, "No invoices retrieved");
        }

        [TestMethod]
        public async Task TestShouldGetBtcLedger() {
            
            // make sure we get a ledger with a not null Entries property
            var ledger = await _bitpay.GetLedger(Ledger.LedgerBtc, new DateTime(2014, 8, 1), new DateTime(2014, 8, 31));
            Assert.IsNotNull(ledger);
            Assert.IsNotNull(ledger.Entries);

            // if you know you have entries in your ledger (check your BitPay account), then you can also check them here
            // Assert.IsTrue(ledger.Entries.Any(), "No entries returned for the ledger");

        }

        [TestMethod]
        public async Task TestShouldGetUsdLedger() {

            // Please see the comments from the GetBtcLedger concerning the Merchant facade

            // make sure we get a ledger with a not null Entries property
            var ledger = await _bitpay.GetLedger(Ledger.LedgerUsd, new DateTime(2014, 1, 1), new DateTime(2014, 1, 31));
            Assert.IsNotNull(ledger);
            Assert.IsNotNull(ledger.Entries);

            // if you know you have entries in your ledger, then you can also 
            // Assert.IsTrue(ledger.Entries.Any(), "No entries returned for the ledger");

        }

        //[TestMethod]
        //public async Task TestShouldSubmitPayoutBatch() {

        //    // The payout batches is for the Payroll facade
        //    if (!_bitpay.ClientIsAuthorized(BitPay.FacadePayroll)) {
        //        // get the pairing code for the payout facade
        //        var pcode = await _bitpay.RequestClientAuthorization(BitPay.FacadePayroll);
        //        /*
        //         Unfortunately at the time of this writing the Payroll facade is not available through the API
        //         so this test will always fail - since you can't approve the Payroll pairing code
        //         */
        //        throw new BitPayException("Please approve the pairing code " + pcode + " in your account.");
        //    }

        //    var date = DateTime.Now;
        //    var threeDaysFromNow = date.AddDays(3);

        //    var effectiveDate = threeDaysFromNow;
        //    var reference = "My test batch";
        //    var bankTransferId = "My bank transfer id";
        //    var currency = "USD";
        //    var instructions = new List<PayoutInstruction>() {
        //        new PayoutInstruction(100.0, "mtHDtQtkEkRRB5mgeWpLhALsSbga3iZV6u", "Alice"),
        //        new PayoutInstruction(200.0, "mvR4Xj7MYT7GJcL93xAQbSZ2p4eHJV5F7A", "Bob")
        //    };

        //    var batch = new PayoutBatch(currency, effectiveDate, bankTransferId, reference, instructions);
        //    batch = await _bitpay.SubmitPayoutBatch(batch);

        //    Assert.IsNotNull(batch.Id, "Batch created with id=NULL");
        //    Assert.IsTrue(batch.Instructions.Count == 2);
        //}

        //[TestMethod]
        //public async Task TestShouldSubmitGetAndDeletePayoutBatch() {

        //    /*
        //       Unfortunately at the time of this writing the Payroll facade is not available through the API
        //       so this test will always fail - since you can't approve the Payroll pairing code
        //     */

        //    var date = DateTime.Now;
        //    var threeDaysFromNow = date.AddDays(3);

        //    var effectiveDate = threeDaysFromNow;
        //    var reference = "My test batch";
        //    var bankTransferId = "My bank transfer id";
        //    var currency = "USD";
        //    var instructions = new List<PayoutInstruction>() {
        //        new PayoutInstruction(100.0, "mtHDtQtkEkRRB5mgeWpLhALsSbga3iZV6u", "Alice"),
        //        new PayoutInstruction(200.0, "mvR4Xj7MYT7GJcL93xAQbSZ2p4eHJV5F7A", "Bob")
        //    };

        //    var batch0 = new PayoutBatch(currency, effectiveDate, bankTransferId, reference, instructions);
        //    batch0 = await _bitpay.SubmitPayoutBatch(batch0);

        //    Assert.IsNotNull(batch0.Id, "Batch (0) created with id=NULL");
        //    Assert.IsTrue(batch0.Instructions.Count == 2);

        //    var batch1 = await _bitpay.GetPayoutBatch(batch0.Id);

        //    Assert.IsNotNull(batch1.Id, "Batch (1) created with id=NULL");
        //    Assert.IsTrue(batch1.Instructions.Count == 2);

        //    await _bitpay.CancelPayoutBatch(batch0.Id);

        //}

    }
}
