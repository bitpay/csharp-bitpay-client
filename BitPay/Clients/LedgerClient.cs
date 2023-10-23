// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BitPay.Exceptions;
using BitPay.Models.Ledger;

using Newtonsoft.Json;

namespace BitPay.Clients
{
    public class LedgerClient
    {
        private readonly IBitPayClient _bitPayClient;
        private readonly AccessTokens _accessTokens;

        public LedgerClient(IBitPayClient bitPayClient, AccessTokens accessTokens)
        {
            _bitPayClient = bitPayClient;
            _accessTokens = accessTokens;
        }

        /// <summary>
        ///     Retrieve a list of ledgers entries by currency & date range using the merchant facade.
        /// </summary>
        /// <param name="currency">The three digit currency string for the ledger to retrieve.</param>
        /// <param name="dateStart">The start date for the query.</param>
        /// <param name="dateEnd">The end date for the query.</param>
        /// <returns>A list of Ledger entries.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<List<LedgerEntry>> GetLedgerEntries(string currency, DateTime dateStart, DateTime dateEnd)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
            parameters.Add("startDate", "" + dateStart.ToString("yyyy-MM-dd"));
            parameters.Add("endDate", "" + dateEnd.ToString("yyyy-MM-dd"));
            var response = await _bitPayClient.Get("ledgers/" + currency, parameters).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                var entries = JsonConvert.DeserializeObject<List<LedgerEntry>>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
                return entries;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Ledger", e.Message);

                throw;
            }
        }
        
        /// <summary>
        ///     Retrieve a list of ledgers available and its current balance using the merchant facade.
        /// </summary>
        /// <returns>A list of Ledger objects retrieved from the server.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<List<Ledger>> GetLedgers()
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
            var response = await _bitPayClient.Get("ledgers", parameters)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response)
                .ConfigureAwait(false);
            
            try
            {
                var ledgers = JsonConvert.DeserializeObject<List<Ledger>>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
                return ledgers;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Ledger", e.Message);

                throw;
            }
        }
    }
}