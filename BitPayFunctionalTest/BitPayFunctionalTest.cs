// Copyright (c) 2019 BitPay.
// All rights reserved.

using BitPay;
using BitPay.Models;
using BitPay.Models.Bill;
using BitPay.Models.Invoice;
using BitPay.Models.Payout;

using Xunit.Abstractions;

using Environment = BitPay.Environment;
using SystemEnvironment = System.Environment;

namespace BitPayFunctionalTest
{
    public class BitPayFunctionalTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private static readonly DateTime Today = DateTime.Now;
        private static readonly DateTime Tomorrow = Today.AddDays(1);
        private static readonly DateTime Yesterday = Today.AddDays(-1);
        
        private readonly Client _client;
        
        /// <summary>
        ///     Before use these tests you have to generate BitPay.config.json by BitPaySetup and put this file
        ///     into this directory.
        ///     You should create recipient in test.bitpay.com/dashboard/payouts/recipients and put this email
        ///     to "email.txt" file in this directory. It's required for submit requests.
        ///     It's impossible to test settlements in test environment.
        /// </summary>
        public BitPayFunctionalTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            string path = GetBitPayUnitTestPath() + Path.DirectorySeparatorChar + "BitPay.config.json";
            _client = new Client(new ConfigFilePath(path), Environment.Test);
        }
        
        /// <summary>
        ///     Tested rates requests:
        ///     - GetRate(string baseCurrency, string currency)
        ///     - GetRates()
        ///     - GetRates(string currency)
        /// </summary>
        [Fact]
        public async Task it_should_test_rate_requests()
        {
            var rate = await _client.GetRate(Currency.BCH, Currency.USD);
            Assert.True(rate.Value != 0);
            
            var rates = await _client.GetRates();
            Assert.True(rates.GetRate(Currency.USD) != 0);
            
            var rateUsd = await _client.GetRates(Currency.BTC);
            Assert.True(rateUsd.GetRate(Currency.BCH) != 0);
        }

        /// <summary>
        ///     Tested currency requests:
        ///     - GetCurrencyInfo(string currencyCode)
        /// </summary>
        [Fact]
        public async Task it_should_test_currency_requests()
        {
            var getCurrencyInfo = await _client.GetCurrencyInfo("BTC");
            Assert.Equal("Bitcoin", getCurrencyInfo.Name);
        }

        /// <summary>
        ///     Tested invoice requests:
        ///     - CreateInvoice
        ///     - GetInvoice
        ///     - GetInvoiceByGuid
        ///     - GetInvoices
        ///     - UpdateInvoice
        ///     - CancelInvoice
        ///     - GetInvoiceEventToken
        ///
        ///     Not tested requests:
        ///     - RequestInvoiceWebhookToBeResent
        /// </summary>
        [Fact]
        public async Task it_should_test_invoice_requests()
        {
            var invoice = await _client.CreateInvoice(GetInvoiceExample());
            var invoiceToken = invoice.Token;
            var invoiceId = invoice.Id!;
            
            var invoiceGet = _client.GetInvoice(invoiceId).Result;
            Assert.Equal(invoiceToken, invoiceGet.Token);
            
            var invoiceGetByGuid = _client.GetInvoiceByGuid(invoice.ResourceGuid!).Result;
            Assert.Equal(invoiceToken, invoiceGetByGuid.Token);
            
            var invoices = _client.GetInvoices(Yesterday, Tomorrow).Result;
            Assert.NotEmpty(invoices);
            invoices.Exists(invoiceList => invoiceList.Token == invoiceToken);
            
            var getInvoiceEventToken = _client.GetInvoiceEventToken(invoiceId).Result;
            Assert.NotNull(getInvoiceEventToken.Token);
            
            var updatedEmail = "updated@email.com";
            var updateInvoiceParameters = new Dictionary<string, dynamic?> {{"buyerEmail", updatedEmail}};
            var updatedInvoice = _client.UpdateInvoice(invoiceId, updateInvoiceParameters).Result;
            Assert.Equal(updatedEmail, updatedInvoice.BuyerProvidedEmail);
            var invoiceGetAfterUpdate = _client.GetInvoice(invoiceId).Result;
            Assert.Equal(updatedEmail, invoiceGetAfterUpdate.BuyerProvidedEmail);
            
            var cancelInvoice = _client.CancelInvoice(invoiceId).Result;
            Assert.True(cancelInvoice.IsCancelled);
            
            var invoiceToCancelByGuid = await _client.CreateInvoice(GetInvoiceExample());
            var cancelInvoiceByGuid = _client.CancelInvoiceByGuid(invoiceToCancelByGuid.ResourceGuid!).Result;
            Assert.True(cancelInvoiceByGuid.IsCancelled);
        }

        /// <summary>
        ///     Tested refund requests:
        /// 
        ///     - CreateRefund(Refund refundToCreate)
        ///     - GetRefund(string refundId)
        ///     - GetRefundByGuid(string guid)
        ///     - GetRefunds(string invoiceId)
        ///     - SendRefundNotification(string refundId)
        ///     - CancelRefund(string refundId)
        ///     - CancelRefundByGuid(string guid)
        /// 
        ///     Not tested refund requests:
        ///     - UpdateRefund(string refundId, string status) / preview status limitation
        ///     - UpdateRefundByGuid(string guid, string status) / preview status limitation
        /// </summary>
        [Fact]
        public async Task it_should_test_refunds_requests()
        {
            var invoice = await _client.CreateInvoice(GetInvoiceExample());
            var invoiceId = invoice.Id!;
            await _client.PayInvoice(invoiceId);
            
            var refundToCreateRequest = new Refund(invoiceId: invoiceId, amount: 10.0M);
            var refund = await _client.CreateRefund(refundToCreateRequest);
            var refundId = refund.Id!;

            var retrieveRefund = await _client.GetRefund(refundId);
            Assert.Equal(refundId, retrieveRefund.Id);
            Assert.NotNull(retrieveRefund.Invoice);

            var retrieveRefundByGuid = await _client.GetRefundByGuid(refund.ResourceGuid!);
            Assert.Equal(refundId, retrieveRefundByGuid.Id);

            var retrieveRefundByInvoiceId = await _client.GetRefunds(invoiceId);
            Assert.NotEmpty(retrieveRefundByInvoiceId);
            retrieveRefundByInvoiceId.Exists(refundByInvoice => refundByInvoice.Invoice == invoiceId);

            var refundNotification = await _client.SendRefundNotification(refundId);
            Assert.True(refundNotification);

            var cancelRefund = await _client.CancelRefund(refundId);
            Assert.Equal("canceled", cancelRefund.Status);
            var retrieveRefundAfterCanceled = await _client.GetRefund(refundId);
            Assert.Equal("canceled", retrieveRefundAfterCanceled.Status);

            var refundToCreateForCancelByGuid = new Refund(invoiceId: invoiceId, amount: 10.0M);
            var refundToCancelByGuid = await _client.CreateRefund(refundToCreateForCancelByGuid);
            var refundCanceledByGuid = await _client.CancelRefundByGuid(refundToCancelByGuid.ResourceGuid!);
            Assert.Equal("canceled", refundCanceledByGuid.Status);
        }

        /// <summary>
        ///     Tested recipient requests:
        ///     - SubmitPayoutRecipients(PayoutRecipients recipients)
        ///     - GetPayoutRecipient(string recipientId)
        ///     - GetPayoutRecipients(string status, int limit, int offset)
        ///     - UpdatePayoutRecipient(string recipientId, PayoutRecipient recipient)
        ///     - DeletePayoutRecipient(string recipientId)
        ///     - RequestPayoutRecipientNotification(string recipientId)
        /// </summary>
        [Fact]
        public async Task it_should_test_recipients_requests()
        {
            var email = "bob@email.com";
            var requestedRecipient = new PayoutRecipient(email, "Bob");
            var requestedRecipients = new List<PayoutRecipient> {requestedRecipient};

            var recipients = await _client.SubmitPayoutRecipients(new PayoutRecipients(requestedRecipients));
            var recipientId = recipients[0].Id!;

            var retrieveRecipient = await _client.GetPayoutRecipient(recipientId);
            Assert.Equal(email, retrieveRecipient.Email);

            var retrieveRecipientsByStatus = await _client.GetPayoutRecipients("invited");
            Assert.NotEmpty(retrieveRecipientsByStatus);

            var updatedLabel = "updatedLabel";
            var updateRecipientRequest  = new PayoutRecipient(label: updatedLabel, email: email);
            var updateRecipient = await _client.UpdatePayoutRecipient(recipientId, updateRecipientRequest);
            Assert.Equal(updatedLabel, updateRecipient.Label);

            var removeRecipient = await _client.DeletePayoutRecipient(recipientId);
            Assert.True(removeRecipient);
        }

        /// <summary>
        ///     You need to have recipient before
        /// 
        ///     Tested payout requests:
        ///     - SubmitPayout(Payout payout)
        ///     - GetPayout(string payoutId)
        ///     - CancelPayout(string payoutId)
        ///     - GetPayouts(Dictionary<string, dynamic> filters)
        ///     - RequestPayoutNotification(string payoutId)
        /// </summary>
        [Fact]
        public async Task it_should_test_payout_requests()
        {
            var email = GetEmail();
            var requestedRecipient = new PayoutRecipient(email, "Bob");
            var requestedRecipients = new List<PayoutRecipient> {requestedRecipient};
            
            var recipients = await _client.SubmitPayoutRecipients(new PayoutRecipients(requestedRecipients));
            var recipientId = recipients[0].Id;

            Payout payout = new Payout
            {
                Amount = 10.00M,
                Currency = "USD",
                LedgerCurrency = "USD",
                RecipientId = recipientId,
                NotificationEmail = email,
                Reference = "C# Integration Test " + Guid.NewGuid(),
                NotificationUrl = "https://somenotiticationURL.com"
            };

            var submitPayout = _client.SubmitPayout(payout).Result;
            var payoutId = submitPayout.Id;
            Assert.NotNull(payoutId);
            Assert.Equal(email, submitPayout.NotificationEmail);
            
            var getPayoutById = _client.GetPayout(payoutId!).Result;
            Assert.Equal(email, getPayoutById.NotificationEmail);
            
            var getPayoutsFilters = new Dictionary<string, dynamic?>
            {
                { "startDate", Yesterday.ToString("yyyy-MM-dd") },
                { "endDate", Tomorrow.ToString("yyyy-MM-dd") }
            };
            
            var getPayouts = _client.GetPayouts(getPayoutsFilters).Result;
            Assert.NotEmpty(getPayouts);
            getPayouts.Exists(singlePayout => singlePayout.NotificationEmail == email);
             
            var requestPayoutNotification = _client.RequestPayoutNotification(payoutId!).Result;
            Assert.True(requestPayoutNotification);
            
            var cancelledPayout = _client.CancelPayout(payoutId!).Result;
            Assert.True(cancelledPayout);
        }
        
        /// <summary>
        ///     You need to have recipient before
        /// 
        ///     Tested payout requests:
        ///     - SubmitPayouts(ICollection<Payout> payouts)
        ///     - GetPayout(string payoutId)
        ///     - CancelPayouts(string groupId)
        ///     - GetPayouts(Dictionary<string, dynamic> filters)
        ///     - RequestPayoutNotification(string payoutId)
        /// </summary>
        [Fact]
        public async Task it_should_test_payout_group_requests()
        {
            var email = GetEmail();
            var requestedRecipient = new PayoutRecipient(email, "Bob");
            var requestedRecipients = new List<PayoutRecipient> {requestedRecipient};
            
            var recipients = await _client.SubmitPayoutRecipients(new PayoutRecipients(requestedRecipients));
            var recipientId = recipients[0].Id;

            Payout payout = new()
            {
                Amount = 10.00M,
                Currency = "USD",
                LedgerCurrency = "USD",
                RecipientId = recipientId,
                NotificationEmail = email,
                Reference = "C# Integration Test " + Guid.NewGuid(),
                NotificationUrl = "https://somenotiticationURL.com",
            };
            
            Payout payout2 = new()
            {
                Amount = 10.00M,
                Currency = "USD",
                LedgerCurrency = "USD",
                RecipientId = recipientId,
                NotificationEmail = email,
                Reference = "C# Integration Test " + Guid.NewGuid(),
                NotificationUrl = "https://somenotiticationURL.com"
            };

            var payoutGroup = _client.SubmitPayouts(new List<Payout> {payout, payout2}).Result;
            Assert.Empty(payoutGroup.Failed);
            Assert.NotEmpty(payoutGroup.Payouts);

            var firstPayoutId = payoutGroup.Payouts[0].Id;
            Assert.NotNull(firstPayoutId);
            Assert.NotNull(payoutGroup.Payouts[1].Id);
            Assert.Equal(email, payoutGroup.Payouts[0].NotificationEmail);
            
            var getPayoutById = _client.GetPayout(firstPayoutId!).Result;
            Assert.Equal(email, getPayoutById.NotificationEmail);
            var getPayoutsFilters = new Dictionary<string, dynamic?>
            {
                { "startDate", Yesterday.ToString("yyyy-MM-dd") },
                { "endDate", Tomorrow.ToString("yyyy-MM-dd") },
                { "groupId", getPayoutById.GroupId }
            };
            
            var getPayouts = _client.GetPayouts(getPayoutsFilters).Result;
            Assert.NotEmpty(getPayouts);
            getPayouts.Exists(singlePayout => singlePayout.NotificationEmail == email);
              
            var requestPayoutNotification = _client.RequestPayoutNotification(firstPayoutId!).Result;
            Assert.True(requestPayoutNotification);
            
            var cancelledPayout = _client.CancelPayouts(getPayoutById.GroupId!).Result;
            Assert.Empty(cancelledPayout.Failed);
            Assert.NotEmpty(cancelledPayout.Payouts);
        }
        
        /// <summary>
        ///     Create payments before start it.
        /// 
        ///     Tested ledgers requests:
        ///     - GetLedgers()
        ///     - GetLedgerEntries(string currency)
        /// </summary>
        [Fact]
        public async Task it_should_test_ledgers_requests()
        {
            var ledgers = await _client.GetLedgers();
            Assert.NotEmpty(ledgers);

            var ledgersEntries = await _client.GetLedgerEntries("USD", Today.AddDays(-31), Today);
            Assert.NotEmpty(ledgersEntries);
        }
        
        /// <summary>
        ///     Tested bills requests:
        ///     - CreateBill(Bill bill)
        ///     - GetBill(string billId)
        ///     - GetBills(string status)
        ///     - UpdateBill(Bill bill, string billId)
        ///     - DeliverBill(string billId, string billToken)
        /// </summary>
        [Fact]
        public async Task it_should_test_bills_requests()
        {
            Item item1 = new Item(price: 10.00M, quantity: 1) { Id = "Test Item 1" };
            List<Item> items = new List<Item> {item1};
            var requestedBill = new Bill(
                number: "bill1234-ABCD",
                currency: "USD",
                email: "john@doe.com",
                items: items
            )
            {
                Name = "John Doe",
                Address1 = "2630 Hegal Place",
                Address2 = "Apt 42",
                City = "Alexandria",
                State = "VA",
                Zip = "23242",
                Country = "US",
                Phone = "555-123-456",
                DueDate = "2021-5-31",
                PassProcessingFee = true,
            };
            var createBill = await _client.CreateBill(requestedBill);
            var billId = createBill.Id!;

            var getBill = await _client.GetBill(billId);
            Assert.Equal(billId, getBill.Id);

            var getBills = await _client.GetBills();
            Assert.NotEmpty(getBills);

            Item itemUpdated = new Item(price: 9.00M, quantity: 1) { Id = "Test Item Updated" };
            List<Item> itemsUpdated = new List<Item> {itemUpdated};
            var updatedBillRequest = new Bill(
                number: requestedBill.Number,
                currency: "USD",
                email:"john@doe.com",
                items: itemsUpdated
            ) 
            {
                Token = createBill.Token,
            };
            var updatedBill = await _client.UpdateBill(updatedBillRequest, billId);
            Assert.Equal(9.00M, updatedBill.Items[0].Price);

            var deliverBill = await _client.DeliverBill(billId, createBill.Token!);
            Assert.Equal("Success", deliverBill);
        }

        /// <summary>
        ///     Tested wallet requests:
        ///
        ///     - GetSupportedWallets()
        /// </summary>
        [Fact]
        public async Task it_should_test_wallet_requests()
        {
            var supportedWallets = await _client.GetSupportedWallets();
            Assert.NotEmpty(supportedWallets);
        }

        private Invoice GetInvoiceExample()
        {
            var invoice = new Invoice(10.0M, "USD")
            {
                FullNotifications = true,
                ExtendedNotifications = true,
                TransactionSpeed = "medium",
                NotificationUrl = "https://notification.url/aaa",
                NotificationEmail = GetEmail(),
                ItemDesc = "Created by integration tests",
                AutoRedirect = true,
                ForcedBuyerSelectedWallet = "bitpay"
            };
            Buyer buyerData = new Buyer
            {
                Name = "Marcin",
                Address1 = "SomeStreet",
                Address2 = "911",
                Locality = "Washington",
                Region = "District of Columbia",
                PostalCode = "20000",
                Country = "USA",
                Email = "buyer@buyeremaildomain.com",
                Notify = true
            };
            invoice.Buyer = buyerData;

            return invoice;
        }
        
        private string GetEmail()
        {
            var email = "";

            try
            {
                email = File.ReadAllText(GetBitPayUnitTestPath() + Path.DirectorySeparatorChar + "email.txt");
            }
            catch (Exception)
            {
                _testOutputHelper.WriteLine("Please create email.txt file with email added in test.bitpay.com. " +
                                            "You can check constructor of this class for more details");
                throw;
            }

            return email;
        }
        
        private static string GetBitPayUnitTestPath()
        {
            var bitPayUnitTestPath = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
            if (bitPayUnitTestPath == null)
            {
                throw new Exception("Invalid BitPay unit test path");
            }
            
            return bitPayUnitTestPath;
        }
    }
}