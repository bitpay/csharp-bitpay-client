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
        private readonly BitPayClient _bitPayClient;
        private readonly AccessTokens _accessTokens;
        private readonly GuidGenerator _guidGenerator;

        public InvoiceClient(BitPayClient bitPayClient, AccessTokens accessTokens, GuidGenerator guidGenerator)
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
        /// <param name="guid">GUID</param>
        /// <returns>A new invoice object returned from the server.</returns>
        /// <throws>InvoiceCreationException InvoiceCreationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Invoice> CreateInvoice(Invoice invoice, string facade = Facade.Merchant,
            bool signRequest = true, string guid = null)
        {
            try
            {
                invoice.Token = _accessTokens.GetAccessToken(facade);
                invoice.Guid = guid ?? _guidGenerator.Execute();
                var json = JsonConvert.SerializeObject(invoice);
                var response = await _bitPayClient.Post("invoices", json, signRequest).ConfigureAwait(false);
                var responseString = await _bitPayClient.ResponseToJsonString(response).ConfigureAwait(false);
                JsonConvert.PopulateObject(responseString, invoice);
            }
            catch (BitPayException ex)
            {
                throw new InvoiceCreationException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new InvoiceCreationException(ex);

                throw;
            }

            // Track the token for this invoice
            // CacheToken(invoice.Id, invoice.Token);

            return invoice;
        }

        /// <summary>
        ///     Retrieve an invoice by id and token.
        /// </summary>
        /// <param name="invoiceId">The id of the requested invoice.</param>
        /// <param name="facade">The facade to get the invoice from</param>
        /// <returns>The invoice object retrieved from the server.</returns>
        /// <throws>InvoiceQueryException InvoiceQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Invoice> GetInvoice(string invoiceId, string facade = Facade.Merchant,
            bool signRequest = true)
        {
            if (invoiceId == null) throw new ArgumentNullException(nameof(invoiceId));

            Dictionary<string, dynamic> parameters = null;
            try
            {
                if (signRequest)
                {
                    // Provide the merchant token when the merchant facade is being used.
                    // GET/invoices expects the merchant token and not the merchant/invoice token.
                    try
                    {
                        parameters = ResourceClientUtil.InitParams();
                        parameters.Add("token", _accessTokens.GetAccessToken(facade));
                    }
                    catch (BitPayException)
                    {
                        // No token for invoice.
                        parameters = null;
                    }
                }

                var response = await _bitPayClient.Get("invoices/" + invoiceId, parameters, signRequest);
                var responseString = await _bitPayClient.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Invoice>(responseString);
            }
            catch (BitPayException ex)
            {
                throw new InvoiceQueryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new InvoiceQueryException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve an invoice by guid.
        /// </summary>
        /// <param name="guid">The guid of the requested invoice.</param>
        /// <param name="facade">The facade to get the invoice from</param>
        /// <returns>The invoice object retrieved from the server.</returns>
        /// <throws>InvoiceQueryException InvoiceQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Invoice> GetInvoiceByGuid(string guid, string facade = Facade.Merchant,
            bool signRequest = true)
        {
            if (guid == null) throw new ArgumentNullException(nameof(guid));

            Dictionary<string, dynamic> parameters = null;
            try
            {
                if (signRequest)
                {
                    // Provide the merchant token when the merchant facade is being used.
                    // GET/invoices expects the merchant token and not the merchant/invoice token.
                    try
                    {
                        parameters = ResourceClientUtil.InitParams();
                        parameters.Add("token", _accessTokens.GetAccessToken(facade));
                    }
                    catch (BitPayException)
                    {
                        // No token for invoice.
                        parameters = null;
                    }
                }

                var response = await _bitPayClient.Get("invoices/guid/" + guid, parameters, signRequest);
                var responseString = await _bitPayClient.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Invoice>(responseString);
            }
            catch (BitPayException ex)
            {
                throw new InvoiceQueryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new InvoiceQueryException(ex);

                throw;
            }
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
            parameters ??= ResourceClientUtil.InitParams();
            try
            {
                // UTC date, ISO-8601 format yyyy-mm-dd.
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
                parameters.Add("dateStart", dateStart.ToString("yyyy-MM-dd"));
                parameters.Add("dateEnd", dateEnd.ToString("yyyy-MM-dd"));

                var response = await _bitPayClient.Get("invoices", parameters);
                var responseString = await _bitPayClient.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<List<Invoice>>(responseString);
            }
            catch (BitPayException ex)
            {
                throw new InvoiceQueryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new InvoiceQueryException(ex);

                throw;
            }
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
            if (invoiceId == null) throw new ArgumentNullException(nameof(invoiceId));

            try
            {
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));

                var json = JsonConvert.SerializeObject(parameters);
                var response = await _bitPayClient.Put("invoices/" + invoiceId, json).ConfigureAwait(false);
                var responseString = await _bitPayClient.ResponseToJsonString(response).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<Invoice>(responseString);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new InvoiceUpdateException(ex);

                throw;
            }
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
        /// <throws>InvoiceCancellationException InvoiceCancellationException class</throws>
        public async Task<Invoice> CancelInvoice(string invoiceId, Boolean forceCancel = true)
        {
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
                if (forceCancel)
                {
                    parameters.Add("forceCancel", true);
                }

                var response = await _bitPayClient.Delete("invoices/" + invoiceId, parameters);
                var responseString = await _bitPayClient.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Invoice>(responseString);
            }
            catch (BitPayException ex)
            {
                throw new InvoiceQueryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new InvoiceCancellationException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Request a webhook to be resent.
        /// </summary>
        /// <param name="invoiceId">Invoice ID.</param>
        /// <returns>Status of request</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> RequestInvoiceWebhookToBeResent(string invoiceId)
        {
            if (invoiceId == null) throw new ArgumentNullException(nameof(invoiceId));

            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken((Facade.Merchant)));
            var json = JsonConvert.SerializeObject(parameters);

            var response = await _bitPayClient.Post("invoices/" + invoiceId + "/notifications", json);
            var responseString = await _bitPayClient.ResponseToJsonString(response).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<bool>(responseString);
        }

        /// <summary>
        ///     Retrieves a bus token which can be used to subscribe to invoice events.
        /// </summary>
        /// <param name="invoiceId">the id of the invoice for which you want to fetch an event token</param>
        /// <returns>event token</returns>
        /// <exception cref="BitPayException"></exception>
        public async Task<InvoiceEventToken> GetInvoiceEventToken(string invoiceId)
        {
            if (invoiceId == null) throw new BitPayException("missing mandatory fields");

            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
            var json = JsonConvert.SerializeObject(parameters);

            var response = await _bitPayClient.Post("invoices/" + invoiceId + "/events", json);
            var responseString = await _bitPayClient.ResponseToJsonString(response).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<InvoiceEventToken>(responseString);
        }
    }
}