using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BitPay.Exceptions;
using BitPay.Models;
using BitPay.Utils;
using Newtonsoft.Json;

namespace BitPay.Clients
{
    public class CurrencyClient
    {
        private readonly IBitPayClient _bitPayClient;
        private List<Currency> _currenciesInfo;

        public CurrencyClient(IBitPayClient bitPayClient)
        {
            _bitPayClient = bitPayClient ?? throw new MissingRequiredField("bitPayClient");
        }

        public async Task<Currency> GetCurrencyInfo(string currencyCode)
        {
            if (currencyCode == null) throw new MissingFieldException(nameof(currencyCode));

            if (_currenciesInfo == null)
            {
                _currenciesInfo = await LoadCurrencies();
            }

            foreach (var currency in _currenciesInfo)
            {
                if (currency.Code == currencyCode)
                {
                    return currency;
                }
            }

            throw new BitPayException(null, "missing currency");
        }

        private async Task<List<Currency>> LoadCurrencies()
        {
            var response = await _bitPayClient.Get("currencies", null, false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response);

            return JsonConvert.DeserializeObject<List<Currency>>(responseString);
        }
    }
}