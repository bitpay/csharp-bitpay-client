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
        private readonly BitPayClient _bitPayClient;
        private readonly AccessTokens _accessTokens;

        public SettlementClient(BitPayClient bitPayClient, AccessTokens accessTokens)
        {
            _bitPayClient = bitPayClient ?? throw new MissingRequiredField("bitPayClient");
            _accessTokens = accessTokens ?? throw new MissingRequiredField("accessTokens");
        }
        
        /// <summary>
        ///     Retrieves a summary of the specified settlement.
        /// </summary>
        /// <param name="settlementId">Settlement Id</param>
        /// <returns>A BitPay Settlement object.</returns>
        /// <throws>SettlementQueryException SettlementQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Settlement> GetById(string settlementId)
        {
            if (settlementId == null) throw new MissingFieldException(nameof(settlementId));
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
            
                var response = await _bitPayClient.Get($"settlements/{settlementId}", parameters);
                var responseString = await _bitPayClient.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Settlement>(responseString);
            }
            catch (BitPayException ex)
            {
                throw new SettlementQueryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new SettlementQueryException(ex);

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
        ///     See https://bitpay.com/api/#rest-api-resources-settlements-retrieve-settlements
        /// </param>
        /// <returns>A list of BitPay Settlement objects</returns>
        /// <throws>SettlementQueryException SettlementQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<List<Settlement>> GetByFilters(Dictionary<string, dynamic> filters)
        {
            if (filters == null) throw new MissingFieldException(nameof(filters));
            try
            {
                filters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
               
                var response = await _bitPayClient.Get("settlements", filters);
                var responseString = await _bitPayClient.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<List<Settlement>>(responseString);
            }
            catch (BitPayException ex)
            {
                throw new SettlementQueryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new SettlementQueryException(ex);

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
        /// <throws>SettlementQueryException SettlementQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Settlement> GetSettlementReconciliationReport(string settlementId, string token)
        {
            if (settlementId == null) throw new MissingFieldException(nameof(settlementId));
            if (token == null) throw new MissingFieldException("token");
            
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", token);

                var response = await _bitPayClient.Get(
                    $"settlements/" + settlementId + "/reconciliationReport", parameters);
                var responseString = await _bitPayClient.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Settlement>(responseString);
            }
            catch (BitPayException ex)
            {
                throw new SettlementQueryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new SettlementQueryException(ex);

                throw;
            }
        }

    }
}