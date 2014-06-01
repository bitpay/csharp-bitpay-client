using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Helpers;

namespace BitPayAPI
{
    /// <summary>
    /// Provides an interface to the BitPay server to obtain exchange rate information.
    /// </summary>
    public class Rates
    {
        private BitPay _bp;
        private List<Rate> _rates;

        /// <summary>
        /// Constructor.  Creates the Rates instance from the BitPay server response.
        /// </summary>
        /// <param name="response">The raw HTTP response from BitPay server api/rates call.</param>
        /// <param name="bp">bp - used to update self.</param>
        public Rates(HttpContent response, BitPay bp)
        {
            dynamic obj = Json.Decode(response.ReadAsStringAsync().Result);
            this._bp = bp;

            _rates = new List<Rate>();
            foreach (dynamic rateObj in obj)
            {
                _rates.Add(new Rate(rateObj.name, rateObj.code, rateObj.rate));
            }
        }

        /// <summary>
        /// Bitcoin exchange rates in a list.
        /// </summary>
        /// <returns>A list of Rate objects.</returns>
	    public List<Rate> getRates()
        {
		    return _rates;
	    }

	    /// <summary>
        /// Updates the exchange rates from the BitPay API.
	    /// </summary>
	    public void update()
        {
		    _rates = _bp.getRates().getRates();
	    }

        /// <summary>
        /// Returns the Bitcoin exchange rate for the given currency code.
        /// Ensure that the currency code is valid, and in all caps.
        /// </summary>
        /// <param name="currencyCode">Three letter currency code in all caps.</param>
        /// <returns>The exchange rate.</returns>
        public decimal getRate(string currencyCode)
        {
		    decimal val = 0;
		    foreach (Rate rateObj in _rates)
            {
			    if (rateObj.getCode().Equals(currencyCode))
                {
                    val = rateObj.getRate();
                    break;
			    }
		    }
    		return val;
	    }
    }
}
