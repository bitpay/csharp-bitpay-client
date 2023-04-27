// Copyright (c) 2019 BitPay.
// All rights reserved.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BitPay.Clients
{
    public interface IBitPayClient
    {
        /// <summary>
        ///     Make a GET request
        /// </summary>
        /// <param name="uri">The URI to query</param>
        /// <param name="parameters">The request parameters</param>
        /// <param name="signatureRequired">Required signature</param>
        /// <returns>The HttpResponseMessage of the request</returns>
        Task<HttpResponseMessage> Get(string uri, Dictionary<string, dynamic>? parameters = null,
            bool signatureRequired = true);

        /// <summary>
        ///     Make a DELETE request
        /// </summary>
        /// <param name="uri">The URI to request</param>
        /// <param name="parameters">The parameters of the request</param>
        /// <returns>The HttpResponseMessage of the request</returns>
        Task<HttpResponseMessage> Delete(string uri, Dictionary<string, dynamic>? parameters = null);

        Task<HttpResponseMessage> Post(string uri, string json, bool signatureRequired = false);

        Task<HttpResponseMessage> Put(string uri, string json);
    }
}