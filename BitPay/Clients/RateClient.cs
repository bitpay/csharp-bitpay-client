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
        private readonly BitPayClient _bitPayClient;

        public RateClient(BitPayClient bitPayClient)
        {
            _bitPayClient = bitPayClient ?? throw new MissingRequiredField("bitPayClient");
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
                var response = await _bitPayClient.Get("rates", signatureRequired: false);
                var responseString = await _bitPayClient.ResponseToJsonString(response);
                var rates = JsonConvert.DeserializeObject<List<Rate>>(responseString);
                return new Rates(rates);
            }
            catch (BitPayException ex)
            {
                throw new RatesQueryException(ex, ex.GetApiCode());
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