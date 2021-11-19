using BitPaySDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPaySDK.Exceptions;
using BitPaySDK.Models;
using BitPaySDK.Models.Bill;
using BitPaySDK.Models.Invoice;
using BitPaySDK.Models.Payout;
using Microsoft.Extensions.Configuration;
using Buyer = BitPaySDK.Models.Invoice.Buyer;
using InvoiceStatus = BitPaySDK.Models.Invoice.Status;
using BillStatus = BitPaySDK.Models.Bill.Status;
using PayoutStatus = BitPaySDK.Models.Payout.Status;

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
        private static readonly string ClientName = "BitPay .Net Client v2.2.1907 Tester on " + Environment.MachineName;
        
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
            var json = "{\"BitPayConfiguration\":{\"Environment\":\"Test\",\"EnvConfig\":{\"Test\":{\"ClientDescription\":\"" + ClientName + "\",\"ApiUrl\":\"https://test.bitpay.com/\",\"ApiVersion\":\"2.0.0\",\"PrivateKeyPath\":\"sec/bitpay_test_private.key\",\"ApiTokens\":{\"pos\":\"FrbBsxHFkoTbzJPDe6vzBghJzMvDe1nbGUJ3M6n5MHQd\",\"merchant\":\"EZYmyjSaUXh6NcF7Ej9g7dizhhsW2eRvWT29W6CG1omT\",\"payroll\":\"DjyLfN2JDeFoHgUV9Xpx3kvLpA5G2emiyFxUv1q9CREt\",\"payout\":\"G4pfTiUU7967YJs7Z7n8e2SuQPa2abDTgFrjFB5ZFZsT\"}},\"Prod\":{\"ClientDescription\":\"\",\"ApiUrl\":\"https://bitpay.com/\",\"ApiVersion\":\"2.0.0\",\"PrivateKeyPath\":\"\",\"ApiTokens\":{\"pos\":\"\",\"merchant\":\"\",\"payroll\":\"\"}}}}}";
            var memoryJsonFile = new MemoryFileInfo("config.json", Encoding.UTF8.GetBytes(json), DateTimeOffset.Now);
            var memoryFileProvider = new MockFileProvider(memoryJsonFile);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(memoryFileProvider, "config.json", false, false)
                .Build();

            // Initialize the BitPay object to be used in the following tests
            _bitpay = new BitPay(configuration);

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

            // payouts require the Payout Facade
            if (!_bitpay.tokenExist(Facade.Payout))
            {
                // get a pairing code for the payout facade for this client
                var pcode = _bitpay.RequestClientAuthorization(Facade.Payout).Result;
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
            var invoice = new Invoice(50.0, Currency.USD);
            var basicInvoice = await _bitpay.CreateInvoice(invoice);
            Assert.IsNotNull(basicInvoice.Id, "Invoice created with id=NULL");
        }

        [TestMethod]
        public async Task TestShouldGetInvoiceUrl() {
            // create an invoice and make sure we receive an invoice url - which means we can check it online
            var basicInvoice = await _bitpay.CreateInvoice(new Invoice(10.0, Currency.USD));
            Assert.IsNotNull(basicInvoice.Url, "Invoice created with url=NULL");
        }

        [TestMethod]
        public async Task TestShouldGetInvoiceStatus() {
            // create an invoice and make sure we receive a correct invoice status (new)
            var basicInvoice = await _bitpay.CreateInvoice(new Invoice(10.0, Currency.USD));
            Assert.AreEqual(InvoiceStatus.New, basicInvoice.Status, "Status is incorrect");
        }

        [TestMethod]
        public async Task TestShouldCreateInvoiceOneTenthBtc() {
            // create an invoice and make sure we receive the correct price value back (under 1 BTC)
            var invoice = await _bitpay.CreateInvoice(new Invoice(0.1, Currency.BTC));
            Assert.AreEqual(0.1, invoice.Price, "Invoice not created correctly: 0.1BTC");
        }

        [TestMethod]
        public async Task TestShouldCreateInvoice100Usd() {
            // create an invoice and make sure we receive the correct price value back (USD)
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, Currency.USD));
            Assert.AreEqual(100.0, invoice.Price, "Invoice not created correctly: 100USD");
        }

        [TestMethod]
        public async Task TestShouldCreateInvoice100Eur() {
            // create an invoice and make sure we receive the correct price value back (EUR)
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, Currency.EUR));
            Assert.AreEqual(100.0, invoice.Price, "Invoice not created correctly: 100EUR");
        }

        [TestMethod]
        public async Task TestShouldGetInvoice() {
            // create an invoice then retrieve it through the get method - they should match
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, Currency.EUR));
            var retrievedInvoice = await _bitpay.GetInvoice(invoice.Id);
            Assert.AreEqual(invoice.Id, retrievedInvoice.Id, "Expected invoice not retrieved");
        }

        [TestMethod]
        public async Task TestShouldCreateInvoiceWithAdditionalParams() {
            // create an invoice and make sure we receive the correct fields values back
            var buyerData = new Buyer();
            buyerData.Name = "Satoshi";
            buyerData.Address1 = "street";
            buyerData.Address2 = "911";
            buyerData.Locality = "Washington";
            buyerData.Region = "District of Columbia";
            buyerData.PostalCode = "20000";
            buyerData.Country = "USA";
//            buyerData.Email = "";
//            buyerData.Phone = "";
            buyerData.Notify = true;
            
            var invoice = new Invoice(100.0, Currency.USD)
            {
                Buyer = buyerData,
                PosData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890",
                PaymentCurrencies = new List<string> {
                    Currency.BTC,
                    Currency.BCH
                },
                AcceptanceWindow = 480000,
                FullNotifications = true,
//                NotificationEmail = "",
                NotificationUrl = "https://hookb.in/03EBRQJrzasGmGkNPNw9",
                OrderId = "1234",
                Physical = true,
//                RedirectUrl = "",
                TransactionSpeed = "high",
                ItemCode = "bitcoindonation",
                ItemDesc = "dhdhdfgh"
            };
            invoice = await _bitpay.CreateInvoice(invoice, Facade.Merchant);
            Assert.AreEqual(InvoiceStatus.New, invoice.Status, "Status is incorrect");
            Assert.AreEqual("Satoshi", invoice.Buyer.Name, "BuyerName is incorrect");
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
            Assert.IsTrue(rates.GetRate(Currency.USD) != 0, "Exchange rate not retrieved: USD");
        }

        [TestMethod]
        public async Task TestShouldGetEurExchangeRate() {
            // get the exchange rates and check the EUR value
            var rates = await _bitpay.GetRates();
            Assert.IsTrue(rates.GetRate(Currency.EUR) != 0, "Exchange rate not retrieved: EUR");
        }

        [TestMethod]
        public async Task TestShouldGetCnyExchangeRate() {
            // get the exchange rates and check the CNY value
            var rates = await _bitpay.GetRates();
            Assert.IsTrue(rates.GetRate(Currency.CNY) != 0, "Exchange rate not retrieved: CNY");
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
            var invoice = await _bitpay.CreateInvoice(new Invoice(1.0, Currency.USD), Facade.Merchant);
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
            var ledger = await _bitpay.GetLedger(Currency.BTC, yesterday, tomorrow);
            Assert.IsNotNull(ledger);
            Assert.IsNotNull(ledger.Entries);

            // if you know you have entries in your ledger (check your BitPay account), then you can also check them here
            // Assert.IsTrue(ledger.Entries.Any(), "No entries returned for the ledger");

        }

        [TestMethod]
        public async Task TestShouldGetUsdLedger() {

            // Please see the comments from the GetBtcLedger concerning the Merchant facade

            // make sure we get a ledger with a not null Entries property
            var ledger = await _bitpay.GetLedger(Currency.USD, yesterday, tomorrow);
            Assert.IsNotNull(ledger);
            Assert.IsNotNull(ledger.Entries);

            // if you know you have entries in your ledger, then you can also 
            // Assert.IsTrue(ledger.Entries.Any(), "No entries returned for the ledger");

        }

        [TestMethod]
        public async Task testShouldSubmitPayoutRecipients()
        {
            List<PayoutRecipient> recipientsList = new List<PayoutRecipient>();
            recipientsList.Add(new PayoutRecipient(
                "sandbox+recipient1@bitpay.com",
                "recipient1",
                "https://hookb.in/wNDlQMV7WMFz88VDyGnJ"));
            recipientsList.Add(new PayoutRecipient(
                "sandbox+recipient2@bitpay.com",
                "recipient2",
                "https://hookb.in/QJOPBdMgRkukpp2WO60o"));
            recipientsList.Add(new PayoutRecipient(
                "sandbox+recipient3@bitpay.com",
                "recipient3",
                "https://hookb.in/QJOPBdMgRkukpp2WO60o"));

            var recipientsObj = new PayoutRecipients(recipientsList);
            List<PayoutRecipient> recipients = await _bitpay.SubmitPayoutRecipients(recipientsObj);

            Assert.IsNotNull(recipients);
            Assert.AreEqual(recipients[0].Email, "sandbox+recipient1@bitpay.com");
        }

        [TestMethod]
        public async Task testShouldGetPayoutRecipientId()
        {
            List<PayoutRecipient> recipientsList = new List<PayoutRecipient>();
            recipientsList.Add(new PayoutRecipient(
                "sandbox+recipient1@bitpay.com",
                "recipient1",
                "https://hookb.in/wNDlQMV7WMFz88VDyGnJ"));
            recipientsList.Add(new PayoutRecipient(
                "sandbox+recipient2@bitpay.com",
                "recipient2",
                "https://hookb.in/QJOPBdMgRkukpp2WO60o"));
            recipientsList.Add(new PayoutRecipient(
                "sandbox+recipient3@bitpay.com",
                "recipient3",
                "https://hookb.in/QJOPBdMgRkukpp2WO60o"));

            PayoutRecipients recipientsObj = new PayoutRecipients(recipientsList);
            var recipients = await _bitpay.SubmitPayoutRecipients(recipientsObj);
            var firstRecipient = recipients.First();
            var retrieved = await _bitpay.GetPayoutRecipient(firstRecipient.Id);

            Assert.IsNotNull(firstRecipient);
            Assert.IsNotNull(retrieved.Id);
            Assert.AreEqual(firstRecipient.Id, retrieved.Id);
            Assert.AreEqual(firstRecipient.Email, "sandbox+recipient1@bitpay.com");
        }

        [TestMethod]
        public async Task testShouldSubmitGetAndDeletePayoutRecipient()
        {
            List<PayoutRecipient> recipientsList = new List<PayoutRecipient>();
            recipientsList.Add(new PayoutRecipient(
                "sandbox+recipient17@bitpay.com",
               "recipient1",
               "https://hookb.in/wNDlQMV7WMFz88VDyGnJ"));
            recipientsList.Add(new PayoutRecipient(
                "sandbox+recipient28@bitpay.com",
                "recipient2",
               "https://hookb.in/QJOPBdMgRkukpp2WO60o"));
            recipientsList.Add(new PayoutRecipient(
                "sandbox+recipient30@bitpay.com",
                "recipient3",
                "https://hookb.in/QJOPBdMgRkukpp2WO60o"));
            var recipientsObj = new PayoutRecipients(recipientsList);

            var basicRecipients = await _bitpay.SubmitPayoutRecipients(recipientsObj);
            var basicRecipient = basicRecipients[0];
            var retrieveRecipient = await _bitpay.GetPayoutRecipient(basicRecipient.Id);
            var retrieveRecipients = await _bitpay.GetPayoutRecipients();
            retrieveRecipient.Label = "Updated Label";
            var updateRecipient = await _bitpay.UpdatePayoutRecipient(retrieveRecipient.Id, retrieveRecipient);
            var deleteRecipient = await _bitpay.DeletePayoutRecipient(retrieveRecipient.Id);

            Assert.IsNotNull(basicRecipient);
            Assert.IsNotNull(retrieveRecipient.Id);
            Assert.IsNotNull(retrieveRecipients);
            Assert.AreEqual(basicRecipient.Id, retrieveRecipient.Id);
            Assert.AreEqual(retrieveRecipient.Status, RecipientStatus.INVITED);
            Assert.AreEqual(updateRecipient.Label,"Updated Label");
            Assert.IsTrue(deleteRecipient);
        }

        [TestMethod]
        public async Task testShouldNotifyPayoutRecipientId()
        {
            List<PayoutRecipient> recipientsList = new List<PayoutRecipient>();
            recipientsList.Add(new PayoutRecipient(
                "sandbox+recipient1@bitpay.com",
                "recipient1",
                "https://hookb.in/wNDlQMV7WMFz88VDyGnJ"));
            PayoutRecipients recipientsObj = new PayoutRecipients(recipientsList);
            var recipients = await _bitpay.SubmitPayoutRecipients(recipientsObj);
            var basicRecipient = recipients[0];
            var result = await _bitpay.SendPayoutRecipientNotification(basicRecipient.Id);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task testShouldSubmitPayout()
        {
            var ledgerCurrency = Currency.ETH;
            var currency = Currency.USD;
            var payout = new Payout(5.0,currency, ledgerCurrency);
            var recipients = await _bitpay.GetPayoutRecipients("active", 1);
            payout.RecipientId = recipients.First().Id;
            payout.NotificationUrl = "https://hookbin.com/yDEDeWJKyasG9yjj9X9P";
            var createpayout = await _bitpay.SubmitPayout(payout);
            var cancelledpayout = await _bitpay.CancelPayout(createpayout.Id);

            Assert.IsNotNull(createpayout.Id);
            Assert.IsTrue(cancelledpayout);
        }

        [TestMethod]
        public async Task testShouldGetPayouts()
        {
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-50);
            var batches = await _bitpay.GetPayouts(startDate, endDate);

            Assert.IsTrue(batches.Count > 0);
        }

        [TestMethod]
        public async Task testShouldGetPayoutsByStatus()
        {
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-50);
            var batches = await _bitpay.GetPayouts(startDate, endDate, PayoutStatus.New, "");

            Assert.IsTrue(batches.Count > 0, "No batches retrieved");
        }

        [TestMethod]
        public async Task testShouldSubmitGetAndDeletePayout()
        {
            var ledgerCurrency = Currency.ETH;
            var currency = Currency.USD;
            var batch = new Payout(5.0, currency,ledgerCurrency);
            var recipients = await _bitpay.GetPayoutRecipients("active", 1);
            batch.RecipientId = recipients.First().Id;
            batch.NotificationUrl = "https://hookb.in/QJOPBdMgRkukpp2WO60o";
            var batch0 = await _bitpay.SubmitPayout(batch);
            var batchRetrieved = await _bitpay.GetPayout(batch0.Id);
            var batchCancelled = await _bitpay.CancelPayout(batchRetrieved.Id);

            Assert.IsNotNull(batch0.Id);
            Assert.IsNotNull(batchRetrieved.Id);
            Assert.AreEqual(batch0.Id, batchRetrieved.Id);
            Assert.AreEqual(batchRetrieved.Status, PayoutStatus.New);
            Assert.IsTrue(batchCancelled); 
        }

        [TestMethod]
        public async Task testShouldNotifyPayoutId()
        {
            var ledgerCurrency = Currency.ETH;
            var currency = Currency.USD;
            var batch = new Payout(5.0, currency, ledgerCurrency);
            var recipients = await _bitpay.GetPayoutRecipients("active", 1);
            batch.RecipientId = recipients.First().Id;
            batch.NotificationUrl = "https://hookb.in/QJOPBdMgRkukpp2WO60o";
            var basicRecipient = await _bitpay.SubmitPayout(batch);
            var result = await _bitpay.SendPayoutNotification(basicRecipient.Id);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task testShouldGetPayoutRecipients()
        {
            var recipients = await _bitpay.GetPayoutRecipients("active", 1);
            Assert.AreEqual(1, recipients.Count);
        }

        [TestMethod]
        public async Task TestShouldSubmitPayoutBatch() {

            var date = DateTime.Now;
            var threeDaysFromNow = date.AddDays(3);

            var effectiveDate = threeDaysFromNow;
            var currency = Currency.USD;
            var ledgerCurrency = Currency.ETH;
            var instructions = new List<PayoutInstruction>() {
                new PayoutInstruction(10.0, RecipientReferenceMethod.EMAIL, "sandbox+recipient1@bitpay.com"),
                new PayoutInstruction(10.0, RecipientReferenceMethod.EMAIL, "sandbox+recipient2@bitpay.com")
            };

            var batch = new PayoutBatch(currency, effectiveDate, instructions, ledgerCurrency);
            batch.NotificationUrl = "https://hookbin.com/yDEDeWJKyasG9yjj9X9P";
            batch = await _bitpay.SubmitPayoutBatch(batch);

            Assert.IsNotNull(batch.Id, "Batch created with id=NULL");
            Assert.IsTrue(batch.Instructions.Count == 2);
        }

        [TestMethod]
        public async Task TestShouldSubmitGetAndDeletePayoutBatch() {

            var date = DateTime.Now;
            var threeDaysFromNow = date.AddDays(3);

            var effectiveDate = threeDaysFromNow;
            var currency = Currency.USD;
            var ledgerCurrency = Currency.ETH;
            var instructions = new List<PayoutInstruction>() {
                new PayoutInstruction(10.0, RecipientReferenceMethod.EMAIL, "sandbox+recipient1@bitpay.com"),
                new PayoutInstruction(10.0, RecipientReferenceMethod.EMAIL, "sandbox+recipient2@bitpay.com")
            };

            var batch0 = new PayoutBatch(currency, effectiveDate, instructions, ledgerCurrency);
            batch0.NotificationUrl = "https://hookbin.com/yDEDeWJKyasG9yjj9X9P";
            batch0 = await _bitpay.SubmitPayoutBatch(batch0);

            Assert.IsNotNull(batch0.Id, "Batch (0) created with id=NULL");
            Assert.IsTrue(batch0.Instructions.Count == 2);

            var batch1 = await _bitpay.GetPayoutBatch(batch0.Id);

            Assert.IsNotNull(batch1.Id, "Batch (1) created with id=NULL");
            Assert.IsTrue(batch1.Instructions.Count == 2);

            await _bitpay.CancelPayoutBatch(batch0.Id);

        }

        [TestMethod]
        public async Task testShouldNotifyPayoutBatchId()
        {

            var date = DateTime.Now;
            var ledgerCurrency = Currency.ETH;
            var threeDaysFromNow = date.AddDays(3);
            var currency = Currency.USD;
            var effectiveDate = threeDaysFromNow;
            var instructions = new List<PayoutInstruction>() {
                new PayoutInstruction(10.0, RecipientReferenceMethod.EMAIL, "sandbox+recipient1@bitpay.com"),
                new PayoutInstruction(10.0, RecipientReferenceMethod.EMAIL, "sandbox+recipient2@bitpay.com")
            };
            var batch = new PayoutBatch(currency, effectiveDate, instructions, ledgerCurrency);
            batch.NotificationUrl = "https://hookbin.com/yDEDeWJKyasG9yjj9X9P";
            batch = await _bitpay.SubmitPayoutBatch(batch);
            var result = await _bitpay.SendPayoutBatchNotification(batch.Id);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task TestShouldGetPayoutBatches() {

            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-50);
            var batches = await _bitpay.GetPayoutBatches(startDate, endDate);

            Assert.IsTrue(batches.Count > 0, "No batches retrieved");
        }

        [TestMethod]
        public async Task TestShouldGetPayoutBatchesByStatus() {

            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-50);
            var batches = await _bitpay.GetPayoutBatches(startDate, endDate, PayoutStatus.New);

            Assert.IsTrue(batches.Count > 0, "No batches retrieved");
        }

        [TestMethod]
        public async Task TestGetSettlements() {
            
            // make sure we get a ledger with a not null Entries property
            var settlements = await _bitpay.GetSettlements(Currency.EUR, yesterday.AddMonths(-1).AddDays(3), tomorrow);
            Assert.IsNotNull(settlements);

        }

        [TestMethod]
        public async Task TestGetSettlement() {
            
            // make sure we get a ledger with a not null Entries property
            var settlements = await _bitpay.GetSettlements(Currency.EUR, yesterday.AddMonths(-1).AddDays(3), tomorrow);
            var firstSettlement = settlements[0];
            var settlement = await _bitpay.GetSettlement(firstSettlement.Id);
            Assert.IsNotNull(settlement.Id);
            Assert.AreEqual(firstSettlement.Id, settlement.Id);
        }
        
        [TestMethod]
        public async Task TestShouldCreateBillUSD() 
        {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});
            
            // create a bill and make sure we receive an id - which means it has been successfully submitted
            var bill = new Bill()
            {
                Number = "1", 
                Currency = Currency.USD, 
                Email = "", //email address mandatory
                Items = items
            };
            var basicBill = await _bitpay.CreateBill(bill);
            Assert.IsNotNull(basicBill.Id);
        }

        [TestMethod]
        public async Task TestShouldCreateBillEur() {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});

            // create a bill and make sure we receive an id - which means it has been successfully submitted
            var bill = new Bill()
            {
                Number = "2", 
                Currency = Currency.EUR, 
                Email = "", //email address mandatory
                Items = items
            };
            var basicBill = await _bitpay.CreateBill(bill);
            Assert.IsNotNull(basicBill.Id);
        }

        [TestMethod]
        public async Task TestShouldGetBillUrl() {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});

            // create a bill and make sure we receive a bill url - which means we can check it online
            var bill = new Bill()
            {
                Number = "3", 
                Currency = Currency.USD, 
                Email = "", //email address mandatory
                Items = items
            };
            var basicBill = await _bitpay.CreateBill(bill);
            Assert.IsNotNull(basicBill.Url);
        }

        [TestMethod]
        public async Task TestShouldGetBillStatus() {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});

            // create a bill and make sure we receive a correct bill status (draft)
            var bill = new Bill()
            {
                Number = "4", 
                Currency = Currency.USD, 
                Email = "", //email address mandatory
                Items = items
            };
            var basicBill = await _bitpay.CreateBill(bill);
            Assert.AreEqual(BillStatus.Draft, basicBill.Status);
        }

        [TestMethod]
        public async Task TestShouldGetBillTotals() {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});

            // create a bill and make sure we receive the same items sum as it was sent
            var bill = new Bill()
            {
                Number = "5", 
                Currency = Currency.USD, 
                Email = "", //email address mandatory
                Items = items
            };
            var basicBill = await _bitpay.CreateBill(bill);
            Assert.AreEqual(basicBill.Items.Select(i => i.Price).Sum(), items.Select(i => i.Price).Sum());
        }

        [TestMethod]
        public async Task TestShouldGetBill() {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});

            // create a bill then retrieve it through the get method - they should match
            var bill = new Bill()
            {
                Number = "6", 
                Currency = Currency.USD, 
                Email = "", //email address mandatory
                Items = items
            };
            var basicBill = await _bitpay.CreateBill(bill);
            var retrievedBill = await _bitpay.GetBill(basicBill.Id);
            Assert.AreEqual(basicBill.Id, retrievedBill.Id);
        }

        [TestMethod]
        public async Task TestShouldGetAndUpdateBill() {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});

            var bill = new Bill()
            {
                Number = "6", 
                Currency = Currency.USD, 
                Email = "", //email address mandatory
                Items = items,
                Name = "basicBill"
            };
            var basicBill = await _bitpay.CreateBill(bill);
            var retrievedBill = await _bitpay.GetBill(basicBill.Id);
            retrievedBill.Currency = Currency.EUR;
            retrievedBill.Name = "updatedBill";
            retrievedBill.Items.Add(new Item(){Price = 60.0, Quantity = 7, Description = "product-added"});
                
            var updatedBill = await _bitpay.UpdateBill(retrievedBill, retrievedBill.Id);
            Assert.AreEqual(basicBill.Id, retrievedBill.Id);
            Assert.AreEqual(retrievedBill.Id, updatedBill.Id);
            Assert.AreEqual(updatedBill.Currency, Currency.EUR);
            Assert.AreEqual(updatedBill.Name, "updatedBill");
        }

        [TestMethod]
        public async Task TestShouldDeliverBill()
        {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});

            // create a bill then retrieve it through the get method - they should match
            var bill = new Bill()
            {
                Number = "7", 
                Currency = Currency.USD, 
                Email = "", //email address mandatory
                Items = items
            };
            var basicBill = await _bitpay.CreateBill(bill);
            var result = await _bitpay.DeliverBill(basicBill.Id, basicBill.Token);
            // Retrieve the updated bill for status confirmation
            var retrievedBill = await _bitpay.GetBill(basicBill.Id);
            // Check the correct response
            Assert.AreEqual("Success", result);
            // Confirm that the bill is sent
            Assert.AreEqual(BillStatus.Sent, retrievedBill.Status);
        }

        [TestMethod]
        public async Task TestShouldGetBills() {

            var bills = await _bitpay.GetBills();
            Assert.IsTrue(bills.Count > 0, "No bills retrieved");
        }

        [TestMethod]
        public async Task TestShouldGetBillsByStatus() {

            var bills = await _bitpay.GetBills(BillStatus.Sent);
            Assert.IsTrue(bills.Count > 0, "No bills retrieved");
        }
    }
}
