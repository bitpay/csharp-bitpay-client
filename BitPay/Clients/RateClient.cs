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
            _bitPayClient = bitPayClient ?? throw new MissingRequiredField("bitPayClient");
        }
        
        /// <summary>
        ///     Retrieve the rates for a cryptocurrency / fiat pair. See https://bitpay.com/bitcoin-exchange-rates.
        /// </summary>
        /// <param name="baseCurrency">
        ///     The cryptocurrency for which you want to fetch the rates. Current supported values are BTC and BCH.
        /// </param>
        /// <param name="currency">The fiat currency for which you want to fetch the baseCurrency rates.</param>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Rate> GetRate(string baseCurrency, string currency)
        {
            if (baseCurrency == null) throw new MissingFieldException(nameof(baseCurrency));
            if (currency == null) throw new MissingFieldException(nameof(currency));
            
            try
            {
                var response = await _bitPayClient.Get("rates/" + baseCurrency + "/" + currency, signatureRequired: false)
                    .ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<Rate>(responseString);
            }
            catch (BitPayException ex)
            {
                throw new RatesQueryException(ex, ex.ApiCode);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new RatesQueryException(ex);

                throw;
            }
        }
        
        /// <summary>
        ///     Retrieve the exchange rate table using the public facade.
        /// </summary>
        /// <returns>The rate table as an object retrieved from the server.</returns>
        /// <throws>RatesQueryException RatesQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Rates> GetRates()
        {
            try
            {
                var response = await _bitPayClient.Get("rates", signatureRequired: false).ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response)
                    .ConfigureAwait(false);
                var rates = JsonConvert.DeserializeObject<List<Rate>>(responseString);
                return new Rates(rates);
            }
            catch (BitPayException ex)
            {
                throw new RatesQueryException(ex, ex.ApiCode);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new RatesQueryException(ex);

                throw;
            }
        }
        
        /// <summary>
        ///     Retrieve the exchange rate table using the public facade.
        /// </summary>
        /// <param name="currency">The fiat currency for which you want to fetch the baseCurrency rates.</param>
        /// <returns>The rate table as an object retrieved from the server.</returns>
        /// <throws>RatesQueryException RatesQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Rates> GetRates(string currency)
        {
            try
            {
                var response = await _bitPayClient.Get("rates/" + currency, signatureRequired: false)
                    .ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                var rates = JsonConvert.DeserializeObject<List<Rate>>(responseString);
                return new Rates(rates);
            }
            catch (BitPayException ex)
            {
                throw new RatesQueryException(ex, ex.ApiCode);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new RatesQueryException(ex);

                throw;
            }
        }
    }
}