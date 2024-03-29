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
            _bitPayClient = bitPayClient ?? throw new MissingRequiredField(nameof(bitPayClient));
            _accessTokens = accessTokens ?? throw new MissingRequiredField(nameof(accessTokens));
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
            if (currency == null) throw new MissingFieldException(nameof(currency));

            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
                parameters.Add("startDate", "" + dateStart.ToString("yyyy-MM-dd"));
                parameters.Add("endDate", "" + dateEnd.ToString("yyyy-MM-dd"));
                var response = await _bitPayClient.Get("ledgers/" + currency, parameters).ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                var entries = JsonConvert.DeserializeObject<List<LedgerEntry>>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
                return entries;
            }
            catch (BitPayException ex)
            {
                throw new LedgerQueryException(ex, ex.ApiCode);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new LedgerQueryException(ex);

                throw;
            }
        }
        
        /// <summary>
        ///     Retrieve a list of ledgers available and its current balance using the merchant facade.
        /// </summary>
        /// <returns>A list of Ledger objects retrieved from the server.</returns>
        /// <throws>LedgerQueryException LedgerQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<List<Ledger>> GetLedgers()
        {
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
                var response = await _bitPayClient.Get("ledgers", parameters)
                    .ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response)
                    .ConfigureAwait(false);
                var ledgers = JsonConvert.DeserializeObject<List<Ledger>>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
                return ledgers;
            }
            catch (BitPayException ex)
            {
                throw new LedgerQueryException(ex, ex.ApiCode);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new LedgerQueryException(ex);

                throw;
            }
        }
    }
}