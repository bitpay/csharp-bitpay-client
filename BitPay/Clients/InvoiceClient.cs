// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BitPay.Exceptions;
using BitPay.Models.Invoice;
using BitPay.Utils;

using Newtonsoft.Json;

namespace BitPay.Clients
{
    public class InvoiceClient
    {
        private readonly IBitPayClient _bitPayClient;
        private readonly AccessTokens _accessTokens;
        private readonly IGuidGenerator _guidGenerator;

        public InvoiceClient(IBitPayClient bitPayClient, AccessTokens accessTokens, IGuidGenerator guidGenerator)
        {
            _bitPayClient = bitPayClient ?? throw new MissingFieldException(nameof(bitPayClient));
            _accessTokens = accessTokens ?? throw new MissingFieldException(nameof(accessTokens));
            _guidGenerator = guidGenerator ?? throw new MissingFieldException(nameof(guidGenerator));
        }

        /// <summary>
        ///     Create an invoice using the specified facade.
        /// </summary>
        /// <param name="invoice">An invoice request object.</param>
        /// <param name="facade">The facade to create the invoice against</param>
        /// <param name="signRequest">Allow unsigned request</param>
        /// <returns>A new invoice object returned from the server.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Invoice> CreateInvoice(Invoice invoice, string facade = Facade.Merchant,
            bool signRequest = true)
        {
            invoice.Token = _accessTokens.GetAccessToken(facade);
            invoice.ResourceGuid ??= _guidGenerator.Execute();

            string json = null!;
            
            try
            {
                json = JsonConvert.SerializeObject(invoice);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowSerializeResourceException("Bill", e.Message);
            }

            var response = await _bitPayClient.Post("invoices", json, signRequest).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);

            try
            {
                JsonConvert.PopulateObject(responseString, invoice);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Bill", e.Message);
            }

            return invoice;
        }

        /// <summary>
        ///     Retrieve an invoice by id and token.
        /// </summary>
        /// <param name="invoiceId">The id of the requested invoice.</param>
        /// <param name="facade">The facade to get the invoice from</param>
        /// <param name="signRequest">Add token to request parameters</param>
        /// <returns>The invoice object retrieved from the server.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Invoice> GetInvoice(string invoiceId, string facade = Facade.Merchant,
            bool signRequest = true)
        {
            Dictionary<string, dynamic?>? parameters = null;
            
            parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(facade));
            
            var response = await _bitPayClient.Get("invoices/" + invoiceId, parameters, signRequest)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);

            Invoice invoice = null!;
            
            try
            {
                invoice = JsonConvert.DeserializeObject<Invoice>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Invoice", e.Message);
            }

            return invoice;
        }

        /// <summary>
        ///     Retrieve an invoice by guid.
        /// </summary>
        /// <param name="invoiceGuid">The guid of the requested invoice.</param>
        /// <param name="facade">The facade to get the invoice from</param>
        /// <param name="signRequest">Add token to request parameters</param>
        /// <returns>The invoice object retrieved from the server.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Invoice> GetInvoiceByGuid(string invoiceGuid, string facade = Facade.Merchant,
            bool signRequest = true)
        {
            Dictionary<string, dynamic?>? parameters = null;
            parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(facade));
            
            var response = await _bitPayClient.Get("invoices/guid/" + invoiceGuid, parameters, signRequest)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);

            Invoice invoice = null!;
            
            try
            {
                invoice = JsonConvert.DeserializeObject<Invoice>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Invoice", e.Message);
            }

            return invoice;
        }

        /// <summary>
        ///     Retrieve a list of invoices by date range using the merchant facade.
        /// </summary>
        /// <param name="dateStart">The start date for the query. Using format YYYY-MM-DD.</param>
        /// <param name="dateEnd">The end date for the query. Using format YYYY-MM-DD.</param>
        /// <param name="parameters">Available parameters: orderId, limit, offset</param>
        /// <returns>A list of invoice objects retrieved from the server.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<List<Invoice>> GetInvoices(DateTime dateStart, DateTime dateEnd,
            Dictionary<string, dynamic?>? parameters = null)
        {
            if (parameters == null)
            {
                parameters = ResourceClientUtil.InitParams();
            }
            
            // UTC date, ISO-8601 format yyyy-mm-dd.
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
            parameters.Add("dateStart", dateStart.ToString("yyyy-MM-dd"));
            parameters.Add("dateEnd", dateEnd.ToString("yyyy-MM-dd"));
            
            var response = await _bitPayClient.Get("invoices", parameters).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);

            List<Invoice> invoices = null!;
            
            try
            {
                invoices = JsonConvert.DeserializeObject<List<Invoice>>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Invoice", e.Message);
            }

            return invoices;
        }

        /// <summary>
        ///     Update a BitPay invoice.
        /// </summary>
        /// <param name="invoiceId">The id of the invoice to updated.</param>
        /// <param name="parameters">Available parameters: buyerEmail, buyerSms, smsCode, autoVerify</param>
        /// <returns>A BitPay updated Invoice object.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Invoice> UpdateInvoice(string invoiceId, Dictionary<string, dynamic?> parameters)
        {
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));

            string json = null!;
            
            try
            {
                json = JsonConvert.SerializeObject(parameters);
               
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowEncodeException(e.Message);
            }
            
            var response = await _bitPayClient.Put("invoices/" + invoiceId, json).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);

            Invoice invoice = null!;
            
            try
            {
                invoice = JsonConvert.DeserializeObject<Invoice>(responseString)!;
               
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Invoice", e.Message);
            }

            return invoice;
        }

        /// <summary>
        ///     Cancel a BitPay invoice.
        /// </summary>
        /// <param name="invoiceId">The id of the invoice to cancel.</param>
        /// <param name="forceCancel">
        /// Parameter that will cancel the invoice even if no contact information is present.
        /// Note: Canceling a paid invoice without contact information requires
        /// a manual support process and is not recommended.
        /// </param>
        /// <returns>Cancelled invoice object.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Invoice> CancelInvoice(string invoiceId, bool forceCancel = true)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
            
            if (forceCancel)
            {
                parameters.Add("forceCancel", true);
            }
            
            var response = await _bitPayClient.Delete("invoices/" + invoiceId, parameters).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);

            Invoice invoice = null!;
            
            try
            {
                invoice = JsonConvert.DeserializeObject<Invoice>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Invoice", e.Message);
            }

            return invoice;
        }

        /// <summary>
        ///     Cancel a BitPay invoice.
        /// </summary>
        /// <param name="guidId">The GUID of the invoice to cancel.</param>
        /// <param name="forceCancel">
        /// Parameter that will cancel the invoice even if no contact information is present.
        /// Note: Canceling a paid invoice without contact information requires
        /// a manual support process and is not recommended.
        /// </param>
        /// <returns>Cancelled invoice object.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Invoice> CancelInvoiceByGuid(string guidId, bool forceCancel = true)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
            if (forceCancel)
            {
                parameters.Add("forceCancel", true);
            }
            
            var response = await _bitPayClient.Delete("invoices/guid/" + guidId, parameters)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);

            Invoice invoice = null!;
            
            try
            {
                invoice = JsonConvert.DeserializeObject<Invoice>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Invoice", e.Message);
            }

            return invoice;
        }

        /// <summary>
        ///     Request a webhook to be resent.
        /// </summary>
        /// <param name="invoiceId">Invoice ID.</param>
        /// <returns>Status of request</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<bool> RequestInvoiceWebhookToBeResent(string invoiceId)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken((Facade.Merchant)));

            string json = null!;
            
            try
            {
                json = JsonConvert.SerializeObject(parameters);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowSerializeParamsException(e.Message);
            }

            var response = await _bitPayClient.Post("invoices/" + invoiceId + "/notifications", json)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            return responseString == "Success";
        }

        /// <summary>
        ///     Retrieves a bus token which can be used to subscribe to invoice events.
        /// </summary>
        /// <param name="invoiceId">the id of the invoice for which you want to fetch an event token</param>
        /// <returns>event token</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<InvoiceEventToken> GetInvoiceEventToken(string invoiceId)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));

            var response = await _bitPayClient.Get("invoices/" + invoiceId + "/events", parameters)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);

            InvoiceEventToken invoiceEventToken = null!;

            try
            {
                invoiceEventToken = JsonConvert.DeserializeObject<InvoiceEventToken>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Invoice Event Token", e.Message);
            }

            return invoiceEventToken;
        }

        /// <summary>
        ///     Pay a BitPay invoice with a mock transaction.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="status"></param>
        /// <returns>Invoice</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Invoice> PayInvoice(string invoiceId, string status)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
            parameters.Add("status", status);

            string json = null!;
            
            try
            {
                json = JsonConvert.SerializeObject(parameters);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowSerializeParamsException(e.Message);
            }

            var response = await _bitPayClient.Put("invoices/pay/" + invoiceId, json).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);

            Invoice invoice = null!;
            
            try
            {
                invoice = JsonConvert.DeserializeObject<Invoice>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Invoice Event Token", e.Message);
            }

            return invoice;
        }
    }
}