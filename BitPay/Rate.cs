using System;

namespace BitPayAPI
{
    /// <summary>
    /// Provides an interface to a single exchange rate.
    /// </summary>
    public class Rate
    {
        string _name;
        string _code;
        decimal _rate;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The full display name of the currency.</param>
        /// <param name="code">The three letter code for the currency, in all caps.</param>
        /// <param name="rate">The numeric exchange rate of this currency provided by the BitPay server.</param>
        public Rate(string name, string code, decimal rate)
        {
            _name = name;
            _code = code;
            _rate = rate;
        }

        /// <summary>
        /// The full display name of the currency.
        /// </summary>
        /// <returns></returns>
        public string getName()
        {
            return _name;
        }

        /// <summary>
        /// The three letter code for the currency, in all caps.
        /// </summary>
        /// <returns></returns>
        public string getCode()
        {
            return _code;
        }

        /// <summary>
        /// The numeric exchange rate of this currency provided by the BitPay server.
        /// </summary>
        /// <returns></returns>
        public decimal getRate()
        {
            return _rate;
        }
    }
}
