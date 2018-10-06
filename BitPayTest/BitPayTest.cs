using BitPayAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using BitPayAPI.Models;

namespace BitPayTest {

    [TestClass]
    public class BitPayTest {

        private BitPay _bitpay;
        private Invoice _basicInvoice;

        private static readonly string PairingCode = "hdQMXJM";
        private static readonly string ClientName = "BitPay C# Library Tester on " + Environment.MachineName;
        private static readonly string BitpayTestUrl = "https://test.bitpay.com/";

        [TestInitialize]
        public void Init() {
            // This scenario qualifies that this (test) client does not have merchant facade access.
            _bitpay = new BitPay(ClientName, BitpayTestUrl);

            if (!_bitpay.ClientIsAuthorized(BitPay.FacadePos)) {
                // Get POS facade authorization.
                // Obtain a pairingCode from your BitPay account administrator.  When the pairingCode
                // is created by your administrator it is assigned a facade.  To generate invoices a
                // POS facade is required.
                _bitpay.AuthorizeClient(PairingCode);
            }
        }

        [TestMethod]
        public void TestShouldGetInvoiceId() {
            var invoice = new Invoice(50.0, "USD");
            _basicInvoice = _bitpay.CreateInvoice(invoice);
            Assert.IsNotNull(_basicInvoice.Id, "Invoice created with id=NULL");
        }

        [TestMethod]
        public void TestShouldGetInvoiceUrl() {
            _basicInvoice = _bitpay.CreateInvoice(new Invoice(10.0, "USD"));
            Assert.IsNotNull(_basicInvoice.Url, "Invoice created with url=NULL");
        }

        [TestMethod]
        public void TestShouldGetInvoiceStatus() {
            _basicInvoice = _bitpay.CreateInvoice(new Invoice(10.0, "USD"));
            Assert.AreEqual(Invoice.StatusNew, _basicInvoice.Status, "Status is incorrect");
        }

        [TestMethod]
        public void TestShouldGetInvoiceBtcPrice() {
            _basicInvoice = _bitpay.CreateInvoice(new Invoice(10.0, "USD"));
            Assert.IsNotNull(_basicInvoice.BtcPrice, "Invoice created with btcPrice=NULL");
        }

        [TestMethod]
        public void TestShouldCreateInvoiceOneTenthBtc() {
            var invoice = _bitpay.CreateInvoice(new Invoice(0.1, "BTC"));
            Assert.AreEqual(0.1, invoice.Price, 0.0000001, "Invoice not created correctly: 0.1BTC");
        }

        [TestMethod]
        public void TestShouldCreateInvoice100Usd() {
            var invoice = _bitpay.CreateInvoice(new Invoice(100.0, "USD"));
            Assert.AreEqual(100.0, invoice.Price, "Invoice not created correctly: 100USD");
        }

        [TestMethod]
        public void TestShouldCreateInvoice100Eur() {
            var invoice = _bitpay.CreateInvoice(new Invoice(100.0, "EUR"));
            Assert.AreEqual(100.0, invoice.Price, "Invoice not created correctly: 100EUR");
        }

        [TestMethod]
        public void TestShouldGetInvoice() {
            var invoice = _bitpay.CreateInvoice(new Invoice(100.0, "EUR"));
            var retrievedInvoice = _bitpay.GetInvoice(invoice.Id);
            Assert.AreEqual(invoice.Id, retrievedInvoice.Id, "Expected invoice not retrieved");
        }

        [TestMethod]
        public void TestShouldCreateInvoiceWithAdditionalParams() {
            var invoice = new Invoice(100.0, "USD")
            {
                BuyerName = "Satoshi",
                BuyerEmail = "satoshi@bitpay.com",
                FullNotifications = true,
                NotificationEmail = "satoshi@bitpay.com",
                PosData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"
            };
            invoice = _bitpay.CreateInvoice(invoice);
            Assert.AreEqual(Invoice.StatusNew, invoice.Status, "Status is incorrect");
            Assert.AreEqual("Satoshi", invoice.BuyerName, "BuyerName is incorrect");
            Assert.AreEqual("satoshi@bitpay.com", invoice.BuyerEmail, "BuyerEmail is incorrect");
            Assert.AreEqual(true, invoice.FullNotifications, "FullNotifications is incorrect");
            Assert.AreEqual("satoshi@bitpay.com", invoice.NotificationEmail, "NotificationEmail is incorrect");
            Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", invoice.PosData, "PosData is incorrect");
        }

        [TestMethod]
        public void TestShouldGetExchangeRates() {
            var rates = _bitpay.GetRates();
            Assert.IsNotNull(rates.GetRates(), "Exchange rates not retrieved");
        }

        [TestMethod]
        public void TestShouldGetUsdExchangeRate() {
            var rates = _bitpay.GetRates();
            Assert.IsTrue(rates.GetRate("USD") != 0, "Exchange rate not retrieved: USD");
        }

        [TestMethod]
        public void TestShouldGetEurExchangeRate() {
            var rates = _bitpay.GetRates();
            Assert.IsTrue(rates.GetRate("EUR") != 0, "Exchange rate not retrieved: EUR");
        }

        [TestMethod]
        public void TestShouldGetCnyExchangeRate() {
            var rates = _bitpay.GetRates();
            Assert.IsTrue(rates.GetRate("CNY") != 0, "Exchange rate not retrieved: CNY");
        }

        [TestMethod]
        public void TestShouldUpdateExchangeRates() {
            var rates = _bitpay.GetRates();
            rates.Update();
            Assert.IsNotNull(rates.GetRates(), "Exchange rates not retrieved after update");
        }

        [TestMethod]
        public void TestShouldGetInvoiceIdOne() {

            var invoice = _bitpay.CreateInvoice(new Invoice(1.0, "USD"), BitPay.FacadeMerchant);
            invoice = _bitpay.GetInvoice(invoice.Id, BitPay.FacadeMerchant);
            Assert.IsNotNull(invoice.Id, "Invoice created with id=NULL");
        }

        [TestMethod]
        public void TestShouldGetInvoices() {
            var invoices = _bitpay.GetInvoices(new DateTime(2014, 8, 1), new DateTime(2014, 8, 31));
            Assert.IsTrue(invoices.Count > 0, "No invoices retrieved");
        }

        [TestMethod]
        public void TestShouldGetBtcLedger() {
            var ledger = _bitpay.GetLedger(Ledger.LedgerBtc, new DateTime(2014, 8, 1), new DateTime(2014, 8, 31));
            Assert.IsTrue(ledger.Entries.Count > 0, "Ledger is empty");
        }

        [TestMethod]
        public void TestShouldGetUsdLedger() {
            var ledger = _bitpay.GetLedger(Ledger.LedgerUsd, new DateTime(2014, 1, 1), new DateTime(2014, 1, 31));
            Assert.IsTrue(ledger.Entries.Count > 0, "Ledger is empty");
        }

        [TestMethod]
        public void TestShouldSubmitPayoutBatch() {
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
            batch = _bitpay.SubmitPayoutBatch(batch);

            Assert.IsNotNull(batch.Id, "Batch created with id=NULL");
            Assert.IsTrue(batch.Instructions.Count == 2);
        }

        [TestMethod]
        public void TestShouldSubmitGetAndDeletePayoutBatch() {
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
            batch0 = _bitpay.SubmitPayoutBatch(batch0);

            Assert.IsNotNull(batch0.Id, "Batch (0) created with id=NULL");
            Assert.IsTrue(batch0.Instructions.Count == 2);

            var batch1 = _bitpay.GetPayoutBatch(batch0.Id);

            Assert.IsNotNull(batch1.Id, "Batch (1) created with id=NULL");
            Assert.IsTrue(batch1.Instructions.Count == 2);

            _bitpay.CancelPayoutBatch(batch0.Id);

        }
    }
}
