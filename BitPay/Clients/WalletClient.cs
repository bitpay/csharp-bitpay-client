// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BitPay.Exceptions;
using BitPay.Models.Wallet;

using Newtonsoft.Json;

namespace BitPay.Clients
{
    public class WalletClient
    {
        private readonly IBitPayClient _bitPayClient;

        public WalletClient(IBitPayClient bitPayClient)
        {
            _bitPayClient = bitPayClient;
        }
        
        /// <summary>
        ///     Retrieve all supported wallets.
        /// </summary>
        /// <returns>A list of wallet objets.</returns> 
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<List<Wallet>> GetSupportedWallets()
        {
            var response = await _bitPayClient.Get("supportedWallets", null, false)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response)
                .ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<List<Wallet>>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Wallet", e.Message);
                throw;
            }
        }
    }
}