using System.Collections.Generic;
using BitPay.Exceptions;
using Microsoft.Extensions.Configuration;

namespace BitPay
{
    public class AccessTokens
    {
        private Dictionary<string, string> _data;

        public AccessTokens()
        {
            _data = new Dictionary<string, string>();
        }

        public AccessTokens(IConfiguration configuration)
        {
            _data = new Dictionary<string, string>();
            var env = configuration.GetSection("BitPayConfiguration:Environment").Value;

            var a = configuration.GetSection("BitPayConfiguration:EnvConfig:" + env + ":ApiTokens").Value;





        }

        /// <summary>
        /// Add merchant token.
        /// </summary>
        /// <param name="token">Token</param>
        public void AddMerchant(string token)
        {
            _data[Facade.Merchant] = token;
        }
        
        /// <summary>
        /// Add payout token.
        /// </summary>
        /// <param name="token">Token</param>
        public void AddPayout(string token)
        {
            _data[Facade.Payout] = token;
        }
        
        /// <summary>
        /// Add POS token.
        /// </summary>
        /// <param name="token">Token</param>
        public void AddPos(string token)
        {
            _data[Facade.Pos] = token;
        }

        /// <summary>
        /// Add facade token.
        /// </summary>
        /// <param name="facade">Facade</param>
        /// <param name="token">Token</param>
        public void AddToken(string facade, string token)
        {
            _data[facade] = token;
        }

        public string GetAccessToken(string key)
        {
            if (!_data.ContainsKey(key))
                throw new TokenNotFoundException(key);

            return _data[key];
        }
    }
}