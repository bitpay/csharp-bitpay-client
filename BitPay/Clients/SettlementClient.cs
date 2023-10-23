// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BitPay.Exceptions;
using BitPay.Models.Settlement;

using Newtonsoft.Json;

namespace BitPay.Clients
{
    public class SettlementClient
    {
        private readonly IBitPayClient _bitPayClient;
        private readonly AccessTokens _accessTokens;

        public SettlementClient(IBitPayClient bitPayClient, AccessTokens accessTokens)
        {
            _bitPayClient = bitPayClient;
            _accessTokens = accessTokens;
        }
        
        /// <summary>
        ///     Retrieves a summary of the specified settlement.
        /// </summary>
        /// <param name="settlementId">Settlement Id</param>
        /// <returns>A BitPay Settlement object.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Settlement> GetById(string settlementId)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
            
            var response = await _bitPayClient.Get($"settlements/{settlementId}", parameters)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<Settlement>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Settlement", e.Message);
                throw;
            }
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
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<List<Settlement>> GetByFilters(Dictionary<string, dynamic?> filters)
        {
            filters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
               
            var response = await _bitPayClient.Get("settlements", filters).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<List<Settlement>>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Settlement", e.Message);
                throw;
            }
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
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Settlement> GetSettlementReconciliationReport(string settlementId, string token)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", token);

            var response = await _bitPayClient.Get("settlements/" + settlementId + "/reconciliationReport", parameters)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<Settlement>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Settlement", e.Message);
                throw;
            }
        }

    }
}