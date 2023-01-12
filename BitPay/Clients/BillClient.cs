using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BitPay.Exceptions;
using BitPay.Models.Bill;
using Newtonsoft.Json;

namespace BitPay.Clients
{
    public class BillClient
    {
        private readonly BitPayClient _bitPayClient;
        private readonly AccessTokens _accessTokens;


        public BillClient(BitPayClient bitPayClient, AccessTokens accessTokens)
        {
            _bitPayClient = bitPayClient ?? throw new MissingRequiredField(nameof(bitPayClient));
            _accessTokens = accessTokens ?? throw new MissingRequiredField(nameof(accessTokens));
        }

        /// <summary>
        ///     Create a bill.
        /// </summary>
        /// <param name="bill">An invoice request object.</param>
        /// <param name="facade">The facade to create the invoice against</param>
        /// <param name="signRequest">Allow unsigned request</param>
        /// <returns>A new bill object returned from the server.</returns
        /// <throws>BillCreationException BillCreationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Bill> CreateBill(Bill bill, string facade = Facade.Merchant, bool signRequest = true)
        {
            try
            {
                bill.Token = this._accessTokens.GetAccessToken(facade);
                var json = JsonConvert.SerializeObject(bill);
                var response = await _bitPayClient.Post("bills", json, signRequest).ConfigureAwait(false);
                var responseString = await _bitPayClient.ResponseToJsonString(response).ConfigureAwait(false);
                var serializerSettings = new JsonSerializerSettings
                    {ObjectCreationHandling = ObjectCreationHandling.Replace};
                JsonConvert.PopulateObject(responseString, bill, serializerSettings);
            }
            catch (BitPayException ex)
            {
                throw new BillCreationException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BillCreationException(ex);

                throw;
            }

            return bill;
        }
        
        /// <summary>
        ///     Retrieve a bill by id.
        /// </summary>
        /// <param name="billId">The id of the requested bill.</param>
        /// <param name="facade">The facade to get the bill from</param>
        /// <param name="signRequest">Allow unsigned request</param>
        /// <returns>The bill object retrieved from the server.</returns>
        /// <throws>BillQueryException BillQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Bill> GetBill(string billId, string facade = Facade.Merchant, bool signRequest = true)
        {
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

                var response = await _bitPayClient.Get("bills/" + billId, parameters, signRequest);
                var responseString = await _bitPayClient.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Bill>(responseString);
            }
            catch (BitPayException ex)
            {
                throw new BillQueryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BillQueryException(ex);

                throw;
            }
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
            try
            {
                // Provide the merchant token when the merchant facade is being used.
                // GET/invoices expects the merchant token and not the merchant/invoice token.
                Dictionary<string, dynamic> parameters = ResourceClientUtil.InitParams();
                try
                {
                    parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
                    if (!String.IsNullOrEmpty(status))
                    {
                        parameters.Add("status", status);
                    }
                }
                catch (BitPayException)
                {
                    // No token for invoice.
                    parameters = null;
                }

                var response = await _bitPayClient.Get("bills", parameters);
                var responseString = await _bitPayClient.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<List<Bill>>(responseString);
            }
            catch (BitPayException ex)
            {
                throw new BillQueryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BillQueryException(ex);

                throw;
            }
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
            try
            {
                var json = JsonConvert.SerializeObject(bill);
                var response = await _bitPayClient.Put("bills/" + billId, json).ConfigureAwait(false);
                var responseString = await _bitPayClient.ResponseToJsonString(response).ConfigureAwait(false);
                var serializerSettings = new JsonSerializerSettings
                    {ObjectCreationHandling = ObjectCreationHandling.Replace};
                JsonConvert.PopulateObject(responseString, bill, serializerSettings);
            }
            catch (BitPayException ex)
            {
                throw new BillUpdateException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BillUpdateException(ex);

                throw;
            }

            return bill;
        }

        /// <summary>
        ///     Deliver a bill to the consumer.
        /// </summary>
        /// <param name="billId">The id of the requested bill.</param>
        /// <param name="billToken">The token of the requested bill.</param>
        /// <param name="signRequest">Allow unsigned request</param>
        /// <returns>A response status returned from the API.</returns>
        /// <throws>BillDeliveryException BillDeliveryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<string> DeliverBill(string billId, string billToken, bool signRequest = true)
        {
            var responseString = "";
            try
            {
                var json = JsonConvert.SerializeObject(new Dictionary<string, string> {{"token", billToken}});
                var response = await _bitPayClient.Post("bills/" + billId + "/deliveries", json, signRequest).ConfigureAwait(false);
                responseString = await _bitPayClient.ResponseToJsonString(response).ConfigureAwait(false);
            }
            catch (BitPayException ex)
            {
                throw new BillDeliveryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BillDeliveryException(ex);

                throw;
            }

            return responseString;
        }
    }
}