// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using BitPay.Exceptions;
using BitPay.Models;

using Newtonsoft.Json;

namespace BitPay.Clients
{
    public class CurrencyClient
    {
        private readonly IBitPayClient _bitPayClient;
        private List<Currency>? _currenciesInfo;

        public CurrencyClient(IBitPayClient bitPayClient)
        {
            _bitPayClient = bitPayClient;
        }
        
        /// <summary>
        ///  Get currency info.
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <returns>Currency</returns>
        /// <exception cref="BitPayGenericException"></exception>
        public async Task<Currency> GetCurrencyInfo(string? currencyCode)
        {
            if (currencyCode == null) BitPayExceptionProvider.ThrowMissingParameterException();

            _currenciesInfo ??= await LoadCurrencies().ConfigureAwait(false);

            foreach (var currency in _currenciesInfo.Where(currency => currency.Code == currencyCode))
            {
                return currency;
            }
            
            BitPayExceptionProvider.ThrowGenericExceptionWithMessage("Missing currency");
            throw new SyntaxErrorException();
        }

        private async Task<List<Currency>> LoadCurrencies()
        {
            var response = await _bitPayClient.Get("currencies", null, false)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response)
                .ConfigureAwait(false);

            return JsonConvert.DeserializeObject<List<Currency>>(responseString)!;
        }
    }
}