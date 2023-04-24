using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BitPay.Clients;
using BitPay.Exceptions;
using BitPay.Models;
using BitPay.Models.Bill;
using BitPay.Models.Invoice;
using BitPay.Models.Ledger;
using BitPay.Models.Payout;
using BitPay.Models.Rate;
using BitPay.Models.Settlement;
using BitPay.Models.Wallet;
using BitPay.Utils;
using Microsoft.Extensions.Configuration;

namespace BitPay
{
    public class Client
    {
        private IBitPayClient _bitPayClient;
        private AccessTokens _accessTokens;
        private IGuidGenerator _guidGenerator;
        private string _identity;

        /// <summary>
        ///     Create Client class for POS.
        /// </summary>
        /// <param name="token">Pos token VO.</param>
        public Client(PosToken token)
        {
            InitPosClient(token, Environment.Prod);
        }

        /// <summary>
        ///     Create Client class for POS.
        /// </summary>
        /// <param name="token">Pos token VO.</param>
        /// <param name="environment">Environment</param>
        public Client(PosToken token, Environment environment)
        {
            InitPosClient(token, environment);
        }

        /// <summary>
        ///     Create Client class for Merchant/Payout.
        /// </summary>
        /// <param name="environment">Environment</param>
        /// <param name="privateKey"></param>
        /// <param name="accessTokens"></param>
        public Client(PrivateKey privateKey, AccessTokens accessTokens, Environment environment = Environment.Prod)
        {
            var ecKey = GetEcKey(privateKey);
            var baseUrl = GetBaseUrl(environment);
            var httpClient = getHttpClient(baseUrl);

            _accessTokens = accessTokens;
            _bitPayClient = new BitPayClient(httpClient, baseUrl, ecKey);
            _guidGenerator = new UuidGenerator();
            CreateIdentity(ecKey);
        }

        /// <summary>
        ///     Create Client class for Merchant/Payout by config file path.
        /// </summary>
        /// <param name="configFilePath">Config File Path</param>
        /// <param name="environment">Environment</param>
        public Client(ConfigFilePath configFilePath, Environment environment = Environment.Prod)
        {
            IConfiguration config = BuildConfigFromFile(configFilePath);
            _accessTokens = new AccessTokens(config);

            var ecKey = GetEcKey(config, environment.ToString());
            var baseUrl = GetBaseUrl(environment);
            var httpClient = getHttpClient(baseUrl);

            _bitPayClient = new BitPayClient(httpClient, baseUrl, ecKey);
            _guidGenerator = new UuidGenerator();
            CreateIdentity(ecKey);
        }

        /// <summary>
        ///     Create Client class with all dependencies.
        /// </summary>
        /// <param name="bitPayClient">BitPayClient</param>
        /// <param name="identity">Identity</param>
        /// <param name="accessTokens">AccessTokens</param>
        /// <param name="guidGenerator">GuidGenerator</param>
        public Client(IBitPayClient bitPayClient, string identity, AccessTokens accessTokens,
            IGuidGenerator guidGenerator)
        {
            _bitPayClient = bitPayClient;
            _accessTokens = accessTokens;
            _identity = identity;
            _guidGenerator = guidGenerator;
        }

        /// <summary>
        ///     Get access token (eg. merchant etc.)
        /// </summary>
        /// <param name="facade"></param>
        /// <returns></returns>
        public string GetAccessToken(string facade)
        {
            return _accessTokens.GetAccessToken(facade);
        }

        /// <summary>
        ///     Get specific info about currency.
        /// </summary>
        /// <param name="currencyCode">Currency code</param>
        /// <returns>Currency</returns>
        public async Task<Currency> GetCurrencyInfo(string currencyCode)
        {
            return await CreateCurrencyClient().GetCurrencyInfo(currencyCode);
        }

        /// <summary>
        ///     Authorize (pair) this client with the server using the specified pairing code.
        /// </summary>
        /// <param name="pairingCode">A code obtained from the server; typically from bitpay.com/api-tokens.</param>
        public Task AuthorizeClient(string pairingCode)
        {
            return CreateAuthorizationClient().AuthorizeClient(pairingCode);
        }

        /// <summary>
        ///     Request authorization (a token) for this client in the specified facade.
        /// </summary>
        /// <param name="facade">The facade for which authorization is requested.</param>
        /// <returns>A pairing code for this client. This code must be used to authorize this client at BitPay.com/api-tokens.</returns>
        /// <throws>ClientAuthorizationException ClientAuthorizationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<string> CreatePairingCodeForFacade(string facade)
        {
            return await CreateAuthorizationClient().CreatePairingCodeForFacade(facade);
        }

        /// <summary>
        ///     Create an invoice using the specified facade.
        /// </summary>
        /// <param name="invoice">An invoice request object.</param>
        /// <param name="guid">GUID</param>
        /// <returns>A new invoice object returned from the server.</returns>
        /// <throws>InvoiceCreationException InvoiceCreationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Invoice> CreateInvoice(Invoice invoice, string guid = null)
        {
            var invoiceClient = CreateInvoiceClient();
            var facade = GetFacadeBasedOnAccessToken();
            var signRequest = IsSignRequestFacade(facade);

            return await invoiceClient.CreateInvoice(invoice, facade, signRequest, guid);
        }

        /// <summary>
        ///     Retrieve an invoice by id and token.
        /// </summary>
        /// <param name="invoiceId">The id of the requested invoice.</param>
        /// <returns>The invoice object retrieved from the server.</returns>
        /// <throws>InvoiceQueryException InvoiceQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Invoice> GetInvoice(string invoiceId)
        {
            var facade = GetFacadeBasedOnAccessToken();
            var signRequest = IsSignRequestFacade(facade);

            return await CreateInvoiceClient().GetInvoice(invoiceId, facade, signRequest);
        }

        /// <summary>
        ///     Retrieve an invoice by guid.
        /// </summary>
        /// <param name="guid">The guid of the requested invoice.</param>
        /// <returns>The invoice object retrieved from the server.</returns>
        /// <throws>InvoiceQueryException InvoiceQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Invoice> GetInvoiceByGuid(string guid)
        {
            var facade = GetFacadeBasedOnAccessToken();
            var signRequest = IsSignRequestFacade(facade);

            return await CreateInvoiceClient().GetInvoiceByGuid(guid, facade, signRequest);
        }

        /// <summary>
        ///     Retrieve a list of invoices by date range using the merchant facade.
        /// </summary>
        /// <param name="dateStart">The start date for the query. Using format YYYY-MM-DD.</param>
        /// <param name="dateEnd">The end date for the query. Using format YYYY-MM-DD.</param>
        /// <param name="parameters">Available parameters: orderId, limit, offset</param>
        /// <returns>A list of invoice objects retrieved from the server.</returns>
        /// <throws>InvoiceQueryException InvoiceQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<List<Invoice>> GetInvoices(DateTime dateStart, DateTime dateEnd,
            Dictionary<string, dynamic> parameters = null)
        {
            return await CreateInvoiceClient().GetInvoices(dateStart, dateEnd, parameters);
        }

        /// <summary>
        ///     Update a BitPay invoice.
        /// </summary>
        /// <param name="invoiceId">The id of the invoice to updated.</param>
        /// <param name="parameters">Available parameters: buyerEmail, buyerSms, smsCode, autoVerify</param>
        /// <returns>A BitPay updated Invoice object.</returns>
        /// <throws>InvoiceUpdateException InvoiceUpdateException class</throws>
        public async Task<Invoice> UpdateInvoice(string invoiceId, Dictionary<string, dynamic> parameters)
        {
            return await CreateInvoiceClient().UpdateInvoice(invoiceId, parameters);
        }

        /// <summary>
        ///     Cancel a BitPay invoice.
        /// </summary>
        /// <param name="invoiceId">The id of the invoice to cancel.</param>
        /// Parameter that will cancel the invoice even if no contact information is present.
        /// Note: Canceling a paid invoice without contact information requires
        /// a manual support process and is not recommended.
        /// <returns>Cancelled invoice object.</returns>
        /// <throws>InvoiceCancellationException InvoiceCancellationException class</throws>
        public async Task<Invoice> CancelInvoice(string invoiceId)
        {
            return await CreateInvoiceClient().CancelInvoice(invoiceId);
        }
        
        /// <summary>
        ///     Cancel a BitPay invoice.
        /// </summary>
        /// <param name="guid">The GUID of the invoice to cancel.</param>
        /// Parameter that will cancel the invoice even if no contact information is present.
        /// Note: Canceling a paid invoice without contact information requires
        /// a manual support process and is not recommended.
        /// <returns>Cancelled invoice object.</returns>
        /// <throws>InvoiceCancellationException InvoiceCancellationException class</throws>
        public async Task<Invoice> CancelInvoiceByGuid(string guid)
        {
            return await CreateInvoiceClient().CancelInvoiceByGuid(guid);
        }

        /// <summary>
        ///     Request a webhook to be resent.
        /// </summary>
        /// <param name="invoiceId">Invoice ID.</param>
        /// <returns>Status of request</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Boolean> RequestInvoiceWebhookToBeResent(string invoiceId)
        {
            return await CreateInvoiceClient().RequestInvoiceWebhookToBeResent(invoiceId);
        }
        
        /// <summary>
        ///     Pay invoice.
        /// </summary>
        /// <param name="invoiceId">Invoice ID.</param>
        /// <param name="status">Status></param>
        public async Task<Invoice> PayInvoice(string invoiceId, string status = "complete")
        {
            return await CreateInvoiceClient().PayInvoice(invoiceId, status);
        }

        /// <summary>
        ///     Retrieves a bus token which can be used to subscribe to invoice events.
        /// </summary>
        /// <param name="invoiceId">the id of the invoice for which you want to fetch an event token</param>
        /// <returns>event token</returns>
        /// <exception cref="BitPayException"></exception>
        public async Task<InvoiceEventToken> GetInvoiceEventToken(string invoiceId)
        {
            return await CreateInvoiceClient().GetInvoiceEventToken(invoiceId);
        }

        ///  <summary>
        ///      Create a refund for a BitPay invoice.
        ///  </summary>
        ///  <param name="refundToCreate">
        ///     Available params: amount, invoiceId, token, preview, immediate, buyerPaysRefundFee, reference.
        ///     See https://bitpay.readme.io/reference/create-a-refund-request
        /// </param>
        ///  <returns>An updated Refund Object</returns> 
        /// <throws>RefundCreationException RefundCreationException class</throws> 
        /// <throws>BitPayException BitPayException class</throws> 
        public async Task<Refund> CreateRefund(Refund refundToCreate)
        {
            return await CreateRefundClient().Create(refundToCreate);
        }

        /// <summary>
        ///     Retrieve a previously made refund request on a BitPay invoice.
        /// </summary>
        ///<param name="refundId">The BitPay refund ID.</param>
        ///<returns>A BitPay Refund object with the associated Refund object.</returns> 
        ///<throws>RefundQueryException RefundQueryException class</throws> 
        public async Task<Refund> GetRefund(string refundId)
        {
            return await CreateRefundClient().GetById(refundId);
        }

        /// <summary>
        ///     Retrieve a previously made refund request on a BitPay invoice.
        /// </summary>
        ///<param name="guid"></param>The BitPay guid.
        ///<returns>A BitPay Refund object with the associated Refund object.</returns> 
        ///<throws>RefundQueryException RefundQueryException class</throws> 
        public async Task<Refund> GetRefundByGuid(string guid)
        {
            return await CreateRefundClient().GetByGuid(guid);
        }

        /// <summary>
        ///     Retrieve all refund requests on a BitPay invoice.
        /// </summary>
        /// <param name="invoiceId">The id of the requested invoice.</param>
        /// <returns>A BitPay invoice object with the associated Refund objects updated.</returns>
        /// <throws>RefundQueryException RefundQueryException class</throws>
        public async Task<List<Refund>> GetRefunds(string invoiceId)
        {
            return await CreateRefundClient().GetByInvoiceId(invoiceId);
        }

        /// <summary>
        ///     Update the status of a BitPay invoice.
        /// </summary>
        ///<param name="refundId">A BitPay refund ID</param> .
        ///<param name="status">The new status for the refund to be updated.</param>   
        /// <returns>A BitPay generated Refund object.</returns>
        ///<throws>RefundUpdateException class</throws> 
        public async Task<Refund> UpdateRefund(string refundId, string status)
        {
            return await CreateRefundClient().Update(refundId, status);
        }

        /// <summary>
        ///     Update the status of a BitPay invoice.
        /// </summary>
        ///<param name="guid">A BitPay Guid.</param>
        ///<param name="status">The new status for the refund to be updated.</param>   
        /// <returns>A BitPay generated Refund object.</returns>
        ///<throws>RefundUpdateException class</throws> 
        public async Task<Refund> UpdateRefundByGuid(string guid, string status)
        {
            return await CreateRefundClient().UpdateByGuid(guid, status);
        }

        /// <summary>
        ///     Send a refund notification
        /// </summary>
        /// <param name="refundId">A BitPay refundId </param>
        /// <returns>An updated Refund Object </returns>
        /// <throws>RefundCreationException RefundCreationException class </throws>
        /// <throws>BitPayException BitPayException class </throws>
        public async Task<Boolean> SendRefundNotification(string refundId)
        {
            return await CreateRefundClient().SendRefundNotification(refundId);
        }

        /// <summary>
        ///     Cancel a previously submitted refund request.
        /// </summary>
        /// <param name="refundId">The ID of the refund request for which you want to cancel.</param>
        /// <returns>Refund object.</returns>
        /// <throws>RefundCancellationException RefundCancellationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Refund> CancelRefund(string refundId)
        {
            return await CreateRefundClient().Cancel(refundId);
        }

        /// <summary>
        ///     Cancel a previously submitted refund request.
        /// </summary>
        /// <param name="guid">The GUID of the refund request for which you want to cancel.</param>
        /// <returns>Refund object.</returns>
        /// <throws>RefundCancellationException RefundCancellationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Refund> CancelRefundByGuid(string guid)
        {
            return await CreateRefundClient().CancelByGuid(guid);
        }

        /// <summary>
        ///     Create a bill.
        /// </summary>
        /// <param name="bill">An invoice request object.</param>
        /// <returns>A new bill object returned from the server.</returns
        /// <throws>BillCreationException BillCreationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Bill> CreateBill(Bill bill)
        {
            var facade = GetFacadeBasedOnAccessToken();
            var signRequest = IsSignRequestFacade(facade);

            return await CreateBillClient().CreateBill(bill, facade, signRequest);
        }

        /// <summary>
        ///     Retrieve a bill by id.
        /// </summary>
        /// <param name="billId">The id of the requested bill.</param>
        /// <returns>The bill object retrieved from the server.</returns>
        /// <throws>BillQueryException BillQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Bill> GetBill(string billId)
        {
            var facade = GetFacadeBasedOnAccessToken();
            var signRequest = IsSignRequestFacade(facade);

            return await CreateBillClient().GetBill(billId, facade, signRequest);
        }

        /// <summary>
        ///     Retrieve a bill by id.
        /// </summary>
        /// <param name="status">The status to filter the bills.</param>
        /// <returns>A list of bill objects.</returns>
        /// <throws>BillQueryException BillQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<List<Bill>> GetBills(string status = null)
        {
            return await CreateBillClient().GetBills(status);
        }

        /// <summary>
        ///     Update a bill.
        /// </summary>
        /// <param name="bill">An invoice object containing the update.</param>
        /// <param name="billId">The id of the bill to update.</param>
        /// <returns>A new bill object returned from the server.</returns>
        /// <throws>BillUpdateException BillUpdateException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Bill> UpdateBill(Bill bill, string billId)
        {
            return await CreateBillClient().UpdateBill(bill, billId);
        }

        /// <summary>
        ///     Deliver a bill to the consumer.
        /// </summary>
        /// <param name="billId">The id of the requested bill.</param>
        /// <param name="billToken">The token of the requested bill.</param>
        /// <returns>A response status returned from the API.</returns>
        /// <throws>BillDeliveryException BillDeliveryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<string> DeliverBill(string billId, string billToken)
        {
            return await CreateBillClient().DeliverBill(billId, billToken);
        }
        
        /// <summary>
        ///     Retrieve the rates for a cryptocurrency / fiat pair. See https://bitpay.com/bitcoin-exchange-rates.
        /// </summary>
        /// <param name="baseCurrency">
        ///     The cryptocurrency for which you want to fetch the rates. Current supported values are BTC and BCH.
        /// </param>
        /// <param name="currency">The fiat currency for which you want to fetch the baseCurrency rates.</param>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Rate> GetRate(string baseCurrency, string currency)
        {
            return await CreateRateClient().GetRate(baseCurrency, currency);
        }

        /// <summary>
        ///     Retrieve the exchange rate table using the public facade.
        /// </summary>
        /// <returns>The rate table as an object retrieved from the server.</returns>
        /// <throws>RatesQueryException RatesQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Rates> GetRates()
        {
            return await CreateRateClient().GetRates();
        }
        
        /// <summary>
        ///     Retrieve the exchange rate table using the public facade.
        /// </summary>
        /// <param name="currency">The fiat currency for which you want to fetch the baseCurrency rates.</param>
        /// <returns>The rate table as an object retrieved from the server.</returns>
        /// <throws>RatesQueryException RatesQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Rates> GetRates(string currency)
        {
            return await CreateRateClient().GetRates(currency);
        }

        /// <summary>
        ///     Retrieve a list of ledgers entries by currency & date range using the merchant facade.
        /// </summary>
        /// <param name="currency">The three digit currency string for the ledger to retrieve.</param>
        /// <param name="dateStart">The start date for the query.</param>
        /// <param name="dateEnd">The end date for the query.</param>
        /// <returns>A list of Ledger entries.</returns>
        /// <throws>LedgerQueryException LedgerQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<List<LedgerEntry>> GetLedgerEntries(string currency, DateTime dateStart, DateTime dateEnd)
        {
            return await CreateLedgerClient().GetLedgerEntries(currency, dateStart, dateEnd);
        }

        /// <summary>
        ///     Retrieve a list of ledgers available and its current balance using the merchant facade.
        /// </summary>
        /// <returns>A list of Ledger objects retrieved from the server.</returns>
        /// <throws>LedgerQueryException LedgerQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<List<Ledger>> GetLedgers()
        {
            return await CreateLedgerClient().GetLedgers();
        }

        /// <summary>
        ///     Submit BitPay Payout Recipients.
        /// </summary>
        /// <param name="recipients">A PayoutRecipients object with request parameters defined.</param>
        /// <returns>A list of BitPay PayoutRecipients objects.</returns>
        /// <throws>PayoutRecipientCreationException PayoutRecipientCreationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<List<PayoutRecipient>> SubmitPayoutRecipients(PayoutRecipients recipients)
        {
            return await CreatePayoutRecipientClient().Submit(recipients);
        }

        /// <summary>
        ///     Retrieve a BitPay payout recipient by recipient id using.
        ///     The client must have been previously authorized for the payout facade.
        /// </summary>
        /// <param name="recipientId">The id of the recipient to retrieve.</param>
        /// <returns>A BitPay PayoutRecipient object.</returns>
        /// <throws>PayoutRecipientQueryException PayoutRecipientQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<PayoutRecipient> GetPayoutRecipient(string recipientId)
        {
            return await CreatePayoutRecipientClient().Get(recipientId);
        }

        /// <summary>
        ///     Retrieve a collection of BitPay Payout Recipients.
        /// </summary>
        /// <param name="status">The recipient status you want to query on.</param>
        /// <param name="limit">Maximum results that the query will return (useful for paging results).</param>
        /// <param name="offset">Offset for paging</param>
        /// <returns>A list of BitPayRecipient objects.</returns>
        /// <throws>PayoutRecipientQueryException PayoutRecipientQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<List<PayoutRecipient>> GetPayoutRecipients(string status = null, int limit = 100,
            int offset = 0)
        {
            return await CreatePayoutRecipientClient().GetByFilters(status, limit, offset);
        }
        
        /// <summary>
        ///     Update a Payout Recipient.
        /// </summary>
        /// <param name="recipientId">The recipient id for the recipient to be updated.</param>
        /// <param name="recipient">A PayoutRecipient object with updated parameters defined.
        ///     Available changes: label, token
        /// </param>
        /// <returns>The updated recipient object.</returns>
        /// <throws>PayoutRecipientUpdateException PayoutRecipientUpdateException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<PayoutRecipient> UpdatePayoutRecipient(string recipientId, PayoutRecipient recipient)
        {
            return await CreatePayoutRecipientClient().Update(recipientId, recipient);
        }

        /// <summary>
        ///     Cancel a BitPay Payout recipient.
        /// </summary>
        /// <param name="recipientId">The id of the recipient to cancel.</param>
        /// <returns>True if the delete operation was successfully, false otherwise.</returns>
        /// <throws>PayoutRecipientCancellationException PayoutRecipientCancellationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Boolean> DeletePayoutRecipient(string recipientId)
        {
            return await CreatePayoutRecipientClient().Delete(recipientId);
        }

        /// <summary>
        ///     Send a payout recipient notification
        /// </summary>
        /// <param name="recipientId">The id of the recipient to notify.</param>
        /// <returns>True if the notification was successfully sent, false otherwise.</returns>
        /// <throws>PayoutRecipientNotificationException PayoutRecipientNotificationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Boolean> RequestPayoutRecipientNotification(string recipientId)
        {
            return await CreatePayoutRecipientClient().RequestPayoutRecipientNotification(recipientId);
        }

        /// <summary>
        ///     Submit a BitPay Payout.
        /// </summary>
        /// <param name="payout">A Payout object with request parameters defined.</param>
        /// <returns>A BitPay generated Payout object.</returns>
        /// <throws>PayoutCreationException PayoutCreationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Payout> SubmitPayout(Payout payout)
        {
            return await CreatePayoutClient().Submit(payout);
        }

        /// <summary>
        ///     Retrieve a BitPay payout by payout id using.  The client must have been previously authorized for the 
        ///     payout facade.
        /// </summary>
        /// <param name="payoutId">The id of the payout to retrieve.</param>
        /// <returns>A BitPay generated Payout object.</returns>
        /// <throws>PayoutQueryException PayoutQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Payout> GetPayout(string payoutId)
        {
            return await CreatePayoutClient().Get(payoutId);
        }

        /// <summary>
        ///     Cancel a BitPay Payout.
        /// </summary>
        /// <param name="payoutId">The id of the payout to cancel.</param>
        /// <returns>True if payout was successfully canceled, false otherwise.</returns>
        /// <throws>PayoutCancellationException PayoutCancellationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Boolean> CancelPayout(string payoutId)
        {
            return await CreatePayoutClient().Cancel(payoutId);
        }

        /// <summary>
        ///     Retrieve a collection of BitPay payouts.
        /// </summary>
        /// <param name="filters">
        /// Filters available: startDate, endDate, status, reference, limit, offset.
        /// See https://bitpay.readme.io/reference/retrieve-payouts-filtered-by-query
        /// </param>
        /// <returns>A list of BitPay Payout objects.</returns>
        /// <throws>PayoutQueryException PayoutQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<List<Payout>> GetPayouts(Dictionary<string, dynamic> filters)
        {
            return await CreatePayoutClient().GetPayouts(filters);
        }

        /// <summary>
        ///     Send a payout notification    
        /// </summary>
        /// <param name="payoutId">The id of the payout to notify.</param>
        /// <returns>True if the notification was successfully sent, false otherwise.</returns>
        /// <throws>PayoutNotificationException PayoutNotificationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Boolean> RequestPayoutNotification(string payoutId)
        {
            return await CreatePayoutClient().RequestPayoutNotification(payoutId);
        }

        /// <summary>
        ///     Retrieves a summary of the specified settlement.
        /// </summary>
        /// <param name="settlementId">Settlement Id</param>
        /// <returns>A BitPay Settlement object.</returns>
        /// <throws>SettlementQueryException SettlementQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Settlement> GetSettlement(string settlementId)
        {
            return await CreateSettlementClient().GetById(settlementId);
        }

        /// <summary>
        ///     Retrieves settlement reports for the calling merchant filtered by query. The `limit` and `offset` parameters
        ///     specify pages for large query sets.
        /// </summary>
        /// <param name="filters">
        ///     Available filters: startDate (Format YYYY-MM-DD), endDate (Format YYYY-MM-DD), status, currency,
        ///     limit, offset.
        ///     See https://bitpay.readme.io/reference/retrieve-settlements
        /// </param>
        /// <returns>A list of BitPay Settlement objects</returns>
        /// <throws>SettlementQueryException SettlementQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<List<Settlement>> GetSettlements(Dictionary<string, dynamic> filters)
        {
            return await CreateSettlementClient().GetByFilters(filters);
        }

        /// <summary>
        ///     Gets a detailed reconciliation report of the activity within the settlement period
        /// </summary>
        /// <param name="settlementId">id of the specific settlement resource to be fetched</param>
        /// <param name="token">
        ///     When fetching the reconciliation report for a specific settlement,
        ///     use the API token linked to the corresponding settlement resource.
        ///     This token can be retrieved via the endpoint GET https://bitpay.com/settlements/:settlementId.
        /// </param>
        /// <returns>A detailed BitPay Settlement object.</returns>
        /// <throws>SettlementQueryException SettlementQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Settlement> GetSettlementReconciliationReport(string settlementId, string token)
        {
            return await CreateSettlementClient().GetSettlementReconciliationReport(settlementId, token);
        }
        
        /// <summary>
        ///     Retrieve all supported wallets.
        /// </summary>
        ///<returns>A list of wallet objets.</returns> 
        ///<throws>WalletQueryException WalletQueryException class</throws> 
        public async Task<List<Wallet>> GetSupportedWallets()
        {
            return await CreateWalletClient().GetSupportedWallets();
        }


        private bool IsSignRequestFacade(string facade)
        {
            return facade != Facade.Pos;
        }

        private string GetFacadeBasedOnAccessToken()
        {
            return _accessTokens.TokenExists(Facade.Merchant) ? Facade.Merchant : Facade.Pos;
        }

        private AuthorizationClient CreateAuthorizationClient()
        {
            return new AuthorizationClient(_bitPayClient, _guidGenerator, _accessTokens, _identity);
        }

        private InvoiceClient CreateInvoiceClient()
        {
            return new InvoiceClient(_bitPayClient, _accessTokens, _guidGenerator);
        }

        private RefundClient CreateRefundClient()
        {
            return new RefundClient(_bitPayClient, _accessTokens, _guidGenerator);
        }

        private BillClient CreateBillClient()
        {
            return new BillClient(_bitPayClient, _accessTokens);
        }

        private RateClient CreateRateClient()
        {
            return new RateClient(_bitPayClient);
        }

        private LedgerClient CreateLedgerClient()
        {
            return new LedgerClient(_bitPayClient, _accessTokens);
        }

        private PayoutRecipientsClient CreatePayoutRecipientClient()
        {
            return new PayoutRecipientsClient(_bitPayClient, _accessTokens, _guidGenerator);
        }

        private PayoutClient CreatePayoutClient()
        {
            return new PayoutClient(_bitPayClient, _accessTokens);
        }

        private SettlementClient CreateSettlementClient()
        {
            return new SettlementClient(_bitPayClient, _accessTokens);
        }

        private WalletClient CreateWalletClient()
        {
            return new WalletClient(_bitPayClient);
        }
        
        private CurrencyClient CreateCurrencyClient()
        {
            return new CurrencyClient(_bitPayClient);
        }

        private IConfiguration BuildConfigFromFile(ConfigFilePath configFilePath)
        {
            if (!File.Exists(configFilePath.Value()))
            {
                throw new Exception("Configuration file not found");
            }

            var builder = new ConfigurationBuilder().AddJsonFile(configFilePath.Value(), false, true);
            return builder.Build();
        }

        /// <summary>
        ///     Gets ECKey.
        /// </summary>
        /// <param name="privateKey">PrivateKey</param>
        /// <exception cref="BitPayException"></exception>
        /// <returns>EcKey</returns>
        private EcKey GetEcKey(PrivateKey privateKey)
        {
            if (File.Exists(privateKey.Value()) && KeyUtils.PrivateKeyExists(privateKey.Value()))
            {
                return KeyUtils.LoadEcKey();
            }
            else
            {
                try
                {
                    return KeyUtils.CreateEcKeyFromString(privateKey.Value());
                }
                catch (Exception)
                {
                    throw new BitPayException( "Private Key file not found OR invalid key provided");
                }
            }
        }

        private void CreateIdentity(EcKey ecKey)
        {
            _identity = KeyUtils.DeriveSin(ecKey);
        }

        private void InitPosClient(PosToken token, Environment environment)
        {
            var baseUrl = GetBaseUrl(environment);
            var httpClient = getHttpClient(baseUrl);

            _accessTokens = new AccessTokens();
            _accessTokens.AddPos(token.Value());
            _bitPayClient = new BitPayClient(httpClient, baseUrl, null);
            _guidGenerator = new UuidGenerator();
        }

        private static HttpClient getHttpClient(string baseUrl)
        {
            return new HttpClient() {BaseAddress = new Uri(baseUrl)};
        }

        private static string GetBaseUrl(Environment environment)
        {
            return environment == Environment.Test ? Config.TestUrl : Config.ProdUrl;
        }
        
        private EcKey GetEcKey(IConfiguration config, string env)
        {
            if (KeyUtils.PrivateKeyExists(config
                .GetSection("BitPayConfiguration:EnvConfig:" + env + ":PrivateKeyPath").Value))
            {
                return KeyUtils.LoadEcKey();
            }
            
            return KeyUtils.CreateEcKeyFromString(config
                .GetSection("BitPayConfiguration:EnvConfig:" + env + ":PrivateKey").Value);
        }
    }
}