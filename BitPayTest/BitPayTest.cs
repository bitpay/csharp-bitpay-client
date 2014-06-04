using BitPayAPI;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitPayTest
{
    [TestClass]
    public class BitPayTest
    {
        private BitPay bitpay;
        private Invoice basicInvoice;
        private static double BTC_EPSILON = .000000001;
        private static double EPSILON = .001;

        private static String SIN = "Teys7dby6EXdxDGnypFozhtMbvYNydxbaXf";
        private static String privKeyFile = "C:\\Users\\Andy\\Documents\\Visual Studio 2012\\Projects\\bitpay-csharp-client-csr\\BitPayTest\\key.priv";
        private static String accountEmail = "andy@bitpay.com";

        public BitPayTest()
        {
            String privateKey = KeyUtils.readKeyFromFile(privKeyFile);
            ECKey privKey = KeyUtils.loadKeys(privateKey);
            bitpay = new BitPay(privKey, SIN);
        }

        [TestMethod]
        public void testShouldGetTokens()
        {
            try
            {
		        List<Token> tokens = this.bitpay.getTokens();
		        Assert.IsTrue(tokens.Count > 0, "List of tokens is empty");
            }
            catch (BitPayException ex)
            {
                Assert.Fail(ex.getMessage());
            }
        }

        [TestMethod]
	    public void testShouldSubmitKey()
        {
            try
            {
		        Key key = this.bitpay.submitKey(accountEmail, SIN, "test");
		        Assert.IsNotNull(key, "Submit key failed to return a response");
            }
            catch (BitPayException ex)
            {
                Assert.Fail(ex.getMessage());
            }
	    }

        [TestMethod]
        public void testShouldGetInvoiceId()
        {
            try
            {
                basicInvoice = bitpay.createInvoice(50, "USD"); 
                Assert.IsNotNull(basicInvoice.id, "Invoice created with id=NULL");
            }
            catch (BitPayException ex)
            {
                Assert.Fail(ex.getMessage());
            }
        }

        [TestMethod]
        public void testShouldGetInvoiceURL()
        {
            try
            {
                basicInvoice = bitpay.createInvoice(50, "USD");
                Assert.IsNotNull(basicInvoice.url, "Invoice created with url=NULL");
            }
            catch (BitPayException ex)
            {
                Assert.Fail(ex.getMessage());
            }
        }

        [TestMethod]
        public void testShouldGetInvoiceStatusL()
        {
            try
            {
                basicInvoice = bitpay.createInvoice(50, "USD");
                Assert.IsNotNull(basicInvoice.status, "Invoice created with status=NULL");
            }
            catch (BitPayException ex)
            {
                Assert.Fail(ex.getMessage());
            }
        }

        [TestMethod]
        public void testShouldGetInvoiceBTCPrice()
        {
            try
            {
                basicInvoice = bitpay.createInvoice(50, "USD");
                Assert.IsNotNull(basicInvoice.btcPrice, "Invoice created with btcPrice=NULL");
            }
            catch (BitPayException ex)
            {
                Assert.Fail(ex.getMessage());
            }
        }

        [TestMethod]
        public void testShouldCreateInvoiceOneTenthBTC()
        {
            try
            {
                Invoice invoice = this.bitpay.createInvoice(0.1, "BTC");
                Assert.AreEqual(0.1, invoice.btcPrice, BTC_EPSILON, "Invoice not created correctly: 0.1BTC");
            }
            catch (BitPayException ex)
            {
                Assert.Fail(ex.getMessage());
            }
        }

        [TestMethod]
        public void testShouldCreateInvoice100USD()
        {
            try
            {
                Invoice invoice = this.bitpay.createInvoice(100.0, "USD");
                Assert.AreEqual(100.0, invoice.price, EPSILON, "Invoice not created correctly: 100USD");
            }
            catch (BitPayException ex)
            {
                Assert.Fail(ex.getMessage());
            }
        }

        [TestMethod]
        public void testShouldCreateInvoice100EUR()
        {
            try
            {
                Invoice invoice = this.bitpay.createInvoice(100.0, "EUR");
                Assert.AreEqual(100.0, invoice.price, EPSILON, "Invoice not created correctly: 100EUR");
            }
            catch (BitPayException ex)
            {
                Assert.Fail(ex.getMessage());
            }
        }

        [TestMethod]
        public void testShouldGetInvoice()
        {
            try
            {
                Invoice invoice = this.bitpay.createInvoice(100.0, "EUR");
                Invoice retreivedInvoice = this.bitpay.getInvoice(invoice.id);
                Assert.AreEqual(invoice.id, retreivedInvoice.id, "Expected invoice not retreived");
            }
            catch (BitPayException ex)
            {
                Assert.Fail(ex.getMessage());
            }
        }

        [TestMethod]
        public void testShouldCreateInvoiceWithAdditionalParams()
        {
            try
            {
                InvoiceParams parameters = new InvoiceParams();
                parameters.buyerName = "Satoshi";
                parameters.buyerEmail = "satoshi@bitpay.com";
                parameters.fullNotifications = true;
                parameters.notificationEmail = "satoshi@bitpay.com";
                Invoice invoice = this.bitpay.createInvoice(100.0, "USD", parameters);
                Assert.IsNotNull(invoice, "Invoice not created");
            }
            catch (BitPayException ex)
            {
                Assert.Fail(ex.getMessage());
            }
        }
	
        [TestMethod]
        public void testShouldGetExchangeRates() 
        {
            try
            {
                Rates rates = this.bitpay.getRates();
                Assert.IsNotNull(rates.getRates(), "Exchange rates not retrieved");
            }
            catch (BitPayException ex)
            {
                Assert.Fail(ex.getMessage());
            }
        }

        [TestMethod]
        public void testShouldGetUSDExchangeRate()
        {
            Rates rates = this.bitpay.getRates();
            Assert.IsTrue(rates.getRate("USD") != 0, "Exchange rate not retrieved: USD");
        }
	
        [TestMethod]
        public void testShouldGetEURExchangeRate()
        {
            Rates rates = this.bitpay.getRates();
            Assert.IsTrue(rates.getRate("EUR") != 0, "Exchange rate not retrieved: EUR");
        }
	
        [TestMethod]
        public void testShouldGetCNYExchangeRate() 
        {
            Rates rates = this.bitpay.getRates();
            Assert.IsTrue(rates.getRate("CNY") != 0, "Exchange rate not retrieved: CNY");
        }
	
        [TestMethod]
        public void testShouldUpdateExchangeRates() 
        {
            Rates rates = this.bitpay.getRates();		
            rates.update();
            Assert.IsNotNull(rates.getRates(), "Exchange rates not retrieved after update");
        }
    }
}
