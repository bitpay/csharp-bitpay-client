// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BitPay.Exceptions;
using BitPay.Models.Payout;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitPay.Clients
{
    public class PayoutClient
    {
        private readonly IBitPayClient _bitPayClient;
        private readonly AccessTokens _accessTokens;

        public PayoutClient(IBitPayClient bitPayClient, AccessTokens accessTokens)
        {
            _bitPayClient = bitPayClient;
            _accessTokens = accessTokens;
        }

        /// <summary>
        ///     Submit a BitPay Payout.
        /// </summary>
        /// <param name="payout">A Payout object with request parameters defined.</param>
        /// <returns>A BitPay generated Payout object.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Payout> Submit(Payout payout)
        {
            payout.Token = _accessTokens.GetAccessToken(Facade.Payout);

            string json = null!;
            
            try
            {
                json = JsonConvert.SerializeObject(payout);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowSerializeResourceException("Payout", e.Message);
            }
            
            var response = await _bitPayClient.Post("payouts", json, true).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<Payout>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Payout", e.Message);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve a BitPay payout by payout id using.  The client must have been previously authorized for the 
        ///     payout facade.
        /// </summary>
        /// <param name="payoutId">The id of the payout to retrieve.</param>
        /// <returns>A BitPay generated Payout object.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Payout> Get(string payoutId)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));

            var response = await _bitPayClient.Get("payouts/" + payoutId, parameters)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response)
                .ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<Payout>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
            
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Payout", e.Message);

                throw;
            }
        }

        /// <summary>
        ///     Cancel a BitPay Payout.
        /// </summary>
        /// <param name="payoutId">The id of the payout to cancel.</param>
        /// <returns>True if payout was successfully canceled, false otherwise.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<bool> Cancel(string payoutId)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));

            var response = await _bitPayClient.Delete("payouts/" + payoutId, parameters)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                JObject responseObject = JsonConvert.DeserializeObject<dynamic>(responseString)!;
                return "success".Equals(
                    responseObject.GetValue("status", StringComparison.Ordinal)?.ToString(),
                    StringComparison.OrdinalIgnoreCase
                ) ;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Payout", e.Message);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve a collection of BitPay payouts.
        /// </summary>
        /// <param name="filters">
        /// Filters available: startDate, endDate, status, reference, limit, offset.
        /// See https://bitpay.readme.io/reference/retrieve-payouts-filtered-by-query
        /// </param>
        /// <returns>A list of BitPay Payout objects.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<List<Payout>> GetPayouts(Dictionary<string, dynamic?> filters)
        {
            filters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));

            var response = await _bitPayClient.Get("payouts", filters).ConfigureAwait(false); 
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false); 
            
            try
            {
                return JsonConvert.DeserializeObject<List<Payout>>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Payout", e.Message);

                throw;
            }
        }

        /// <summary>
        ///     Send a payout notification    
        /// </summary>
        /// <param name="payoutId">The id of the payout to notify.</param>
        /// <returns>True if the notification was successfully sent, false otherwise.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<bool> RequestPayoutNotification(string payoutId)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));

            string json = null!;
            
            try
            {
                json = JsonConvert.SerializeObject(parameters);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowSerializeResourceException("Payout", e.Message);

                throw;
            }
            
            var response = await _bitPayClient.Post("payouts/" + payoutId + "/notifications", json, true)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                JObject responseObject = JsonConvert.DeserializeObject<dynamic>(responseString)!;
                return "success".Equals(
                    responseObject.GetValue("status", StringComparison.Ordinal)?.ToString(),
                    StringComparison.OrdinalIgnoreCase
                    );
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Payout", e.Message);

                throw;
            }
        }
    }
}