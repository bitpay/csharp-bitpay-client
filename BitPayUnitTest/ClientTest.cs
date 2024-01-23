// Copyright (c) 2019 BitPay.
// All rights reserved.

using System.Net;
using System.Net.Http;

using BitPay;
using BitPay.Clients;
using BitPay.Exceptions;
using BitPay.Models.Bill;
using BitPay.Models.Invoice;
using BitPay.Models.Payout;
using BitPay.Utils;

using Moq;

using Environment = BitPay.Environment;
using SystemEnvironment = System.Environment;

namespace BitPayUnitTest
{
    public class ClientTest
    {
        const string Identity = "someIdentity";
        const string ExampleGuid = "37bd36bd-6fcb-409c-a907-47f9244302aa";
        const string MerchantToken = "merchantToken";
        const string PayoutToken = "payoutToken";

        private readonly Mock<IBitPayClient> _bitPayClient;
        private readonly Mock<AccessTokens> _accessTokens;
        private readonly Mock<IGuidGenerator> _guidGenerator;

        public ClientTest()
        {
            _bitPayClient = new Mock<IBitPayClient>();
            _accessTokens = new Mock<AccessTokens>();
            _guidGenerator = new Mock<IGuidGenerator>();
        }

        [Fact]
        public void it_should_provide_pos_client()
        {
            string posToken = "posToken";

            var client = new Client(new PosToken(posToken));
            
            Assert.IsType<Client>(client);
            Assert.Equal(posToken, client.GetAccessToken("pos"));
        }

        [Fact]
        public void it_should_provide_client_by_key()
        {
            // given
            String privateKey =
                "75371435315047800683080420474719166774492308988314944856528163960396135344086";
            String merchantToken = "merchantToken";
            AccessTokens tokens = new AccessTokens();
            tokens.AddMerchant(merchantToken);
        
            // when
            Client client = new Client(new PrivateKey(privateKey), tokens, Environment.Test);
        
            // then
            Assert.Equal(merchantToken, client.GetAccessToken(Facade.Merchant));
        }

        [Fact]
        public void it_should_provide_client_by_config_file()
        {
            // given
            string path = GetBitPayUnitTestPath() + Path.DirectorySeparatorChar + "BitPay.config.json";

            // when
            Client bitpay = new Client(new ConfigFilePath(path), Environment.Test);

            // then
            Assert.Equal("merchantToken", bitpay.GetAccessToken(Facade.Merchant));
            Assert.Equal("payoutToken", bitpay.GetAccessToken(Facade.Payout));
        }

        [Fact]
        public void TestShouldThrowsBitPayExceptionForInvalidPrivateKey()
        {
            void Client() => new Client(new PrivateKey("invalid"), _accessTokens.Object, Environment.Test);
            BitPayException exception = Assert.Throws<BitPayException>((Action) Client);
            Assert.Equal("Private Key file not found OR invalid key provided", exception.Message);
        }
        
        [Fact]
        public void it_should_authorize_client_by_pairing_code() 
        {
            // given
            var pairingCode = "123123123";
            HttpContent response = new StringContent("[{\"policies\":[{\"policy\":\"id\",\"method\":\"active\",\"params\":[\"Tf2yXsY49iFyDfxt3b2kf9VPRMwPxxAyCRW\"]}],\"token\":\"t0k3n\",\"facade\":\"merchant\",\"dateCreated\":1668425446554,\"pairingExpiration\":1668511846554,\"pairingCode\":\"123123123\"}]");
            
            _guidGenerator.Setup(p => p.Execute()).Returns(ExampleGuid);
            _bitPayClient.Setup(b => b.Post(
                "tokens",
                "{\"guid\":\"37bd36bd-6fcb-409c-a907-47f9244302aa\",\"id\":\"someIdentity\",\"pairingCode\":\"123123123\"}",
                false
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Post, "tokens")
            });

            var testedClass = GetTestedClass();

            testedClass.AuthorizeClient(pairingCode);
            
            _bitPayClient.Verify(b => b.Post(
                "tokens",
                "{\"guid\":\"37bd36bd-6fcb-409c-a907-47f9244302aa\",\"id\":\"someIdentity\",\"pairingCode\":\"123123123\"}",
                false
                ), Times.Once
            );
        }
        
        [Fact]
        public void it_should_authorize_client_by_facade()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "authorizeClientByFacadeResponse.json"));
            _bitPayClient.Setup(b => b.Post(
                "tokens",
                "{\"guid\":\"37bd36bd-6fcb-409c-a907-47f9244302aa\",\"id\":\"someIdentity\",\"facade\":\"merchant\"}",
                false
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Post, "tokens")
            });
            _guidGenerator.Setup(p => p.Execute()).Returns(ExampleGuid);

            var testedClass = GetTestedClass();
            
            // when
            var pairingCode = testedClass.CreatePairingCodeForFacade(Facade.Merchant);
            
            // then
            Assert.Equal("C4Lg7oW", pairingCode.Result);
        }
        
        [Fact]
        public void it_should_create_bill()
        {
            // given
            var testedClass = GetTestedClassAsMerchant();
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "createBillResponse.json"));
            _bitPayClient.Setup(b => b.Post(
                "bills",
                File.ReadAllText(GetJsonResponsePath() + "createBillRequest.json"),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Post, "bills")
            });
            
            // when
            var result = testedClass.CreateBill(GetBill()).Result;

            // then
            Assert.Equal("USD", result.Currency);
            Assert.Equal("6EBQR37MgDJPfEiLY3jtRq7eTP2aodR5V5wmXyyZhru5FM5yF4RCGKYQtnT7nhwHjA", result.Token);
            Assert.Equal("john@doe.com", result.Email);
            Assert.Equal("bill1234-EFGH", result.Number);
            Assert.Equal("John Doe", result.Name);
            Assert.Equal("2630 Hegal Place", result.Address1);
            Assert.Equal("Apt 42", result.Address2);
            Assert.Equal("Alexandria", result.City);
            Assert.Equal("VA", result.State);
            Assert.Equal("23242", result.Zip);
            Assert.Equal("jane@doe.com", result.Cc?.First());
            Assert.Equal("555-123-456", result.Phone);
            Assert.Equal("2021-05-31T00:00:00Z", result.DueDate);
            Assert.True(result.PassProcessingFee);
            Assert.Equal("draft", result.Status);
            Assert.Equal("https://bitpay.com/bill?id=3Zpmji8bRKxWJo2NJbWX5H&resource=bills", result.Url);
            Assert.Equal("3Zpmji8bRKxWJo2NJbWX5H", result.Id);
            Assert.Equal("7HyKWn3d4xdhAMQYAEVxVq", result.Merchant);
            Assert.Equal("NV35GRWtrdB2cmGEjY4LKY", result.Items.First().Id);
            Assert.Equal("Test Item 1", result.Items.First().Description);
            Assert.Equal(6.0M, result.Items.First().Price);
            Assert.Equal(1, result.Items.First().Quantity);
            Assert.Equal("Apy3i2TpzHRYP8tJCkrZMT", result.Items.Last().Id);
            Assert.Equal("Test Item 2", result.Items.Last().Description);
            Assert.Equal(4.0M, result.Items.Last().Price);
            Assert.Equal(1, result.Items.Last().Quantity);
        }
        
        [Fact]
        public void it_should_return_bill()
        {
            // given
            var testedClass = GetTestedClassAsMerchant();
            var requestParameters = new Dictionary<string, dynamic> {{"token", MerchantToken}};

            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "createBillResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "bills/3Zpmji8bRKxWJo2NJbWX5H",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "bills/3Zpmji8bRKxWJo2NJbWX5H")
            });
            
            // when
            var result = testedClass.GetBill("3Zpmji8bRKxWJo2NJbWX5H").Result;
        
            // then
            Assert.Equal("USD", result.Currency);
            Assert.Equal("6EBQR37MgDJPfEiLY3jtRq7eTP2aodR5V5wmXyyZhru5FM5yF4RCGKYQtnT7nhwHjA", result.Token);
            Assert.Equal("john@doe.com", result.Email);
            Assert.Equal("bill1234-EFGH", result.Number);
            Assert.Equal("John Doe", result.Name);
            Assert.Equal("2630 Hegal Place", result.Address1);
            Assert.Equal("Apt 42", result.Address2);
            Assert.Equal("Alexandria", result.City);
            Assert.Equal("VA", result.State);
            Assert.Equal("23242", result.Zip);
            Assert.Equal("jane@doe.com", result.Cc?.First());
            Assert.Equal("555-123-456", result.Phone);
            Assert.Equal("2021-05-31T00:00:00Z", result.DueDate);
            Assert.True(result.PassProcessingFee);
            Assert.Equal("draft", result.Status);
            Assert.Equal("https://bitpay.com/bill?id=3Zpmji8bRKxWJo2NJbWX5H&resource=bills", result.Url);
            Assert.Equal("3Zpmji8bRKxWJo2NJbWX5H", result.Id);
            Assert.Equal("7HyKWn3d4xdhAMQYAEVxVq", result.Merchant);
            Assert.Equal("NV35GRWtrdB2cmGEjY4LKY", result.Items.First().Id);
            Assert.Equal("Test Item 1", result.Items.First().Description);
            Assert.Equal(6.0M, result.Items.First().Price);
            Assert.Equal(1, result.Items.First().Quantity);
            Assert.Equal("Apy3i2TpzHRYP8tJCkrZMT", result.Items.Last().Id);
            Assert.Equal("Test Item 2", result.Items.Last().Description);
            Assert.Equal(4.0M, result.Items.Last().Price);
            Assert.Equal(1, result.Items.Last().Quantity);
        }
        
        [Fact]
        public void it_should_return_bills_by_status()
        {
            // given
            var testedClass = GetTestedClassAsMerchant();
            var requestParameters = new Dictionary<string, dynamic> {{"token", MerchantToken}, {"status", "draft"}};

            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getBillsResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "bills",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "bills")
            });
            
            // when
            var result = testedClass.GetBills("draft").Result;
        
            // then
            Assert.Equal("X6KJbe9RxAGWNReCwd1xRw", result.First().Id);
            Assert.Equal("USD", result.First().Currency);
            Assert.Equal("6EBQR37MgDJPfEiLY3jtRqBMYLg8XSDqhp2kp7VSDqCMHGHnsw4bqnnwQmtehzCvSo", result.First().Token);
            Assert.Equal("john@doe.com", result.First().Email);
            Assert.Equal("bill1234-ABCD", result.First().Number);
            Assert.Equal("John Doe", result.First().Name);
            Assert.Equal("2630 Hegal Place", result.First().Address1);
            Assert.Equal("Apt 42", result.First().Address2);
            Assert.Equal("Alexandria", result.First().City);
            Assert.Equal("VA", result.First().State);
            Assert.Equal("23242", result.First().Zip);
            Assert.Equal("jane@doe.com", result.First().Cc?.First());
            Assert.Equal("555-123-456", result.First().Phone);
            Assert.Equal("2021-05-31T00:00:00Z", result.First().DueDate);
            Assert.True(result.First().PassProcessingFee);
            Assert.Equal("draft", result.First().Status);
            Assert.Equal("https://bitpay.com/bill?id=X6KJbe9RxAGWNReCwd1xRw&resource=bills", result.First().Url);
            Assert.Equal("7HyKWn3d4xdhAMQYAEVxVq", result.First().Merchant);
            Assert.Equal("EL4vx41Nxc5RYhbqDthjE", result.First().Items.First().Id);
            Assert.Equal("Test Item 1", result.First().Items.First().Description);
            Assert.Equal(6.0M, result.First().Items.First().Price);
            Assert.Equal(1, result.First().Items.First().Quantity);
            Assert.Equal("6spPADZ2h6MfADvnhfsuBt", result.First().Items.Last().Id);
            Assert.Equal("Test Item 2", result.First().Items.Last().Description);
            Assert.Equal(4.0M, result.First().Items.Last().Price);
            Assert.Equal(1, result.First().Items.Last().Quantity);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void it_should_update_bill()
        {
            // given
            var billId = "3Zpmji8bRKxWJo2NJbWX5H";
            var testedClass = GetTestedClassAsMerchant();

            var newItem = new Item (price: 5.00M, quantity: 1) { Description = "Test Item 3" };
            var billToUpdate = GetBill();
            billToUpdate.Status = "draft";
            billToUpdate.Items.Add(newItem);

            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "updateBillResponse.json"));
            _bitPayClient.Setup(b => b.Put(
                "bills/" + billId,
                File.ReadAllText(GetJsonResponsePath() + "updateBillRequest.json")
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Put, "bills/3Zpmji8bRKxWJo2NJbWX5H")
            });
            
            // when
            var result = testedClass.UpdateBill(billToUpdate, billId).Result;
        
            // then
            Assert.Equal("USD", result.Currency);
            Assert.Equal("7dnoyMe27VDKY1WNrCTqgK5RWbEi4XkvBSUTTwET6XnNYfWKYdrnSyg7myn7oc3vms", result.Token);
            Assert.Equal("john@doe.com", result.Email);
            Assert.Equal("bill1234-EFGH", result.Number);
            Assert.Equal("John Doe", result.Name);
            Assert.Equal("2630 Hegal Place", result.Address1);
            Assert.Equal("Apt 42", result.Address2);
            Assert.Equal("Alexandria", result.City);
            Assert.Equal("VA", result.State);
            Assert.Equal("23242", result.Zip);
            Assert.Equal("jane@doe.com", result.Cc?.First());
            Assert.Equal("555-123-456", result.Phone);
            Assert.Equal("2021-05-31T00:00:00Z", result.DueDate);
            Assert.True(result.PassProcessingFee);
            Assert.Equal("draft", result.Status);
            Assert.Equal("https://bitpay.com/bill?id=3Zpmji8bRKxWJo2NJbWX5H&resource=bills", result.Url);
            Assert.Equal("3Zpmji8bRKxWJo2NJbWX5H", result.Id);
            Assert.Equal("7HyKWn3d4xdhAMQYAEVxVq", result.Merchant);
            Assert.Equal("8vXbhqWDL1A9F66ZwJAiyJ", result.Items.First().Id);
            Assert.Equal("Test Item 1", result.Items.First().Description);
            Assert.Equal(6.0M, result.Items.First().Price);
            Assert.Equal(1, result.Items.First().Quantity);
            Assert.Equal("89xhSLYPnLDBczsQHCvJ2D", result.Items.Last().Id);
            Assert.Equal("Test Item 3", result.Items.Last().Description);
            Assert.Equal(5.0M, result.Items.Last().Price);
            Assert.Equal(1, result.Items.Last().Quantity);
        }

        [Fact]
        public void it_should_deliver_bill()
        {
            // given
            var testedClass = GetTestedClassAsMerchant();

            HttpContent response = new StringContent("{\"data\": \"Success\"}");
            _bitPayClient.Setup(b => b.Post(
                "bills/3Zpmji8bRKxWJo2NJbWX5H/deliveries",
            "{\"token\":\"billToken\"}",
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Post, "bills/3Zpmji8bRKxWJo2NJbWX5H/deliveries")
            });
            
            // when
            var result = testedClass.DeliverBill("3Zpmji8bRKxWJo2NJbWX5H", "billToken").Result;
        
            // then
            Assert.Equal("Success", result);
        }

        [Fact]
        public void it_should_get_currency_info()
        {
            // given
            var testedClass = GetTestedClassAsMerchant();

            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getCurrenciesResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "currencies",
                null,
                false
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "currencies")
            });
            
            // when
            var result = testedClass.GetCurrencyInfo("BTC").Result;

            // then
            Assert.Equal("BTC", result.Code);
            Assert.Equal("฿", result.Symbol);
        }

        [Fact]
        public void it_should_create_invoice()
        {
            // given
            var invoice = GetInvoiceExample();

            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "createInvoiceResponse.json"));
            _bitPayClient.Setup(b => b.Post(
                "invoices",
                File.ReadAllText(GetJsonResponsePath() + "createInvoiceRequest.json"),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Post, "invoices")
            });
            
            // when
            var result = GetTestedClassAsMerchant().CreateInvoice(invoice).Result;
            
            // then
            Assert.Equal("payment#1234", result.ResourceGuid);
            Assert.Equal("https://bitpay.com/invoice?id=G3viJEJgE8Jk2oekSdgT2A", result.Url);
            Assert.Equal("\"{ \"ref\" : 711454, \"item\" : \"test_item\" }\"", result.PosData);
            Assert.Equal("new", result.Status);
            Assert.Equal(20, result.Price);
            Assert.Equal("USD", result.Currency);
            Assert.Equal("Item 1", result.ItemDesc);
            Assert.Equal("20210511_fghij", result.OrderId);
            Assert.Equal(1620733980748L, result.InvoiceTime);
            Assert.Equal(1620734880748L, result.ExpirationTime);
            Assert.Equal(1620733980807L, result.CurrentTime);
            Assert.Equal("payment#1234", result.ResourceGuid);
            Assert.Equal("G3viJEJgE8Jk2oekSdgT2A", result.Id);
            Assert.False(result.LowFeeDetected);
            Assert.Equal(0, result.AmountPaid);
            Assert.Equal("0", result.DisplayAmountPaid);
            Assert.Equal("false", result.ExceptionStatus);
            Assert.Equal(6, result.TargetConfirmations);
            Assert.Equal(new List<InvoiceTransaction>(), result.Transactions);
            Assert.Equal("medium", result.TransactionSpeed);
            Assert.Equal("john@doe.com", result.Buyer?.Email);
            Assert.Equal("https://merchantwebsite.com/shop/return", result.RedirectUrl);
            Assert.False(result.AutoRedirect);
            Assert.Equal("https://merchantwebsite.com/shop/cancel", result.CloseUrl);
            Assert.False(result.RefundAddressRequestPending);
            Assert.Equal("john@doe.com", result.BuyerProvidedEmail);
            Assert.Equal("john@doe.com", result.BuyerProvidedInfo?.EmailAddress);
            Assert.Equal("bitpay", result.BuyerProvidedInfo?.SelectedWallet);
            Assert.Equal("BTC", result.BuyerProvidedInfo?.SelectedTransactionCurrency);
            Assert.Equal(false, result.SupportedTransactionCurrencies.GetSupportedCurrency("SHIB_m").Enabled);
            Assert.Equal("Some Reason", result.SupportedTransactionCurrencies.GetSupportedCurrency("SHIB_m").Reason);
            Assert.Equal("08.01.2024 23:50:56", result.RefundAddresses[0]["n2MDYgEhxCAnuoVd1JpPmvxZShE6rQA6zv"].Date.ToString());
        }

        [Fact]
        public void it_should_get_invoice()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic> {{"token", MerchantToken}};

            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getInvoiceResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "invoices/G3viJEJgE8Jk2oekSdgT2A",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "invoices/G3viJEJgE8Jk2oekSdgT2A")
            });

            // when
            var result = GetTestedClassAsMerchant().GetInvoice("G3viJEJgE8Jk2oekSdgT2A").Result;
            
            // then
            Assert.Equal("payment#1234", result.ResourceGuid);
            Assert.Equal("https://bitpay.com/invoice?id=G3viJEJgE8Jk2oekSdgT2A", result.Url);
            Assert.Equal("\"{ \"ref\" : 711454, \"item\" : \"test_item\" }\"", result.PosData);
            Assert.Equal("confirmed", result.Status);
            Assert.Equal(20, result.Price);
            Assert.Equal("USD", result.Currency);
            Assert.Equal("20210511_fghij", result.OrderId);
            Assert.Equal(1620733980748L, result.InvoiceTime);
            Assert.Equal(1620734880748L, result.ExpirationTime);
            Assert.Equal(1620734253073L, result.CurrentTime);
            Assert.Equal("payment#1234", result.ResourceGuid);
            Assert.Equal("G3viJEJgE8Jk2oekSdgT2A", result.Id);
            Assert.False(result.LowFeeDetected);
            Assert.Equal(739100, result.AmountPaid);
            Assert.Equal("0.007391", result.DisplayAmountPaid);
            Assert.Equal("false", result.ExceptionStatus);
            Assert.Equal(6, result.TargetConfirmations);
            Assert.Equal(739100, result.Transactions?.First().Amount);
            Assert.Equal("medium", result.TransactionSpeed);
            Assert.Equal("john@doe.com", result.Buyer?.Email);
            Assert.Equal("https://merchantwebsite.com/shop/return", result.RedirectUrl);
            Assert.False(result.AutoRedirect);
            Assert.Equal("https://merchantwebsite.com/shop/cancel", result.CloseUrl);
            Assert.False(result.RefundAddressRequestPending);
            Assert.Equal("john@doe.com", result.BuyerProvidedEmail);
            Assert.Equal("john@doe.com", result.BuyerProvidedInfo?.EmailAddress);
            Assert.Equal("bitpay", result.BuyerProvidedInfo?.SelectedWallet);
            Assert.Equal("BCH", result.BuyerProvidedInfo?.SelectedTransactionCurrency);
        }

        [Fact]
        public void it_should_get_invoice_by_guid()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic> {{"token", MerchantToken}};

            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getInvoiceResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "invoices/guid/payment#1234",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "invoices/guid/payment#1234")
            });

            // when
            var result = GetTestedClassAsMerchant().GetInvoiceByGuid("payment#1234").Result;
            
            // then
            Assert.Equal("payment#1234", result.ResourceGuid);
            Assert.Equal("https://bitpay.com/invoice?id=G3viJEJgE8Jk2oekSdgT2A", result.Url);
            Assert.Equal("\"{ \"ref\" : 711454, \"item\" : \"test_item\" }\"", result.PosData);
            Assert.Equal("confirmed", result.Status);
            Assert.Equal(20, result.Price);
            Assert.Equal("USD", result.Currency);
            Assert.Equal("20210511_fghij", result.OrderId);
            Assert.Equal(1620733980748L, result.InvoiceTime);
            Assert.Equal(1620734880748L, result.ExpirationTime);
            Assert.Equal(1620734253073L, result.CurrentTime);
            Assert.Equal("payment#1234", result.ResourceGuid);
            Assert.Equal("G3viJEJgE8Jk2oekSdgT2A", result.Id);
            Assert.False(result.LowFeeDetected);
            Assert.Equal(739100, result.AmountPaid);
            Assert.Equal("0.007391", result.DisplayAmountPaid);
            Assert.Equal("false", result.ExceptionStatus);
            Assert.Equal(6, result.TargetConfirmations);
            Assert.Equal(739100, result.Transactions?.First().Amount);
            Assert.Equal("medium", result.TransactionSpeed);
            Assert.Equal("john@doe.com", result.Buyer?.Email);
            Assert.Equal("https://merchantwebsite.com/shop/return", result.RedirectUrl);
            Assert.False(result.AutoRedirect);
            Assert.Equal("https://merchantwebsite.com/shop/cancel", result.CloseUrl);
            Assert.False(result.RefundAddressRequestPending);
            Assert.Equal("john@doe.com", result.BuyerProvidedEmail);
            Assert.Equal("john@doe.com", result.BuyerProvidedInfo?.EmailAddress);
            Assert.Equal("bitpay", result.BuyerProvidedInfo?.SelectedWallet);
            Assert.Equal("BCH", result.BuyerProvidedInfo?.SelectedTransactionCurrency);
        }

        [Fact]
        public void it_should_get_invoices()
        {
            // given
            var dateStart = new DateTime(2021, 5, 10);
            var dateEnd = new DateTime(2021, 5, 12);
            var parameters = new Dictionary<string, dynamic?> {{"limit", 10}};
            var requestParameters = new Dictionary<string, dynamic>
            {
                {"limit", 10}, {"token", "merchantToken"}, {"dateStart", "2021-05-10"}, {"dateEnd", "2021-05-12"}
            };

            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getInvoicesResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "invoices",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "invoices")
            });

            // when
            var result = GetTestedClassAsMerchant().GetInvoices(dateStart, dateEnd, parameters).Result;
            
            // then
            Assert.Equal(2, result.Count);
            Assert.Equal("payment#1234", result.First().ResourceGuid);
            Assert.Equal("https://bitpay.com/invoice?id=KSnNNfoMDsbRzd1U9ypmVH", result.First().Url);
            Assert.Equal("\"{ \"ref\" : 711454, \"item\" : \"test_item\" }\"", result.First().PosData);
            Assert.Equal("confirmed", result.First().Status);
            Assert.Equal(20, result.First().Price);
            Assert.Equal("USD", result.First().Currency);
            Assert.Equal("20210511_abcde", result.First().OrderId);
            Assert.Equal(1620734545366L, result.First().InvoiceTime);
            Assert.Equal(1620735445366L, result.First().ExpirationTime);
            Assert.Equal(1620744196444L, result.First().CurrentTime);
            Assert.Equal("payment#1234", result.First().ResourceGuid);
            Assert.Equal("KSnNNfoMDsbRzd1U9ypmVH", result.First().Id);
            Assert.False(result.First().LowFeeDetected);
            Assert.Equal(744500, result.First().AmountPaid);
            Assert.Equal("0.007445", result.First().DisplayAmountPaid);
            Assert.Equal("false", result.First().ExceptionStatus);
            Assert.Equal(6, result.First().TargetConfirmations);
            Assert.Equal(744500, result.First().Transactions?.First().Amount);
            Assert.Equal("medium", result.First().TransactionSpeed);
            Assert.Equal("john@doe.com", result.First().Buyer?.Email);
            Assert.Equal("https://merchantwebsite.com/shop/return", result.First().RedirectUrl);
            Assert.False(result.First().AutoRedirect);
            Assert.Equal("https://merchantwebsite.com/shop/cancel", result.First().CloseUrl);
            Assert.False(result.First().RefundAddressRequestPending);
            Assert.Equal("john@doe.com", result.First().BuyerProvidedEmail);
            Assert.Equal("john@doe.com", result.First().BuyerProvidedInfo?.EmailAddress);
            Assert.Equal("bitpay", result.First().BuyerProvidedInfo?.SelectedWallet);
            Assert.Equal("BCH", result.First().BuyerProvidedInfo?.SelectedTransactionCurrency);
        }

        [Fact]
        public void it_should_update_invoice()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "updateInvoiceResponse.json"));
            _bitPayClient.Setup(b => b.Put(
                "invoices/G3viJEJgE8Jk2oekSdgT2A",
                "{\"buyerSms\":\"+12223334444\",\"token\":\"merchantToken\"}")
            ).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Put, "invoices/G3viJEJgE8Jk2oekSdgT2A")
            });
            var testedClass = GetTestedClassAsMerchant();
            var updatedData = new Dictionary<string, dynamic?> {{"buyerSms", "+12223334444"}};

            // when
            var result = testedClass.UpdateInvoice("G3viJEJgE8Jk2oekSdgT2A", updatedData).Result;

            // then
            Assert.Equal("payment#1234", result.ResourceGuid);
            Assert.Equal("+12223334444", result.BuyerProvidedInfo?.Sms);
            Assert.Equal("https://bitpay.com/invoice?id=G3viJEJgE8Jk2oekSdgT2A", result.Url);
            Assert.Equal("\"{ \"ref\" : 711454, \"item\" : \"test_item\" }\"", result.PosData);
            Assert.Equal("confirmed", result.Status);
            Assert.Equal(20, result.Price);
            Assert.Equal("USD", result.Currency);
            Assert.Equal("20210511_fghij", result.OrderId);
            Assert.Equal(1620733980748L, result.InvoiceTime);
            Assert.Equal(1620734880748L, result.ExpirationTime);
            Assert.Equal(1620734253073L, result.CurrentTime);
            Assert.Equal("payment#1234", result.ResourceGuid);
            Assert.Equal("G3viJEJgE8Jk2oekSdgT2A", result.Id);
            Assert.False(result.LowFeeDetected);
            Assert.Equal(739100L, result.AmountPaid);
            Assert.Equal("0.007391", result.DisplayAmountPaid);
            Assert.Equal("false", result.ExceptionStatus);
            Assert.Equal(6, result.TargetConfirmations);
            Assert.Single(result.Transactions!);
            Assert.Equal("medium", result.TransactionSpeed);
            Assert.Equal("john@doe.com", result.Buyer?.Email);
            Assert.Equal("https://merchantwebsite.com/shop/return", result.RedirectUrl);
            Assert.False(result.AutoRedirect);
            Assert.Equal("https://merchantwebsite.com/shop/cancel", result.CloseUrl);
            Assert.Equal(new List<dynamic>(), result.RefundAddresses);
            Assert.False(result.RefundAddressRequestPending);
            Assert.Equal("john@doe.com", result.BuyerProvidedEmail);
            Assert.Equal("john@doe.com", result.BuyerProvidedInfo?.EmailAddress);
            Assert.Equal("bitpay", result.BuyerProvidedInfo?.SelectedWallet);
            Assert.Equal("BCH", result.BuyerProvidedInfo?.SelectedTransactionCurrency);
        }

        [Fact]
        public void it_should_cancel_invoice()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic>
            {
                {"token", MerchantToken}, { "forceCancel", true}
            };
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "cancelInvoiceSuccessResponse.json"));
            _bitPayClient.Setup(b => b.Delete(
                "invoices/Hpqc63wvE1ZjzeeH4kEycF",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!))
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Delete, "invoices/Hpqc63wvE1ZjzeeH4kEycF")
            });

            // when
            var result = this.GetTestedClassAsMerchant().CancelInvoice("Hpqc63wvE1ZjzeeH4kEycF").Result;

            // then
            Assert.Equal("payment#1234", result.ResourceGuid);
            Assert.Equal("https://bitpay.com/invoice?id=G3viJEJgE8Jk2oekSdgT2A", result.Url);
            Assert.Equal("\"{ \"ref\" : 711454, \"item\" : \"test_item\" }\"", result.PosData);
            Assert.Equal("expired", result.Status);
            Assert.Equal(20, result.Price);
            Assert.Equal("USD", result.Currency);
            Assert.Equal("20210511_fghij", result.OrderId);
            Assert.Equal(1620733980748L, result.InvoiceTime);
            Assert.Equal(1620734880748L, result.ExpirationTime);
            Assert.Equal(1620734253073L, result.CurrentTime);
            Assert.Equal("payment#1234", result.ResourceGuid);
            Assert.Equal("G3viJEJgE8Jk2oekSdgT2A", result.Id);
            Assert.False(result.LowFeeDetected);
            Assert.Equal(739100, result.AmountPaid);
            Assert.Equal("0.007391", result.DisplayAmountPaid);
            Assert.Equal("false", result.ExceptionStatus);
            Assert.Equal(6, result.TargetConfirmations);
            Assert.Equal(new List<InvoiceTransaction>(), result.Transactions);
            Assert.Equal("medium", result.TransactionSpeed);
            Assert.Equal("john@doe.com", result.Buyer?.Email);
            Assert.Equal("https://merchantwebsite.com/shop/return", result.RedirectUrl);
            Assert.False(result.AutoRedirect);
            Assert.Equal("https://merchantwebsite.com/shop/cancel", result.CloseUrl);
            Assert.False(result.RefundAddressRequestPending);
            Assert.Equal("john@doe.com", result.BuyerProvidedEmail);
            Assert.Equal("john@doe.com", result.BuyerProvidedInfo?.EmailAddress);
            Assert.Equal("bitpay", result.BuyerProvidedInfo?.SelectedWallet);
            Assert.Equal("BCH", result.BuyerProvidedInfo?.SelectedTransactionCurrency);
        }
        
        [Fact]
        public void it_should_cancel_invoice_by_guid()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic>
            {
                {"token", MerchantToken}, { "forceCancel", true}
            };
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "cancelInvoiceSuccessResponse.json"));
            _bitPayClient.Setup(b => b.Delete(
                "invoices/guid/payment#1234",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!))
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Delete, "invoices/guid/payment#1234")
            });

            // when
            var result = this.GetTestedClassAsMerchant().CancelInvoiceByGuid("payment#1234").Result;

            // then
            Assert.Equal("payment#1234", result.ResourceGuid);
            Assert.Equal("https://bitpay.com/invoice?id=G3viJEJgE8Jk2oekSdgT2A", result.Url);
            Assert.Equal("\"{ \"ref\" : 711454, \"item\" : \"test_item\" }\"", result.PosData);
            Assert.Equal("expired", result.Status);
            Assert.Equal(20, result.Price);
            Assert.Equal("USD", result.Currency);
            Assert.Equal("20210511_fghij", result.OrderId);
            Assert.Equal(1620733980748L, result.InvoiceTime);
            Assert.Equal(1620734880748L, result.ExpirationTime);
            Assert.Equal(1620734253073L, result.CurrentTime);
            Assert.Equal("payment#1234", result.ResourceGuid);
            Assert.Equal("G3viJEJgE8Jk2oekSdgT2A", result.Id);
            Assert.False(result.LowFeeDetected);
            Assert.Equal(739100, result.AmountPaid);
            Assert.Equal("0.007391", result.DisplayAmountPaid);
            Assert.Equal("false", result.ExceptionStatus);
            Assert.Equal(6, result.TargetConfirmations);
            Assert.Equal(new List<InvoiceTransaction>(), result.Transactions);
            Assert.Equal("medium", result.TransactionSpeed);
            Assert.Equal("john@doe.com", result.Buyer?.Email);
            Assert.Equal("https://merchantwebsite.com/shop/return", result.RedirectUrl);
            Assert.False(result.AutoRedirect);
            Assert.Equal("https://merchantwebsite.com/shop/cancel", result.CloseUrl);
            Assert.False(result.RefundAddressRequestPending);
            Assert.Equal("john@doe.com", result.BuyerProvidedEmail);
            Assert.Equal("john@doe.com", result.BuyerProvidedInfo?.EmailAddress);
            Assert.Equal("bitpay", result.BuyerProvidedInfo?.SelectedWallet);
            Assert.Equal("BCH", result.BuyerProvidedInfo?.SelectedTransactionCurrency);
        }

        [Fact]
        public void it_should_request_invoice_webhook_to_be_resent()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "invoiceWebhookResponse.json"));
            _bitPayClient.Setup(b => b.Post(
                "invoices/Hpqc63wvE1ZjzeeH4kEycF/notifications",
            "{\"token\":\"merchantToken\"}",
                false
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(
                    HttpMethod.Delete, "invoices/Hpqc63wvE1ZjzeeH4kEycF/notifications")
            });

            // when
            var result = GetTestedClassAsMerchant().RequestInvoiceWebhookToBeResent("Hpqc63wvE1ZjzeeH4kEycF").Result;
            
            // then
            Assert.True(result);
        }

        [Fact]
        public void it_should_retrieve_an_invoice_event_token()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic>
            {
                {"token", MerchantToken}
            };
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getInvoiceEventToken.json"));
            _bitPayClient.Setup(b => b.Get(
                "invoices/GZRP3zgNHTDf8F5BmdChKz/events",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
            true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "invoices/GZRP3zgNHTDf8F5BmdChKz/events")
            });

            // when
            var result = GetTestedClassAsMerchant().GetInvoiceEventToken("GZRP3zgNHTDf8F5BmdChKz").Result;

            // then
            Assert.Equal("4MuqDPt93i9Xbf8SnAPniwbGeNLW8A3ScgAmukFMgFUFRqTLuuhVdAFfePPysVqL2P", result.Token);
            Assert.Equal(new List<string> {"payment", "confirmation"}, result.Events);
            Assert.Equal(new List<string> {"subscribe", "unsubscribe"}, result.Actions);
        }

        [Fact]
        public void it_should_get_ledgers()
        {
            var requestParameters = new Dictionary<string, dynamic> {{"token", MerchantToken}};
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getLedgersResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "ledgers",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "ledgers")
            });

            // when
            var result = GetTestedClassAsMerchant().GetLedgers().Result;
            
            // then
            Assert.Equal(3, result.Count);
            Assert.Equal("EUR", result.First().Currency);
            Assert.Equal("BTC", result.Last().Currency);
            Assert.Equal(0.0M, result.First().Balance);
            Assert.Equal(0.000287M, result.Last().Balance);
        }

        [Fact]
        public void it_should_get_ledger_entries()
        {
            var requestParameters = new Dictionary<string, dynamic>
            {
                {"token", MerchantToken}, {"startDate", "2021-05-10"}, {"endDate", "2021-05-31"}
            };
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getLedgerEntriesResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "ledgers/USD",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "ledgers/USD")
            });

            // when
            var result = GetTestedClassAsMerchant().GetLedgerEntries(
                "USD", 
                new DateTime(2021, 5, 10),
                new DateTime(2021, 5, 31)
            ).Result;
            var secondEntry = result[1];
            
            // then
            Assert.Equal(3, result.Count);
            Assert.Equal(1023, secondEntry.Code);
            Assert.Equal("-8000000", secondEntry.Amount);
            Assert.Equal("Invoice Fee", secondEntry.Description);
            Assert.Equal("2021-05-10T20:08:52.919Z", secondEntry.Timestamp.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffK"));
            Assert.Equal(919, secondEntry.Timestamp.Value.Millisecond);
            Assert.Equal("Hpqc63wvE1ZjzeeH4kEycF", secondEntry.InvoiceId);
            Assert.Equal("2630 Hegal Place", secondEntry.Buyer?.Address1);
            Assert.Equal(10, secondEntry.InvoiceAmount);
            Assert.Equal("USD", secondEntry.InvoiceCurrency);
            Assert.Equal("BCH", secondEntry.TransactionCurrency);
            Assert.Equal("XCkhgHKP2pSme4qszMpM3B", secondEntry.Id);
        }

        [Fact]
        public void it_should_submit_payout()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "createPayoutResponse.json"));
            _bitPayClient.Setup(b => b.Post(
                "payouts",
                File.ReadAllText(GetJsonResponsePath() + "createPayoutRequest.json"),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Post, "payouts")
            });
            
            // when
            var result = GetTestedClassAsPayout().SubmitPayout(GetPayoutExample()).Result;
            
            // then
            Assert.Equal(10.0M, result.Amount);
            Assert.Equal("USD", result.Currency);
            Assert.Null(result.DateExecuted);
            Assert.Equal(DateTime.Parse("2021-05-27T09:00:00.000Z").ToUniversalTime(), result.EffectiveDate);
            Assert.Equal("john@doe.com", result.Email);
            Assert.Null(result.ExchangeRates);
            Assert.Equal("JMwv8wQCXANoU2ZZQ9a9GH", result.Id);
            Assert.Equal("John Doe", result.Label);
            Assert.Equal("GBP", result.LedgerCurrency);
            Assert.Null(result.Message);
            Assert.Equal("merchant@email.com", result.NotificationEmail);
            Assert.Equal("https://yournotiticationURL.com/wed3sa0wx1rz5bg0bv97851eqx", result.NotificationUrl);
            Assert.Equal("LDxRZCGq174SF8AnQpdBPB", result.RecipientId);
            Assert.Equal("payout_20210527", result.Reference);
            Assert.Equal(DateTime.Parse("2021-05-27T10:47:37.834Z").ToUniversalTime(), result.RequestDate);
            Assert.Equal("7qohDf2zZnQK5Qanj8oyC2", result.ShopperId);
            Assert.Equal("new", result.Status);
            Assert.Equal("6RZSTPtnzEaroAe2X4YijenRiqteRDNvzbT8NjtcHjUVd9FUFwa7dsX8RFgRDDC5SL", result.Token);
            Assert.Empty(result.Transactions!);
        }
        
        [Fact]
        public void it_should_submit_payout_group()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "createPayoutGroupResponse.json"));
            _bitPayClient.Setup(b => b.Post(
                "payouts/group",
                File.ReadAllText(GetJsonResponsePath() + "createPayoutGroupRequest.json"),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Post, "payouts/group")
            });
            
            // when
            var result = GetTestedClassAsPayout()
                .SubmitPayouts(new List<Payout>{GetPayoutExample(), GetPayoutExample()})
                .Result;
            
            // then
            Assert.NotEmpty(result.Payouts);
            var firstPayout = result.Payouts[0];
            Assert.Equal(10.0M, firstPayout.Amount);
            Assert.Equal("USD", firstPayout.Currency);
            Assert.Null(firstPayout.DateExecuted);
            Assert.Equal(DateTime.Parse("2021-05-27T09:00:00.000Z").ToUniversalTime(), firstPayout.EffectiveDate);
            Assert.Equal("john@doe.com", firstPayout.Email);
            Assert.Null(firstPayout.ExchangeRates);
            Assert.Equal("JMwv8wQCXANoU2ZZQ9a9GH", firstPayout.Id);
            Assert.Equal("John Doe", firstPayout.Label);
            Assert.Equal("GBP", firstPayout.LedgerCurrency);
            Assert.Null(firstPayout.Message);
            Assert.Equal("merchant@email.com", firstPayout.NotificationEmail);
            Assert.Equal("https://yournotiticationURL.com/wed3sa0wx1rz5bg0bv97851eqx", firstPayout.NotificationUrl);
            Assert.Equal("LDxRZCGq174SF8AnQpdBPB", firstPayout.RecipientId);
            Assert.Equal("payout_20210527", firstPayout.Reference);
            Assert.Equal(DateTime.Parse("2021-05-27T10:47:37.834Z").ToUniversalTime(), firstPayout.RequestDate);
            Assert.Equal("7qohDf2zZnQK5Qanj8oyC2", firstPayout.ShopperId);
            Assert.Equal("new", firstPayout.Status);
            Assert.Empty(firstPayout.Transactions!);
            
            Assert.NotEmpty(result.Failed);
            Assert.Equal("Ledger currency is required", result.Failed[0].ErrMessage);
            Assert.Equal("john@doe.com", result.Failed[0].Payee);
        }

        [Fact]
        public void it_should_get_payout()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic> {{"token", PayoutToken}};
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getPayoutResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "payouts/JMwv8wQCXANoU2ZZQ9a9GH",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "payouts/JMwv8wQCXANoU2ZZQ9a9GH")
            });
            
            // when
            var result = GetTestedClassAsPayout().GetPayout("JMwv8wQCXANoU2ZZQ9a9GH").Result;
            
            // then
            Assert.Equal(10.0M, result.Amount);
            Assert.Equal("USD", result.Currency);
            Assert.Equal(DateTime.Parse("2021-05-27T09:00:00.000Z").ToUniversalTime(), result.DateExecuted);
            Assert.Equal(DateTime.Parse("2021-05-27T09:00:00.000Z").ToUniversalTime(), result.EffectiveDate);
            Assert.Equal("john@doe.com", result.Email);
            Assert.Equal(27883.962246420004, result.ExchangeRates?["BTC"].Property("GBP").Value.Value);
            Assert.Equal("JMwv8wQCXANoU2ZZQ9a9GH", result.Id);
            Assert.Equal("John Doe", result.Label);
            Assert.Equal("GBP", result.LedgerCurrency);
            Assert.Null(result.Message);
            Assert.Equal("merchant@email.com", result.NotificationEmail);
            Assert.Equal("https://yournotiticationURL.com/wed3sa0wx1rz5bg0bv97851eqx", result.NotificationUrl);
            Assert.Equal("LDxRZCGq174SF8AnQpdBPB", result.RecipientId);
            Assert.Equal("payout_20210527", result.Reference);
            Assert.Equal(DateTime.Parse("2021-05-27T10:47:37.834Z").ToUniversalTime(), result.RequestDate);
            Assert.Equal("7qohDf2zZnQK5Qanj8oyC2", result.ShopperId);
            Assert.Equal("complete", result.Status);
            Assert.Equal("6RZSTPtnzEaroAe2X4YijenRiqteRDNvzbT8NjtcHjUVd9FUFwa7dsX8RFgRDDC5SL", result.Token);
            Assert.Equal("db53d7e2bf3385a31257ce09396202d9c2823370a5ca186db315c45e24594057", result.Transactions?[0].Txid);
        }

        [Fact]
        public void it_should_cancel_payout()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic> {{"token", PayoutToken}};
            HttpContent response = new StringContent("{\"status\": \"success\",\"data\": {},\"message\": null }");
            _bitPayClient.Setup(b => b.Delete(
                "payouts/KMXZeQigXG6T5abzCJmTcH",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!))
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Delete, "payouts/KMXZeQigXG6T5abzCJmTcH")
            });
            
            // when
            var result = GetTestedClassAsPayout().CancelPayout("KMXZeQigXG6T5abzCJmTcH").Result;
            
            //  then
            Assert.True(result);
        }
        
        [Fact]
        public void it_should_cancel_payout_group()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "cancelPayoutGroupResponse.json"));
            var requestParameters = new Dictionary<string, dynamic> {{"token", PayoutToken}};
            _bitPayClient.Setup(b => b.Delete(
                "payouts/group/KMXZeQigXG6T5abzCJmTcH",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!))
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Delete, "payouts/group/KMXZeQigXG6T5abzCJmTcH")
            });
            
            // when
            var result = GetTestedClassAsPayout().CancelPayouts("KMXZeQigXG6T5abzCJmTcH").Result;
            
            //  then
            Assert.NotEmpty(result.Payouts);
            Assert.NotEmpty(result.Failed);
            Assert.Equal("D8tgWzn1psUua4NYWW1vYo", result.Failed[0].PayoutId);
            Assert.Equal($"PayoutId is missing or invalid", result.Failed[0].ErrMessage);
        }

        [Fact]
        public void it_should_get_payouts()
        {
            // given
            var parameters = new Dictionary<string, dynamic?>
            {
                {"startDate", "2021-05-27"}, {"endDate", "2021-05-31"}
            };
            var requestParameters = new Dictionary<string, dynamic?>
            {
                {"startDate", "2021-05-27"}, {"endDate", "2021-05-31"}, {"token", PayoutToken}
            };
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getPayoutsResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "payouts",
                requestParameters,
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "payouts")
            });
            
            // when
            var result = GetTestedClassAsPayout().GetPayouts(parameters).Result;
            
            //  then
            Assert.Equal(10.0M, result.First().Amount);
            Assert.Equal("USD", result.First().Currency);
            Assert.Null(result.First().DateExecuted);
            Assert.Equal(DateTime.Parse("2021-05-27T09:00:00.000Z").ToUniversalTime(), result.First().EffectiveDate);
            Assert.Equal("john@doe.com", result.First().Email);
            Assert.Equal("JMwv8wQCXANoU2ZZQ9a9GH", result.First().Id);
            Assert.Equal("John Doe", result.First().Label);
            Assert.Equal("GBP", result.First().LedgerCurrency);
            Assert.Null(result.First().Message);
            Assert.Equal("merchant@email.com", result.First().NotificationEmail);
            Assert.Equal("https://yournotiticationURL.com/wed3sa0wx1rz5bg0bv97851eqx", result.First().NotificationUrl);
            Assert.Equal("LDxRZCGq174SF8AnQpdBPB", result.First().RecipientId);
            Assert.Equal("payout_20210527", result.First().Reference);
            Assert.Equal(DateTime.Parse("2021-05-27T10:47:37.8340000Z").ToUniversalTime(), result.First().RequestDate);
            Assert.Equal("7qohDf2zZnQK5Qanj8oyC2", result.First().ShopperId);
            Assert.Equal("complete", result.First().Status);
            Assert.Equal("9pVLfvdjt59q1JiY2JEsf2uzeeEpSqDwwfRAzuFr9CcrxZX25rTnP6HdRhsMBGLArz", result.First().Token);
            Assert.Equal("db53d7e2bf3385a31257ce09396202d9c2823370a5ca186db315c45e24594057", result.First().Transactions?[0].Txid);

            Assert.Equal(10.0M, result[1].Amount);
            Assert.Equal("USD", result[1].Currency);
            Assert.Null(result[1].DateExecuted);
            Assert.Equal(DateTime.Parse("2021-05-28T09:00:00.0000000Z").ToUniversalTime(), result[1].EffectiveDate);
            Assert.Equal("jane@doe.com", result[1].Email);
            Assert.Equal("KMXZeQigXG6T5abzCJmTcH", result[1].Id);
            Assert.Equal("Jane Doe", result[1].Label);
            Assert.Equal("GBP", result[1].LedgerCurrency);
            Assert.Null(result[1].Message);
            Assert.Equal("merchant@email.com", result[1].NotificationEmail);
            Assert.Equal("https://yournotiticationURL.com/wed3sa0wx1rz5bg0bv97851eqx", result[1].NotificationUrl);
            Assert.Equal("LDxRZCGq174SF8AnQpdBPB", result[1].RecipientId);
            Assert.Equal("payout_20210528", result[1].Reference);
            Assert.Equal(DateTime.Parse("2021-05-28T10:23:43.7650000Z").ToUniversalTime(), result[1].RequestDate);
            Assert.Equal("7qohDf2zZnQK5Qanj8oyC2", result[1].ShopperId);
            Assert.Equal("cancelled", result[1].Status);
            Assert.Equal("9pVLfvdjt59q1JiY2JEsf2hr5FsjimfY4qRLFi85tMiXSCkJ9mQ2oSQqYKVangKaro", result[1].Token);
            Assert.Empty(result[1].Transactions!);
        }

        [Fact]
        public void it_should_request_payout_notification()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "sendPayoutNotificationResponse.json"));
            _bitPayClient.Setup(b => b.Post(
                "payouts/JMwv8wQCXANoU2ZZQ9a9GH/notifications",
                "{\"token\":\"payoutToken\"}",
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Post, "payouts/JMwv8wQCXANoU2ZZQ9a9GH/notifications")
            });
            
            // when
            var result = GetTestedClassAsPayout().RequestPayoutNotification("JMwv8wQCXANoU2ZZQ9a9GH").Result;
            
            // then
            Assert.True(result);
        }

        [Fact]
        public void it_should_submit_payout_recipients()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "submitPayoutRecipientsResponse.json"));
            _bitPayClient.Setup(b => b.Post(
                "recipients",
                File.ReadAllText(GetJsonResponsePath() + "submitPayoutRecipientsRequest.json"),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Post, "recipients")
            });
            
            // when
            var result = GetTestedClassAsPayout().SubmitPayoutRecipients(GetPayoutRecipientsExample()).Result;
            
            // then
            Assert.Equal(2, result.Count);
            
            Assert.Equal("alice@email.com", result.First().Email);
            Assert.Equal("Alice", result.First().Label);
            Assert.Equal("invited", result.First().Status);
            Assert.Equal("JA4cEtmBxCp5cybtnh1rds", result.First().Id);
            Assert.Equal("2LVBntm7z92rnuVjVX5ZVaDoUEaoY4LxhZMMzPAMGyXcejgPXVmZ4Ae3oGaCGBFKQf", result.First().Token);
            
            Assert.Equal("bob@email.com", result.Last().Email);
            Assert.Equal("Bob", result.Last().Label);
            Assert.Equal("invited", result.Last().Status);
            Assert.Equal("X3icwc4tE8KJ5hEPNPpDXW", result.Last().Id);
            Assert.Equal("2LVBntm7z92rnuVjVX5ZVaDoUEaoY4LxhZMMzPAMGyXrrBAB9vRY3BVxGLbAa6uEx7", result.Last().Token);
        }

        [Fact]
        public void it_should_get_payout_recipients_by_status()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic>
            {
                {"status", "invited"}, {"limit", "100"}, {"offset", "0"}, {"token", PayoutToken}
            };
            
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getPayoutRecipientsResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "recipients",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "recipients")
            });
            
            // when
            var result = GetTestedClassAsPayout().GetPayoutRecipients("invited").Result;
            
            // then
            Assert.Equal(2, result.Count);
            
            Assert.Equal("alice@email.com", result.First().Email);
            Assert.Equal("Alice", result.First().Label);
            Assert.Equal("invited", result.First().Status);
            Assert.Equal("JA4cEtmBxCp5cybtnh1rds", result.First().Id);
            Assert.Equal("2LVBntm7z92rnuVjVX5ZVaDoUEaoY4LxhZMMzPAMGyXcejgPXVmZ4Ae3oGaCGBFKQf", result.First().Token);
            
            Assert.Equal("bob@email.com", result.Last().Email);
            Assert.Equal("Bob", result.Last().Label);
            Assert.Equal("invited", result.Last().Status);
            Assert.Equal("X3icwc4tE8KJ5hEPNPpDXW", result.Last().Id);
            Assert.Equal("2LVBntm7z92rnuVjVX5ZVaDoUEaoY4LxhZMMzPAMGyXrrBAB9vRY3BVxGLbAa6uEx7", result.Last().Token);
        }

        [Fact]
        public void it_should_get_payout_recipient()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic> {{"token", PayoutToken}};
            
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getPayoutRecipientResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "recipients/JA4cEtmBxCp5cybtnh1rds",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "recipients/JA4cEtmBxCp5cybtnh1rds")
            });
            
            // when
            var result = GetTestedClassAsPayout().GetPayoutRecipient("JA4cEtmBxCp5cybtnh1rds").Result;
            
            // then
            Assert.Equal("john.smith@email.com", result.Email);
            Assert.Equal("John Smith", result.Label);
            Assert.Equal("invited", result.Status);
            Assert.Equal("JA4cEtmBxCp5cybtnh1rds", result.Id);
            Assert.Equal("2LVBntm7z92rnuVjVX5ZVaDoUEaoY4LxhZMMzPAMGyXcejgPXVmZ4Ae3oGaCGBFKQf", result.Token);
        }

        [Fact]
        public void it_should_update_payout_recipient()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "updatePayoutRecipientResponse.json"));
            _bitPayClient.Setup(b => b.Put(
                "recipients/X3icwc4tE8KJ5hEPNPpDXW",
            "{\"email\":\"test@example.com\",\"label\":\"Bob123\",\"token\":\"payoutToken\"}"
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Put, "recipients/X3icwc4tE8KJ5hEPNPpDXW")
            });
            var updatedLabel = "Bob123";
            var updatedPayoutRecipient = new PayoutRecipient("test@example.com", updatedLabel);

            // when
            var result = GetTestedClassAsPayout().UpdatePayoutRecipient("X3icwc4tE8KJ5hEPNPpDXW", updatedPayoutRecipient).Result;
            
            // then
            Assert.Equal(updatedLabel, result.Label);
        }

        [Fact]
        public void it_should_delete_payout_recipient()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic> {{"token", PayoutToken}};
            HttpContent response = new StringContent("{\"status\": \"success\", \"data\": {}, \"message\": null}");
            _bitPayClient.Setup(b => b.Delete(
                "recipients/X3icwc4tE8KJ5hEPNPpDXW",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!))
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Delete, "recipients/X3icwc4tE8KJ5hEPNPpDXW")
            });

            // when
            var result = GetTestedClassAsPayout().DeletePayoutRecipient("X3icwc4tE8KJ5hEPNPpDXW").Result;
            
            // then
            Assert.True(result);
        }

        [Fact]
        public void it_should_get_rate()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getRateResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "rates/BCH/USD",
                null,
                false
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "rates/BCH/USD")
            });

            // when
            var result = GetTestedClass().GetRate("BCH", "USD").Result;
            
            // then
            Assert.Equal(100.99M, result.Value);
        }

        [Fact]
        public void it_should_get_rates()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getRatesResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "rates",
                null,
                false
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "rates")
            });

            // when
            var result = GetTestedClass().GetRates().Result;

            // then
            Assert.Equal(183, result.GetRates().Count);
            Assert.Equal(17725.64M, result.GetRate("USD"));
        }

        [Fact]
        public void it_should_get_rates_by_base_currency()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getRatesResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "rates/BTC",
                null,
                false
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "rates/BTC")
            });

            // when
            var result = GetTestedClass().GetRates("BTC").Result;

            // then
            Assert.Equal(183, result.GetRates().Count);
            Assert.Equal(17725.64M, result.GetRate("USD"));
        }

        [Fact]
        public void it_should_create_refund()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "createRefundResponse.json"));
            _bitPayClient.Setup(b => b.Post(
                "refunds",
                "{\"token\":\"merchantToken\",\"amount\":10.00,\"guid\":\"37bd36bd-6fcb-409c-a907-47f9244302aa\"}",
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Post, "refunds")
            });
            var refund = new Refund {Invoice = "Hpqc63wvE1ZjzeeH4kEycF", Amount = 10.00M};

            // when
            var result = GetTestedClassAsMerchant().CreateRefund(refund).Result;
            
            // then
            Assert.Equal("ee26b5e0-9185-493e-bc12-e846d5fcf07c", result.ResourceGuid);
            Assert.Equal(10, result.Amount);
            Assert.Equal("USD", result.Currency);
            Assert.Equal("Hpqc63wvE1ZjzeeH4kEycF", result.Invoice);
            Assert.Null(result.Preview);
            Assert.False(result.Immediate);
            Assert.False(result.BuyerPaysRefundFee);
            Assert.Equal("Test refund", result.Reference);
            Assert.Equal(0.04M, result.RefundFee);
            Assert.Equal(DateTime.Parse("2021-08-29T20:45:35.368Z").ToUniversalTime(), result.LastRefundNotification);
            Assert.Equal(0.000594M, result.TransactionAmount);
            Assert.Equal(0.0000020M, result.TransactionRefundFee);
            Assert.Equal("BTC", result.TransactionCurrency);
            Assert.Equal("WoE46gSLkJQS48RJEiNw3L", result.Id);
            Assert.Equal(DateTime.Parse("2021-08-29T20:45:34.000Z").ToUniversalTime(), result.RequestDate);
            Assert.Equal("created", result.Status);
        }

        [Fact]
        public void it_should_get_refund_by_id()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic> {{"token", MerchantToken}};
            
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getRefundResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "refunds/WoE46gSLkJQS48RJEiNw3L",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "refunds/WoE46gSLkJQS48RJEiNw3L")
            });
            
            // when
            var result = GetTestedClassAsMerchant().GetRefund("WoE46gSLkJQS48RJEiNw3L").Result;
            
            // then
            Assert.Equal("ee26b5e0-9185-493e-bc12-e846d5fcf07c", result.ResourceGuid);
            Assert.Equal(10, result.Amount);
            Assert.Equal("USD", result.Currency);
            Assert.Equal("Hpqc63wvE1ZjzeeH4kEycF", result.Invoice);
            Assert.Null(result.Preview);
            Assert.False(result.Immediate);
            Assert.False(result.BuyerPaysRefundFee);
            Assert.Equal("Test refund", result.Reference);
            Assert.Equal(0.04M, result.RefundFee);
            Assert.Equal(DateTime.Parse("2021-08-29T20:45:35.368Z").ToUniversalTime(), result.LastRefundNotification);
            Assert.Equal(0.000594M, result.TransactionAmount);
            Assert.Equal(0.0000020M, result.TransactionRefundFee);
            Assert.Equal("BTC", result.TransactionCurrency);
            Assert.Equal("WoE46gSLkJQS48RJEiNw3L", result.Id);
            Assert.Equal(DateTime.Parse("2021-08-29T20:45:34.000Z").ToUniversalTime(), result.RequestDate);
            Assert.Equal("created", result.Status);
        }

        [Fact]
        public void it_should_get_refund_by_guid()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic> {{"token", MerchantToken}};
            
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getRefundResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "refunds/guid/ee26b5e0-9185-493e-bc12-e846d5fcf07c",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "refunds/guid/ee26b5e0-9185-493e-bc12-e846d5fcf07c")
            });
            
            // when
            var result = GetTestedClassAsMerchant().GetRefundByGuid("ee26b5e0-9185-493e-bc12-e846d5fcf07c").Result;
            
            // then
            Assert.Equal("ee26b5e0-9185-493e-bc12-e846d5fcf07c", result.ResourceGuid);
            Assert.Equal(10, result.Amount);
            Assert.Equal("USD", result.Currency);
            Assert.Equal("Hpqc63wvE1ZjzeeH4kEycF", result.Invoice);
            Assert.Null(result.Preview);
            Assert.False(result.Immediate);
            Assert.False(result.BuyerPaysRefundFee);
            Assert.Equal("Test refund", result.Reference);
            Assert.Equal(0.04M, result.RefundFee);
            Assert.Equal(DateTime.Parse("2021-08-29T20:45:35.368Z").ToUniversalTime(), result.LastRefundNotification);
            Assert.Equal(0.000594M, result.TransactionAmount);
            Assert.Equal(0.0000020M, result.TransactionRefundFee);
            Assert.Equal("BTC", result.TransactionCurrency);
            Assert.Equal("WoE46gSLkJQS48RJEiNw3L", result.Id);
            Assert.Equal(DateTime.Parse("2021-08-29T20:45:34.000Z").ToUniversalTime(), result.RequestDate);
            Assert.Equal("created", result.Status);
        }

        [Fact]
        public void it_should_get_refunds_by_invoice_id()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic>
            {
                {"token", MerchantToken}, {"invoiceId", "Hpqc63wvE1ZjzeeH4kEycF"}
            };
            
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getRefundsByInvoiceId.json"));
            _bitPayClient.Setup(b => b.Get(
                "refunds",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "refunds")
            });
            
            // when
            var result = GetTestedClassAsMerchant().GetRefunds("Hpqc63wvE1ZjzeeH4kEycF").Result;
            
            // then
            Assert.Equal(2, result.Count);
            
            Assert.Equal(5.0M, result[0].Amount);
            Assert.Equal("USD", result[0].Currency);
            Assert.Equal("Hpqc63wvE1ZjzeeH4kEycF", result[0].Invoice);
            Assert.Null(result[0].Preview);
            Assert.False(result[0].Immediate);
            Assert.False(result[0].BuyerPaysRefundFee);
            Assert.Equal("Test refund", result[0].Reference);
            Assert.Equal(0.02M, result[0].RefundFee);
            Assert.Equal(DateTime.Parse("2021-08-28T22:49:33.368Z").ToUniversalTime(), result[0].LastRefundNotification);
            Assert.Equal(0.000297M, result[0].TransactionAmount);
            Assert.Equal(0.0000010M, result[0].TransactionRefundFee);
            Assert.Equal("BTC", result[0].TransactionCurrency);
            Assert.Equal("WoE46gSLkJQS48RJEiNw3L", result[0].Id);
            Assert.Equal(DateTime.Parse("2021-08-28T22:49:33.000Z").ToUniversalTime(), result[0].RequestDate);
            Assert.Equal("canceled", result[0].Status);

            Assert.Equal(10, result[1].Amount);
            Assert.Equal("USD", result[1].Currency);
            Assert.Equal("Hpqc63wvE1ZjzeeH4kEycF", result[1].Invoice);
            Assert.Null(result[1].Preview);
            Assert.False(result[1].Immediate);
            Assert.False(result[1].BuyerPaysRefundFee);
            Assert.Equal("Test refund 2", result[1].Reference);
            Assert.Equal(0.04M, result[1].RefundFee);
            Assert.Equal(DateTime.Parse("2021-08-29T20:45:35.368Z").ToUniversalTime(), result[1].LastRefundNotification);
            Assert.Equal(0.000594M, result[1].TransactionAmount);
            Assert.Equal(0.0000020M, result[1].TransactionRefundFee);
            Assert.Equal("BTC", result[1].TransactionCurrency);
            Assert.Equal("WoE46gSLkJQS48RJEiNw3L", result[1].Id);
            Assert.Equal(DateTime.Parse("2021-08-29T20:45:34.000Z").ToUniversalTime(), result[1].RequestDate);
            Assert.Equal("created", result[1].Status);
        }

        [Fact]
        public void it_should_update_refund()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "updateRefundResponse.json"));
            _bitPayClient.Setup(b => b.Put(
                "refunds/X3icwc4tE8KJ5hEPNPpDXW",
                "{\"token\":\"merchantToken\",\"status\":\"created\"}"
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Put, "refunds/X3icwc4tE8KJ5hEPNPpDXW")
            });
            const string status = "created";

            // when
            var result = GetTestedClassAsMerchant().UpdateRefund("X3icwc4tE8KJ5hEPNPpDXW", status).Result;
            
            // then
            Assert.Equal(status, result.Status);
        }

        [Fact]
        public void it_should_update_refund_by_guid()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "updateRefundResponse.json"));
            _bitPayClient.Setup(b => b.Put(
                "refunds/guid/ee26b5e0-9185-493e-bc12-e846d5fcf07c",
                "{\"token\":\"merchantToken\",\"status\":\"created\"}"
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Put, "refunds/guid/ee26b5e0-9185-493e-bc12-e846d5fcf07c")
            });
            const string status = "created";

            // when
            var result = GetTestedClassAsMerchant().UpdateRefundByGuid("ee26b5e0-9185-493e-bc12-e846d5fcf07c", status).Result;
            
            // then
            Assert.Equal(status, result.Status);
        }

        [Fact]
        public void it_should_send_refund_notification()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "sendRefundNotificationResponse.json"));
            _bitPayClient.Setup(b => b.Post(
                "refunds/WoE46gSLkJQS48RJEiNw3L/notifications",
                "{\"token\":\"merchantToken\"}",
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Post, "refunds/WoE46gSLkJQS48RJEiNw3L/notifications")
            });

            // when
            var result = GetTestedClassAsMerchant().SendRefundNotification("WoE46gSLkJQS48RJEiNw3L").Result;
            
            // then
            Assert.True(result);
        }

        [Fact]
        public void it_should_cancel_refund()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic> {{"token", MerchantToken}};
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "cancelRefundResponse.json"));
            _bitPayClient.Setup(b => b.Delete(
                "refunds/WoE46gSLkJQS48RJEiNw3L",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!))
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Delete, "refunds/WoE46gSLkJQS48RJEiNw3L")
            });

            // when
            var result = this.GetTestedClassAsMerchant().CancelRefund("WoE46gSLkJQS48RJEiNw3L").Result;
            
            Assert.Equal("cancelled", result.Status);
            Assert.Equal(10, result.Amount);
            Assert.Equal("USD", result.Currency);
            Assert.Equal("Hpqc63wvE1ZjzeeH4kEycF", result.Invoice);
            Assert.Null(result.Preview);
            Assert.False(result.Immediate);
            Assert.False(result.BuyerPaysRefundFee);
            Assert.Equal("Test refund", result.Reference);
            Assert.Equal(0.04M, result.RefundFee);
            Assert.Equal(DateTime.Parse("2021-08-29T20:45:35.368Z").ToUniversalTime(), result.LastRefundNotification);
            Assert.Equal(0.000594M, result.TransactionAmount);
            Assert.Equal(0.0000020M, result.TransactionRefundFee);
            Assert.Equal("BTC", result.TransactionCurrency);
            Assert.Equal("WoE46gSLkJQS48RJEiNw3L", result.Id);
            Assert.Equal(DateTime.Parse("2021-08-29T20:45:34.000Z").ToUniversalTime(), result.RequestDate);
        }

        [Fact]
        public void it_should_cancel_refund_by_guid()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic> {{"token", MerchantToken}};
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "cancelRefundResponse.json"));
            _bitPayClient.Setup(b => b.Delete(
                "refunds/guid/WoE46gSLkJQS48RJEiNw3L",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!))
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Delete, "refunds/guid/WoE46gSLkJQS48RJEiNw3L")
            });

            // when
            var result = this.GetTestedClassAsMerchant().CancelRefundByGuid("WoE46gSLkJQS48RJEiNw3L").Result;
            
            Assert.Equal("cancelled", result.Status);
            Assert.Equal(10, result.Amount);
            Assert.Equal("USD", result.Currency);
            Assert.Equal("Hpqc63wvE1ZjzeeH4kEycF", result.Invoice);
            Assert.Null(result.Preview);
            Assert.False(result.Immediate);
            Assert.False(result.BuyerPaysRefundFee);
            Assert.Equal("Test refund", result.Reference);
            Assert.Equal(0.04M, result.RefundFee);
            Assert.Equal(DateTime.Parse("2021-08-29T20:45:35.368Z").ToUniversalTime(), result.LastRefundNotification);
            Assert.Equal(0.000594M, result.TransactionAmount);
            Assert.Equal(0.0000020M, result.TransactionRefundFee);
            Assert.Equal("BTC", result.TransactionCurrency);
            Assert.Equal("WoE46gSLkJQS48RJEiNw3L", result.Id);
            Assert.Equal(DateTime.Parse("2021-08-29T20:45:34.000Z").ToUniversalTime(), result.RequestDate);
        }

        [Fact]
        public void it_should_get_settlements()
        {
            // given
            var parameters = new Dictionary<string, dynamic?>
            {
                {"startDate", "2021-05-10"}, {"endDate", "2021-05-12"}, {"status", "processing"}
            };
            var requestParameters = new Dictionary<string, dynamic>
            {
                {"startDate", "2021-05-10"}, {"endDate", "2021-05-12"}, {"status", "processing"}, {"token", MerchantToken}
            };
            
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getSettlementsResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "settlements",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "settlements")
            });
            
            // when
            var result = GetTestedClassAsMerchant().GetSettlements(parameters).Result;
            
            // then
            Assert.Equal(2, result.Count);
            Assert.Equal("KBkdURgmE3Lsy9VTnavZHX", result[0].Id);
            Assert.Equal("YJCgTf3jrXHkUVzLQ7y4eg", result[0].AccountId);
            Assert.Equal("EUR", result[0].Currency);
            Assert.Equal("Test Organization", result[0].PayoutInfo.Name);
            Assert.Equal("NL85ABNA0000000000", result[0].PayoutInfo.Account);
            Assert.Equal("Corporate account", result[0].PayoutInfo.Label);
            Assert.Equal("Netherlands", result[0].PayoutInfo.BankCountry);
            Assert.Equal("Test", result[0].PayoutInfo.Bank);
            Assert.Equal("RABONL2U", result[0].PayoutInfo.Swift);
            Assert.Equal("processing", result[0].Status);
            Assert.Equal(DateTime.Parse("2021-05-10T09:05:00.176Z").ToUniversalTime(), result[0].DateCreated);
            Assert.Equal(DateTime.Parse("2021-05-10T11:52:29.681Z").ToUniversalTime(), result[0].DateExecuted);
            Assert.Null(result[0].DateCompleted);
            Assert.Equal(DateTime.Parse("2021-05-09T09:00:00.000Z").ToUniversalTime(), result[0].OpeningDate);
            Assert.Equal(DateTime.Parse("2021-05-10T09:00:00.000Z").ToUniversalTime(), result[0].ClosingDate);
            Assert.Equal(1.27m, result[0].OpeningBalance);
            Assert.Equal(20.82m, result[0].LedgerEntriesSum);
            Assert.Empty(result[0].WithHoldings!);
            Assert.Equal(590.08m, result[0].WithHoldingsSum);
            Assert.Equal(22.09m, result[0].TotalAmount);
            Assert.Null(result[0].LedgerEntries);
            Assert.Equal("2gBtViSiBWSEJGo1LfaMFHoaBRzE2jek2VitKAYeenj2SRiTVSCgRvs1WTN8w4w8Lc", result[0].Token);

            Assert.Equal("RPWTabW8urd3xWv2To989v", result[1].Id);
            Assert.Equal("YJCgTf3jrXHkUVzLQ7y4eg", result[1].AccountId);
            Assert.Equal("EUR", result[1].Currency);
            Assert.Equal("Test Organization", result[1].PayoutInfo.Name);
            Assert.Equal("NL85ABNA0000000000", result[1].PayoutInfo.Account);
            Assert.Equal("Corporate account", result[1].PayoutInfo.Label);
            Assert.Equal("Netherlands", result[1].PayoutInfo.BankCountry);
            Assert.Equal("Test", result[1].PayoutInfo.Bank);
            Assert.Equal("RABONL2U", result[1].PayoutInfo.Swift);
            Assert.Equal("processing", result[1].Status);
            Assert.Equal(DateTime.Parse("2021-05-11T09:05:00.176Z").ToUniversalTime(), result[1].DateCreated);
            Assert.Equal(DateTime.Parse("2021-05-11T11:52:29.681Z").ToUniversalTime(), result[1].DateExecuted);
            Assert.Null(result[1].DateCompleted);
            Assert.Equal(DateTime.Parse("2021-05-10T09:00:00.000Z").ToUniversalTime(), result[1].OpeningDate);
            Assert.Equal(DateTime.Parse("2021-05-11T09:00:00.000Z").ToUniversalTime(), result[1].ClosingDate);
            Assert.Equal(23.27m, result[1].OpeningBalance);
            Assert.Equal(20.82m, result[1].LedgerEntriesSum);
            Assert.Equal(8.21m, result[1].WithHoldings?[0].Amount);
            Assert.Equal(8.21m, result[1].WithHoldingsSum);
            Assert.Equal(35.88m, result[1].TotalAmount);
            Assert.Null(result[1].LedgerEntries);
            Assert.Equal("2gBtViSiBWSEJitKAYSCgRvs1WTN8w4Go1Leenj2SRiTVFHoaBRzE2jek2VfaMw8Lc", result[1].Token);
        }

        [Fact]
        public void it_should_get_settlement()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic> {{"token", MerchantToken}};
            
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getSettlementResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "settlements/DNFnN3fFjjzLn6if5bdGJC",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "settlements/DNFnN3fFjjzLn6if5bdGJC")
            });
            
            // when
            var result = GetTestedClassAsMerchant().GetSettlement("DNFnN3fFjjzLn6if5bdGJC").Result;

            // then
            Assert.Equal("RPWTabW8urd3xWv2To989v", result.Id);
            Assert.Equal("YJCgTf3jrXHkUVzLQ7y4eg", result.AccountId);
            Assert.Equal("EUR", result.Currency);
            Assert.Equal("Test Organization", result.PayoutInfo.Name);
            Assert.Equal("NL85ABNA0000000000", result.PayoutInfo.Account);
            Assert.Equal("Corporate account", result.PayoutInfo.Label);
            Assert.Equal("Netherlands", result.PayoutInfo.BankCountry);
            Assert.Equal("Test", result.PayoutInfo.Bank);
            Assert.Equal("RABONL2U", result.PayoutInfo.Swift);
            Assert.Equal("processing", result.Status);
            Assert.Equal(DateTime.Parse("2021-05-11T09:05:00.176Z").ToUniversalTime(), result.DateCreated);
            Assert.Equal(DateTime.Parse("2021-05-11T11:52:29.681Z").ToUniversalTime(), result.DateExecuted);
            Assert.Null(result.DateCompleted);
            Assert.Equal(DateTime.Parse("2021-05-10T09:00:00.000Z").ToUniversalTime(), result.OpeningDate);
            Assert.Equal(DateTime.Parse("2021-05-11T09:00:00.000Z").ToUniversalTime(), result.ClosingDate);
            Assert.Equal(23.27m, result.OpeningBalance);
            Assert.Equal(20.82m, result.LedgerEntriesSum);
            Assert.Equal(8.21m, result.WithHoldings?[0].Amount);
            Assert.Equal(8.21m, result.WithHoldingsSum);
            Assert.Equal(35.88m, result.TotalAmount);
            Assert.Null(result.LedgerEntries);
            Assert.Equal("2GrR6GDeYxUFYM9sDKViy6nFFTy4Rjvm1SYdLBjK46jkeJdgUTRccRfhtwkhNcuZky", result.Token);
        }

        [Fact]
        public void it_should_get_settlement_reconciliation_report()
        {
            // given
            var requestParameters = new Dictionary<string, dynamic>
            {
                {"token", "5T1T5yGDEtFDYe8jEVBSYLHKewPYXZrDLvZxtXBzn69fBbZYitYQYH4BFYFvvaVU7D"}
            };
            
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getSettlementReconciliationReportResponse.json"));
            _bitPayClient.Setup(b => b.Get(
                "settlements/RvNuCTMAkURKimwgvSVEMP/reconciliationReport",
                It.Is<Dictionary<string, dynamic?>>(d => requestParameters.SequenceEqual(d!)),
                true
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "settlements/RvNuCTMAkURKimwgvSVEMP/reconciliationReport")
            });
            
            // when
            var result = GetTestedClassAsMerchant().GetSettlementReconciliationReport(
                "RvNuCTMAkURKimwgvSVEMP", 
                "5T1T5yGDEtFDYe8jEVBSYLHKewPYXZrDLvZxtXBzn69fBbZYitYQYH4BFYFvvaVU7D").Result;

            // then
            Assert.Equal("RvNuCTMAkURKimwgvSVEMP", result.Id);
            Assert.Equal("YJCgTf3jrXHkUVzLQ7y4eg", result.AccountId);
            Assert.Equal("USD", result.Currency);
            Assert.Null(result.PayoutInfo.Name);
            Assert.Equal("NL85ABNA0000000000", result.PayoutInfo.Iban);
            Assert.Equal("Test", result.PayoutInfo.Label);
            Assert.Equal("Netherlands", result.PayoutInfo.BankCountry);
            Assert.Equal("Test", result.PayoutInfo.BankName);
            Assert.Equal("RABONL2U", result.PayoutInfo.Swift);
            Assert.Equal("processing", result.Status);
            Assert.Equal(DateTime.Parse("2018-08-23T20:45:22.742Z").ToUniversalTime(), result.DateCreated);
            Assert.Equal(DateTime.Parse("2018-08-23T20:47:06.912Z").ToUniversalTime(), result.DateExecuted);
            Assert.Null(result.DateCompleted);
            Assert.Equal(DateTime.Parse("2018-08-01T13:00:00.000Z").ToUniversalTime(), result.OpeningDate);
            Assert.Equal(DateTime.Parse("2018-08-23T13:00:00.000Z").ToUniversalTime(), result.ClosingDate);
            Assert.Equal(23.13m, result.OpeningBalance);
            Assert.Equal(2956.77m, result.LedgerEntriesSum);
            Assert.Equal(590.08m, result.WithHoldings?[0].Amount);
            Assert.Equal(590.08m, result.WithHoldingsSum);
            Assert.Equal(2389.82m, result.TotalAmount);
            Assert.Equal(42, result.LedgerEntries?.Count);
            Assert.Equal(1000, result.LedgerEntries?[0].Code);
            Assert.Equal("E1pJQNsHP2oHuMo2fagpe6", result.LedgerEntries?[0].InvoiceId);
            Assert.Equal(5.83M, result.LedgerEntries?[0].Amount);
            Assert.Equal(DateTime.Parse("2018-08-01T20:16:03.742Z").ToUniversalTime(), result.LedgerEntries?[0].Timestamp);
            Assert.Equal("Test invoice BCH", result.LedgerEntries?[0].Description);
            Assert.Equal("Test invoice BCH", result.LedgerEntries?[0].InvoiceData?.OrderId);
            Assert.Equal(5.0M, result.LedgerEntries?[0].InvoiceData?.Price);
            Assert.Equal("EUR", result.LedgerEntries?[0].InvoiceData?.Currency);
            Assert.Equal("BCH", result.LedgerEntries?[0].InvoiceData?.TransactionCurrency);
            Assert.Equal(100.0, result.LedgerEntries?[0].InvoiceData?.PayoutPercentage["USD"]);
        }
        
        [Fact]
        public void it_should_get_supported_wallets()
        {
            // given
            HttpContent response = new StringContent(File.ReadAllText(GetJsonResponsePath() + "getSupportedWallets.json"));
            _bitPayClient.Setup(b => b.Get(
                "supportedWallets",
                null,
                false
            )).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = response,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "supportedWallets")
            });
            
            // when
            var result = GetTestedClassAsMerchant().GetSupportedWallets().Result;

            // then
            Assert.Equal(7, result.Count);
            Assert.Equal("bitpay", result[0].Key);
            Assert.Equal("BitPay", result[0].DisplayName);
            Assert.True(result[0].PayPro);
            Assert.Equal("bitpay-wallet.png", result[0].Avatar);
            Assert.Equal("https://bitpay.com/img/wallet-logos/bitpay-wallet.png", result[0].Image);
            Assert.Equal(15, result[0].Currencies?.Count);
            Assert.Equal("BTC", result[0].Currencies?[0].Code);
            Assert.True(result[0].Currencies?[0].PayPro);
            Assert.Equal("BIP72b", result[0].Currencies?[0].Qr.Type);
            Assert.Equal("https://bitpay.com/img/icon/currencies/BTC.svg", result[0].Currencies?[0].Image);
        }
        

        private Bill GetBill() {
            List<string> cc = new List<string> {"jane@doe.com"};

            List<Item> items = new();
            Item item1 = new(price: 6.00M, quantity: 1) { Id = "Test Item 1" };
            Item item2 = new(price: 4.00M, quantity: 1) { Description = "Test Item 2" };
            items.Add(item1);
            items.Add(item2);

            Bill bill = new(
                number: "bill1234-ABCD",
                currency: "USD",
                email: "23242",
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
                Cc = cc,
                Phone = "555-123-456",
                DueDate = "2021-5-31",
                PassProcessingFee = true,
            };

            return bill;
        }
        
        private Invoice GetInvoiceExample()
        {
            var invoice = new Invoice(10.0M, "USD")
            {
                OrderId = ExampleGuid,
                FullNotifications = true,
                ExtendedNotifications = true,
                TransactionSpeed = "medium",
                NotificationUrl = "https://notification.url/aaa",
                ItemDesc = "Example",
                NotificationEmail = "m.warzybok@sumoheavy.com",
                AutoRedirect = true,
                ForcedBuyerSelectedWallet = "bitpay"
            };
            Buyer buyerData = new()
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
        
        private Payout GetPayoutExample()
        {
            Payout payout = new()
            {
                Amount = 10.00M,
                Currency = "USD",
                LedgerCurrency = "GBP",
                Reference = "payout_20210527",
                NotificationEmail = "merchant@email.com",
                NotificationUrl = "https://yournotiticationURL.com/wed3sa0wx1rz5bg0bv97851eqx",
                Email = "john@doe.com",
                Label = "John Doe"
            };

            return payout;
        }

        private PayoutRecipients GetPayoutRecipientsExample()
        {
            PayoutRecipient payoutRecipient1 = new("alice@email.com", "Alice");
            PayoutRecipient payoutRecipient2 = new("bob@email.com", "Bob");

            return new PayoutRecipients(new List<PayoutRecipient> { payoutRecipient1, payoutRecipient2 });
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
        
        private static string GetJsonResponsePath()
        {
            return GetBitPayUnitTestPath() + Path.DirectorySeparatorChar + "json" + Path.DirectorySeparatorChar;
        }

        private Client GetTestedClass()
        {
            return new Client(
                _bitPayClient.Object,
                Identity,
                _accessTokens.Object,
                _guidGenerator.Object
            );
        }
        
        private Client GetTestedClassAsMerchant()
        {
            _accessTokens.Setup(t => t.GetAccessToken(Facade.Merchant)).Returns(MerchantToken);
            _accessTokens.Setup(t => t.TokenExists(Facade.Merchant)).Returns(true);
            _guidGenerator.Setup(g => g.Execute()).Returns(ExampleGuid);
            
            return new Client(
                _bitPayClient.Object,
                Identity,
                _accessTokens.Object,
                _guidGenerator.Object
            );
        }
        
        private Client GetTestedClassAsPayout()
        {
            _accessTokens.Setup(t => t.GetAccessToken(Facade.Payout)).Returns(PayoutToken);
            _accessTokens.Setup(t => t.TokenExists(Facade.Payout)).Returns(true);
            _guidGenerator.Setup(g => g.Execute()).Returns(ExampleGuid);
            
            return new Client(
                _bitPayClient.Object,
                Identity,
                _accessTokens.Object,
                _guidGenerator.Object
            );
        }
    }
}