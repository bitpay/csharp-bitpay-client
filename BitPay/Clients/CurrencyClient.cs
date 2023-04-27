// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
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

        public async Task<Currency> GetCurrencyInfo(string currencyCode)
        {
            if (currencyCode == null) throw new MissingFieldException(nameof(currencyCode));

            _currenciesInfo ??= await LoadCurrencies().ConfigureAwait(false);

            foreach (var currency in _currenciesInfo.Where(currency => currency.Code == currencyCode))
            {
                return currency;
            }

            throw new BitPayException("missing currency");
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