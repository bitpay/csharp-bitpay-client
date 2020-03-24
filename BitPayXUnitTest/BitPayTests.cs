using System;
using System.Linq;
using Xunit;
using BitPaySDK;
using System.Collections.Generic;
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

namespace BitPayXUnitTest
{
    public class Tests
    {
        // This is the BitPay object we're going to use through all the tests
        private BitPay _bitpay;
        // The pairing code generated in your BitPay account -
        // https://test.bitpay.com/dashboard/merchant/api-tokens
        // This is the POS Pairing Code
        private static readonly string PairingCode = "Bh3yy6r";

        // Your favourite client name
        private static readonly string ClientName = "BitPay .Net Client v2.2.1907 Tester on " + Environment.MachineName;
        
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
            var json = "{\"BitPayConfiguration\":{\"Environment\":\"Test\",\"EnvConfig\":{\"Test\":{\"PrivateKeyPath\":\"bitpay_private_test.key\",\"ApiTokens\":{\"merchant\":\"CE2WRSEEt9FgXvXboxNFA4YdQyyDJmgVAo752TGA7eUj\",\"payroll\":\"9pJ7fzW1GGeuDQfj32aNATCDnyY6YAacVMcDrs7HHUNo\"}},\"Prod\":{\"PrivateKeyPath\":\"\",\"ApiTokens\":{\"merchant\":\"\",\"payroll\":\"\"}}}}}";
            var memoryJsonFile = new MemoryFileInfo("config.json", Encoding.UTF8.GetBytes(json), DateTimeOffset.Now);
            var memoryFileProvider = new MockFileProvider(memoryJsonFile);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(memoryFileProvider, "config.json", false, false)
                .Build();
            
            // Initialize the BitPay object to be used in the following tests
            // Initialize with IConfiguration object
            _bitpay = new BitPay(configuration);
            
            // Initialize with separate variables
            _bitpay = new BitPay(
                Env.Test,
                "bitpay_private_test.key",
                new Env.Tokens(){
                    Merchant = "CE2WRSEEt9FgXvXboxNFA4YdQyyDJmgVAo752TGA7eUj",
                    Payout = "9pJ7fzW1GGeuDQfj32aNATCDnyY6YAacVMcDrs7HHUNo"
                }
            );

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
            var invoice = new Invoice(30.0, Currency.USD);
            var basicInvoice = await _bitpay.CreateInvoice(invoice);
            Assert.NotNull(basicInvoice.Id);
        }
        
        [Fact]
        public async Task testShouldCreateInvoiceBTC() 
        {
            // create an invoice and make sure we receive an id - which means it has been successfully submitted
            var invoice = new Invoice(30.0, Currency.USD);
            invoice.PaymentCurrencies = new List<string>();
            invoice.PaymentCurrencies.Add(Currency.BTC);
            var basicInvoice = await _bitpay.CreateInvoice(invoice);
            Assert.NotNull(basicInvoice.Id);
        }
        
        [Fact]
        public async Task testShouldCreateInvoiceBCH() 
        {
            // create an invoice and make sure we receive an id - which means it has been successfully submitted
            var invoice = new Invoice(30.0, Currency.USD);
            invoice.PaymentCurrencies = new List<string>();
            invoice.PaymentCurrencies.Add(Currency.BCH);
            var basicInvoice = await _bitpay.CreateInvoice(invoice);
            Assert.NotNull(basicInvoice.Id);
        }
        
        [Fact]
        public async Task testShouldCreateInvoiceETH() 
        {
            // create an invoice and make sure we receive an id - which means it has been successfully submitted
            var invoice = new Invoice(30.0, Currency.USD);
            invoice.PaymentCurrencies = new List<string>();
            invoice.PaymentCurrencies.Add(Currency.ETH);
            var basicInvoice = await _bitpay.CreateInvoice(invoice);
            Assert.NotNull(basicInvoice.Id);
        }

        [Fact]
        public async Task TestShouldGetInvoiceUrl() {
            // create an invoice and make sure we receive an invoice url - which means we can check it online
            var basicInvoice = await _bitpay.CreateInvoice(new Invoice(10.0, Currency.USD));
            Assert.NotNull(basicInvoice.Url);
        }

        [Fact]
        public async Task TestShouldGetInvoiceStatus() {
            // create an invoice and make sure we receive a correct invoice status (new)
            var basicInvoice = await _bitpay.CreateInvoice(new Invoice(10.0, Currency.USD));
            Assert.Equal(InvoiceStatus.New, basicInvoice.Status);
        }

        [Fact]
        public async Task TestShouldCreateInvoiceOneTenthBtc() {
            // create an invoice and make sure we receive the correct price value back (under 1 BTC)
            var invoice = await _bitpay.CreateInvoice(new Invoice(0.1, Currency.BTC));
            Assert.Equal(0.1, invoice.Price);
        }

        [Fact]
        public async Task TestShouldCreateInvoice100Usd() {
            // create an invoice and make sure we receive the correct price value back (USD)
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, Currency.USD));
            Assert.Equal(100.0, invoice.Price);
        }

        [Fact]
        public async Task TestShouldCreateInvoice100Eur() {
            // create an invoice and make sure we receive the correct price value back (EUR)
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, Currency.EUR));
            Assert.Equal(100.0, invoice.Price);
        }

        [Fact]
        public async Task TestShouldGetInvoice() {
            // create an invoice then retrieve it through the get method - they should match
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, Currency.EUR));
            var retrievedInvoice = await _bitpay.GetInvoice(invoice.Id);
            Assert.Equal(invoice.Id, retrievedInvoice.Id);
        }

        [Fact]
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
            Assert.Equal(InvoiceStatus.New, invoice.Status);
            Assert.Equal("Satoshi", invoice.Buyer.Name);
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
            Assert.True(rates.GetRate(Currency.USD) != 0, "Exchange rate not retrieved: USD");
        }

        [Fact]
        public async Task TestShouldGetEurExchangeRate() {
            // get the exchange rates and check the EUR value
            var rates = await _bitpay.GetRates();
            Assert.True(rates.GetRate(Currency.EUR) != 0, "Exchange rate not retrieved: EUR");
        }

        [Fact]
        public async Task TestShouldGetCnyExchangeRate() {
            // get the exchange rates and check the CNY value
            var rates = await _bitpay.GetRates();
            Assert.True(rates.GetRate(Currency.CNY) != 0, "Exchange rate not retrieved: CNY");
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
            var invoice = await _bitpay.CreateInvoice(new Invoice(1.0, Currency.USD), Facade.Merchant);
            invoice = await _bitpay.GetInvoice(invoice.Id);
            Assert.NotNull(invoice.Id);
        }

        [Fact]
        public async Task TestShouldGetInvoices() {
            // get invoices between two dates
            var invoices = await _bitpay.GetInvoices(yesterday, tomorrow);
            Assert.True(invoices.Count > 0, "No invoices retrieved");
        }

        /*
        To use this test:
	    You must have a paid/completed invoice in your account (list of invoices). The test looks for the first invoice in the "complete"
	    state and authorises a refund. The actual refund will not be executed until the email receiver enters his bitcoin refund address.
        */
        [Fact]
        public async Task testShouldCreateGetCancelRefundRequest() {
            //check within the last few days
            var date = DateTime.Now;
            var today = date;
            var sevenDaysAgo = date.AddDays(-95);
            var invoices = await _bitpay.GetInvoices(sevenDaysAgo, today, InvoiceStatus.Complete);
            Invoice firstInvoice = invoices.First();
                
            Assert.NotNull(firstInvoice);
            string refundEmail = "";

            Boolean createdRefund = await _bitpay.CreateRefund(firstInvoice, refundEmail, firstInvoice.Price, firstInvoice.Currency);
            List<Refund> retrievedRefunds = await _bitpay.GetRefunds(firstInvoice);
            Refund firstRefund = retrievedRefunds.First();
            Refund retrievedRefund = await _bitpay.GetRefund(firstInvoice, firstRefund.Id);
            Boolean cancelled = await _bitpay.CancelRefund(firstInvoice, firstRefund.Id);

            Assert.True(createdRefund);
            Assert.True(retrievedRefunds.Count > 0);
            Assert.NotNull(firstRefund);
            Assert.NotNull(retrievedRefund);
            Assert.True(cancelled);
        }

        [Fact]
        public async Task TestShouldGetLedgerBtc() {
            
            // make sure we get a ledger with a not null Entries property
            var ledger = await _bitpay.GetLedger(Currency.BTC, yesterday.AddMonths(-1).AddDays(3), tomorrow);
            Assert.NotNull(ledger);
            Assert.NotNull(ledger.Entries);

        }

        [Fact]
        public async Task TestShouldGetLedgerUsd() {
            // Please see the comments from the GetBtcLedger concerning the Merchant facade

            // make sure we get a ledger with a not null Entries property
            var ledger = await _bitpay.GetLedger(Currency.USD, yesterday.AddMonths(-1).AddDays(3), tomorrow);
            Assert.NotNull(ledger);
            Assert.NotNull(ledger.Entries);

        }

        [Fact]
        public async Task TestShouldGetLedgers() {
            
            var ledgers = await _bitpay.GetLedgers();
            Assert.NotNull(ledgers);
            Assert.True(ledgers.Count > 0, "No ledgers retrieved");

        }

        [Fact]
        public async Task TestShouldSubmitPayoutBatch() {

            var date = DateTime.Now;
            var threeDaysFromNow = date.AddDays(3);

            var effectiveDate = threeDaysFromNow;
            var currency = Currency.USD;
            var instructions = new List<PayoutInstruction>() {
                new PayoutInstruction(100.0, "mtHDtQtkEkRRB5mgeWpLhALsSbga3iZV6u"),
                new PayoutInstruction(200.0, "mvR4Xj7MYT7GJcL93xAQbSZ2p4eHJV5F7A")
            };

            var batch = new PayoutBatch(currency, effectiveDate, instructions);
            batch = await _bitpay.SubmitPayoutBatch(batch);

            Assert.NotNull(batch.Id);
            Assert.True(batch.Instructions.Count == 2);
        }

        [Fact]
        public async Task TestShouldSubmitGetAndDeletePayoutBatch() {
            
            var date = DateTime.Now;
            var threeDaysFromNow = date.AddDays(3);

            var effectiveDate = threeDaysFromNow;
            var currency = Currency.USD;
            var instructions = new List<PayoutInstruction>() {
                new PayoutInstruction(100.0, "mtHDtQtkEkRRB5mgeWpLhALsSbga3iZV6u"),
                new PayoutInstruction(200.0, "mvR4Xj7MYT7GJcL93xAQbSZ2p4eHJV5F7A")
            };

            var batch0 = new PayoutBatch(currency, effectiveDate, instructions);
            batch0 = await _bitpay.SubmitPayoutBatch(batch0);

            Assert.NotNull(batch0.Id);
            Assert.True(batch0.Instructions.Count == 2);

            var batch1 = await _bitpay.GetPayoutBatch(batch0.Id);

            Assert.NotNull(batch1.Id);
            Assert.True(batch1.Instructions.Count == 2);

            await _bitpay.CancelPayoutBatch(batch0.Id);

        }

        [Fact]
        public async Task TestShouldGetPayoutBatches() {
            
            var batches = await _bitpay.GetPayoutBatches();
            Assert.True(batches.Count > 0, "No batches retrieved");
        }

        [Fact]
        public async Task TestShouldGetPayoutBatchesByStatus() {
            
            var batches = await _bitpay.GetPayoutBatches(PayoutStatus.New);
            Assert.True(batches.Count > 0, "No batches retrieved");
        }

        [Fact]
        public async Task TestGetSettlements() {
            
            // make sure we get a ledger with a not null Entries property
            var settlements = await _bitpay.GetSettlements(Currency.EUR, yesterday.AddMonths(-1).AddDays(3), tomorrow);
            Assert.NotNull(settlements);

        }

        [Fact]
        public async Task TestGetSettlement() {
            
            // make sure we get a ledger with a not null Entries property
            var settlements = await _bitpay.GetSettlements(Currency.EUR, yesterday.AddMonths(-1).AddDays(3), tomorrow);
            var firstSettlement = settlements[0];
            var settlement = await _bitpay.GetSettlement(firstSettlement.Id);
            Assert.NotNull(settlement.Id);
            Assert.Equal(firstSettlement.Id, settlement.Id);
        }
        
        [Fact]
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
            Assert.NotNull(basicBill.Id);
        }

        [Fact]
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
            Assert.NotNull(basicBill.Id);
        }

        [Fact]
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
            Assert.NotNull(basicBill.Url);
        }

        [Fact]
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
            Assert.Equal(BillStatus.Draft, basicBill.Status);
        }

        [Fact]
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
            Assert.Equal(basicBill.Items.Select(i => i.Price).Sum(), items.Select(i => i.Price).Sum());
        }

        [Fact]
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
            Assert.Equal(basicBill.Id, retrievedBill.Id);
        }

        [Fact]
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
            Assert.Equal(basicBill.Id, retrievedBill.Id);
            Assert.Equal(retrievedBill.Id, updatedBill.Id);
            Assert.Equal(updatedBill.Currency, Currency.EUR);
            Assert.Equal(updatedBill.Name, "updatedBill");
        }

        [Fact]
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
            Assert.Equal("Success", result);
            // Confirm that the bill is sent
            Assert.Equal(BillStatus.Sent, retrievedBill.Status);
        }

        [Fact]
        public async Task TestShouldGetBills() {
            
            var bills = await _bitpay.GetBills();
            Assert.True(bills.Count > 0, "No bills retrieved");
        }

        [Fact]
        public async Task TestShouldGetBillsByStatus() {
            
            var bills = await _bitpay.GetBills(BillStatus.Sent);
            Assert.True(bills.Count > 0, "No bills retrieved");
        }
    }
}