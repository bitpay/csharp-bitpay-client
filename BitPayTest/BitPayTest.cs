using BitPayAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BitPayTest {

    [TestClass]
    public class BitPayTest {

        private BitPay _bitpay;
        private Invoice _basicInvoice;

        private static readonly string PairingCode = "uacrqFv";
        private static readonly string ClientName = "BitPay C# Library Tester on " + Environment.MachineName;
        private static readonly string BitpayTestUrl = "https://test.bitpay.com/";

        [TestInitialize]
        public void Init() {
            try {
                // This scenario qualifies that this (test) client does not have merchant facade access.
                _bitpay = new BitPay(ClientName, BitpayTestUrl);

                if (!_bitpay.clientIsAuthorized(BitPay.FACADE_POS)) {
                    // Get POS facade authorization.
                    // Obtain a pairingCode from your BitPay account administrator.  When the pairingCode
                    // is created by your administrator it is assigned a facade.  To generate invoices a
                    // POS facade is required.
                    _bitpay.authorizeClient(PairingCode);
                }
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestShouldGetInvoiceId() {
            try {
                _basicInvoice = _bitpay.createInvoice(new Invoice(10.0, "USD"));
                Assert.IsNotNull(_basicInvoice.Id, "Invoice created with id=NULL");
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestShouldGetInvoiceUrl() {
            try {
                _basicInvoice = _bitpay.createInvoice(new Invoice(10.0, "USD"));
                Assert.IsNotNull(_basicInvoice.Url, "Invoice created with url=NULL");
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestShouldGetInvoiceStatus() {
            try {
                _basicInvoice = _bitpay.createInvoice(new Invoice(10.0, "USD"));
                Assert.AreEqual(Invoice.STATUS_NEW, _basicInvoice.Status, "Status is incorrect");
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestShouldGetInvoiceBtcPrice() {
            try {
                _basicInvoice = _bitpay.createInvoice(new Invoice(10.0, "USD"));
                Assert.IsNotNull(_basicInvoice.BtcPrice, "Invoice created with btcPrice=NULL");
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestShouldCreateInvoiceOneTenthBtc() {
            try {
                Invoice invoice = this._bitpay.createInvoice(new Invoice(0.1, "BTC"));
                Assert.AreEqual(0.1, invoice.BtcPrice, 0.0000001, "Invoice not created correctly: 0.1BTC");
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestShouldCreateInvoice100Usd() {
            try {
                Invoice invoice = this._bitpay.createInvoice(new Invoice(100.0, "USD"));
                Assert.AreEqual(100.0, invoice.Price, "Invoice not created correctly: 100USD");
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestShouldCreateInvoice100Eur() {
            try {
                Invoice invoice = this._bitpay.createInvoice(new Invoice(100.0, "EUR"));
                Assert.AreEqual(100.0, invoice.Price, "Invoice not created correctly: 100EUR");
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestShouldGetInvoice() {
            try {
                Invoice invoice = this._bitpay.createInvoice(new Invoice(100.0, "EUR"));
                Invoice retreivedInvoice = this._bitpay.getInvoice(invoice.Id);
                Assert.AreEqual(invoice.Id, retreivedInvoice.Id, "Expected invoice not retreived");
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestShouldCreateInvoiceWithAdditionalParams() {
            try {
                Invoice invoice = new Invoice(100.0, "USD");
                invoice.BuyerName = "Satoshi";
                invoice.BuyerEmail = "satoshi@bitpay.com";
                invoice.FullNotifications = true;
                invoice.NotificationEmail = "satoshi@bitpay.com";
                invoice.PosData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                invoice = this._bitpay.createInvoice(invoice);
                Assert.AreEqual(Invoice.STATUS_NEW, invoice.Status, "Status is incorrect");
                Assert.AreEqual("Satoshi", invoice.BuyerName, "BuyerName is incorrect");
                Assert.AreEqual("satoshi@bitpay.com", invoice.BuyerEmail, "BuyerEmail is incorrect");
                Assert.AreEqual(true, invoice.FullNotifications, "FullNotifications is incorrect");
                Assert.AreEqual("satoshi@bitpay.com", invoice.NotificationEmail, "NotificationEmail is incorrect");
                Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", invoice.PosData, "PosData is incorrect");
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestShouldGetExchangeRates() {
            try {
                Rates rates = this._bitpay.getRates();
                Assert.IsNotNull(rates.getRates(), "Exchange rates not retrieved");
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestShouldGetUsdExchangeRate() {
            Rates rates = this._bitpay.getRates();
            Assert.IsTrue(rates.getRate("USD") != 0, "Exchange rate not retrieved: USD");
        }

        [TestMethod]
        public void TestShouldGetEurExchangeRate() {
            Rates rates = this._bitpay.getRates();
            Assert.IsTrue(rates.getRate("EUR") != 0, "Exchange rate not retrieved: EUR");
        }

        [TestMethod]
        public void TestShouldGetCnyExchangeRate() {
            Rates rates = this._bitpay.getRates();
            Assert.IsTrue(rates.getRate("CNY") != 0, "Exchange rate not retrieved: CNY");
        }

        [TestMethod]
        public void TestShouldUpdateExchangeRates() {
            Rates rates = this._bitpay.getRates();
            rates.update();
            Assert.IsNotNull(rates.getRates(), "Exchange rates not retrieved after update");
        }
    }
}
