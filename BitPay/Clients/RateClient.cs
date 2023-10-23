// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BitPay.Exceptions;
using BitPay.Models.Rate;

using Newtonsoft.Json;

namespace BitPay.Clients
{
    public class RateClient
    {
        private readonly IBitPayClient _bitPayClient;

        public RateClient(IBitPayClient bitPayClient)
        {
            _bitPayClient = bitPayClient;
        }
        
        /// <summary>
        ///     Retrieve the rates for a cryptocurrency / fiat pair. See https://bitpay.com/bitcoin-exchange-rates.
        /// </summary>
        /// <param name="baseCurrency">
        ///     The cryptocurrency for which you want to fetch the rates. Current supported values are BTC and BCH.
        /// </param>
        /// <param name="currency">The fiat currency for which you want to fetch the baseCurrency rates.</param>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Rate> GetRate(string baseCurrency, string currency)
        {
            var response = await _bitPayClient.Get("rates/" + baseCurrency + "/" + currency, signatureRequired: false)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<Rate>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Rate", e.Message);

                throw;
            }
        }
        
        /// <summary>
        ///     Retrieve the exchange rate table using the public facade.
        /// </summary>
        /// <returns>The rate table as an object retrieved from the server.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Rates> GetRates()
        {
            var response = await _bitPayClient.Get("rates", signatureRequired: false).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response)
                .ConfigureAwait(false);

            List<Rate> rates;
            
            try
            {
                rates = JsonConvert.DeserializeObject<List<Rate>>(responseString)!;
                
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Rate", e.Message);

                throw;
            }
            
            return new Rates(rates);
        }
        
        /// <summary>
        ///     Retrieve the exchange rate table using the public facade.
        /// </summary>
        /// <param name="currency">The fiat currency for which you want to fetch the baseCurrency rates.</param>
        /// <returns>The rate table as an object retrieved from the server.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Rates> GetRates(string currency)
        {
            var response = await _bitPayClient.Get("rates/" + currency, signatureRequired: false)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            List<Rate> rates;
            
            try
            {
                rates = JsonConvert.DeserializeObject<List<Rate>>(responseString)!;
                
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Rate", e.Message);

                throw;
            }
            
            return new Rates(rates);
        }
    }
}