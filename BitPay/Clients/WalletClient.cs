using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BitPay.Exceptions;
using BitPay.Models.Wallet;
using BitPay.Utils;
using Newtonsoft.Json;

namespace BitPay.Clients
{
    public class WalletClient
    {
        private readonly IBitPayClient _bitPayClient;

        public WalletClient(IBitPayClient bitPayClient)
        {
            _bitPayClient = bitPayClient ?? throw new MissingRequiredField("bitPayClient");
        }
        
        /// <summary>
        ///     Retrieve all supported wallets.
        /// </summary>
        ///<returns>A list of wallet objets.</returns> 
        ///<throws>WalletQueryException WalletQueryException class</throws> 
        public async Task<List<Wallet>> GetSupportedWallets()
        {
            try
            {
                var response = await _bitPayClient.Get("supportedWallets", null, false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<List<Wallet>>(responseString);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new WalletQueryException(ex);

                throw;
            }
        }
    }
}