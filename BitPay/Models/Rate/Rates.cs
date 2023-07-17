// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BitPay.Clients;

namespace BitPay.Models.Rate
{
	/// <summary>
	///     Provides an interface to the BitPay server to obtain exchange rate information.
	/// </summary>
	public class Rates
    {
        private List<Rate> _rates;

        public Rates(List<Rate> rates)
        {
            _rates = rates;
        }

        public List<Rate> GetRates()
        {
            return _rates;
        }

        public async Task Update(RateClient rateClient)
        {
            if (rateClient == null)
            {
                throw new ArgumentNullException(nameof(rateClient));
            }

            _rates = (await rateClient.GetRates().ConfigureAwait(false)).GetRates();
        }

        public decimal GetRate(string currencyCode)
        {
            return (from rateObj in _rates where string.Equals(rateObj.Code, currencyCode, StringComparison.Ordinal) select rateObj.Value).FirstOrDefault();
        }
    }
}