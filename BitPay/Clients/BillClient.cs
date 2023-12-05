// Copyright (c) 2019 BitPay.
// All rights reserved.

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
        private readonly IBitPayClient _bitPayClient;
        private readonly AccessTokens _accessTokens;


        public BillClient(IBitPayClient bitPayClient, AccessTokens accessTokens)
        {
            _bitPayClient = bitPayClient;
            _accessTokens = accessTokens;
        }

        /// <summary>
        ///     Create a bill.
        /// </summary>
        /// <param name="bill">An invoice request object.</param>
        /// <param name="facade">The facade to create the invoice against</param>
        /// <param name="signRequest">Allow unsigned request</param>
        /// <returns>A new bill object returned from the server.</returns>
        /// <throws>BitPayGenericException BitPayGenericException class</throws>
        /// <throws>BitPayApiException BitPayApiException class</throws>
        public async Task<Bill> CreateBill(Bill bill, string facade = Facade.Merchant, bool signRequest = true)
        {
            if (bill == null)
            {
                BitPayExceptionProvider.ThrowMissingParameterException();
                throw new InvalidOperationException();
            }
            
            bill.Token = _accessTokens.GetAccessToken(facade);

            string json;
            
            try
            {
                json = JsonConvert.SerializeObject(bill);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowSerializeResourceException("Bill", e.Message);
                throw;
            }
            
            var response = await _bitPayClient.Post("bills", json, signRequest).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                var serializerSettings = new JsonSerializerSettings
                    {ObjectCreationHandling = ObjectCreationHandling.Replace};
                JsonConvert.PopulateObject(responseString, bill, serializerSettings);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Bill", e.Message);
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
        /// <throws>BitPayGenericException BitPayGenericException class</throws>
        /// <throws>BitPayApiException BitPayApiException class</throws>
        public async Task<Bill> GetBill(string billId, string facade = Facade.Merchant, bool signRequest = true)
        {
            Dictionary<string, dynamic?>? parameters = null;
            
            parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(facade));
            
            var response = await _bitPayClient.Get("bills/" + billId, parameters, signRequest)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<Bill>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Bill", e.Message);

                throw;
            }
        }
        
        /// <summary>
        ///     Retrieve a bill by id.
        /// </summary>
        /// <param name="status">The status to filter the bills.</param>
        /// <returns>A list of bill objects.</returns>
        /// <throws>BitPayGenericException BitPayGenericException class</throws>
        /// <throws>BitPayApiException BitPayApiException class</throws>
        public async Task<List<Bill>> GetBills(string? status = null)
        {
            Dictionary<string, dynamic?>? parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
            if (!String.IsNullOrEmpty(status))
            {
                parameters.Add("status", status!);
            }
            
            var response = await _bitPayClient.Get("bills", parameters).ConfigureAwait(false);
            var responseString =await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);

            List<Bill> bills = null!;
            
            try
            {
                bills = JsonConvert.DeserializeObject<List<Bill>>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Bill", e.Message);
            }

            return bills;
        }

        /// <summary>
        ///     Update a bill.
        /// </summary>
        /// <param name="bill">An invoice object containing the update.</param>
        /// <param name="billId">The id of the bill to update.</param>
        /// <returns>A new bill object returned from the server.</returns>
        /// <throws>BitPayGenericException BitPayGenericException class</throws>
        /// <throws>BitPayApiException BitPayApiException class</throws>
        public async Task<Bill> UpdateBill(Bill bill, string billId)
        {
            if (bill == null)
            {
                BitPayExceptionProvider.ThrowMissingParameterException();
                throw new InvalidOperationException();
            }

            string json = null!;
            
            try
            {
                json = JsonConvert.SerializeObject(bill);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowSerializeResourceException("Bill", e.Message);
            }
            
            var response = await _bitPayClient.Put("bills/" + billId, json).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);

            try
            {
                var serializerSettings = new JsonSerializerSettings
                    {ObjectCreationHandling = ObjectCreationHandling.Replace};
                JsonConvert.PopulateObject(responseString, bill, serializerSettings);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowSerializeResourceException("Bill", e.Message);
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
        /// <throws>BitPayGenericException BitPayGenericException class</throws>
        /// <throws>BitPayApiException BitPayApiException class</throws>
        public async Task<string> DeliverBill(string billId, string billToken, bool signRequest = true)
        {
            string json = null!;
            
            try
            {
                json = JsonConvert.SerializeObject(new Dictionary<string, string> {{"token", billToken}});
                
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowEncodeException(e.Message);
            }
            
            var response = await _bitPayClient.Post("bills/" + billId + "/deliveries", json, signRequest).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);

            return responseString;
        }
    }
}