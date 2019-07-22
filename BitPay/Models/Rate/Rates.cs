using System.Collections.Generic;
using System.Threading.Tasks;

namespace BitPaySDK.Models.Rate
{
	/// <summary>
	///     Provides an interface to the BitPay server to obtain exchange rate information.
	/// </summary>
	public class Rates
    {
        private readonly BitPay _bp;
        private List<Rate> _rates;

        public Rates(List<Rate> rates, BitPay bp)
        {
            _bp = bp;
            _rates = rates;
        }

        public List<Rate> GetRates()
        {
            return _rates;
        }

        public async Task Update()
        {
            _rates = (await _bp.GetRates()).GetRates();
        }

        public decimal GetRate(string currencyCode)
        {
            decimal val = 0;
            foreach (var rateObj in _rates)
                if (rateObj.Code.Equals(currencyCode))
                {
                    val = rateObj.Value;
                    break;
                }

            return val;
        }
    }
}