// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
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
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _data = new Dictionary<string, string>();
            var env = configuration.GetSection("BitPayConfiguration:Environment").Value;

            var tokens = configuration.GetSection("BitPayConfiguration:EnvConfig:" + env + ":ApiTokens").GetChildren();
            foreach (IConfigurationSection token in tokens)
            {
                _data[token.Key] = token.Value;
            }
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

        public virtual string GetAccessToken(string key)
        {
            if (!_data.ContainsKey(key))
                throw new TokenNotFoundException(key);

            return _data[key];
        }

        /// <summary>
        ///     Specified whether the client has authorization (a token) for the specified facade.
        /// </summary>
        /// <param name="facade">The facade name for which authorization is tested.</param>
        /// <returns>True if this client is authorized, false otherwise.</returns>
        public virtual bool TokenExists(string facade)
        {
            return _data.ContainsKey(facade);
        }
    }
}