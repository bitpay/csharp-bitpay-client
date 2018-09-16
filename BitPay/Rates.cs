using System.Collections.Generic;

namespace BitPayAPI
{
    /// <summary>
    /// Provides an interface to the BitPay server to obtain exchange rate information.
    /// </summary>
    public class Rates
    {
        private BitPay _bp;
        private List<Rate> _rates;

        public Rates(List<Rate> rates, BitPay bp)
        {
            _bp = bp;
            _rates = rates;
        }

	    public List<Rate> getRates()
        {
		    return _rates;
	    }

	    public void update()
        {
		    _rates = _bp.getRates().getRates();
	    }

        public decimal getRate(string currencyCode)
        {
		    decimal val = 0;
		    foreach (Rate rateObj in _rates)
            {
			    if (rateObj.Code.Equals(currencyCode))
                {
                    val = rateObj.Value;
                    break;
			    }
		    }
    		return val;
	    }
    }
}
