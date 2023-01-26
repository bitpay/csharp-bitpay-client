using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BitPay.Exceptions;

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
        public Task<HttpResponseMessage> Get(string uri, Dictionary<string, dynamic> parameters = null,
            bool signatureRequired = true);

        /// <summary>
        ///     Make a DELETE request
        /// </summary>
        /// <param name="uri">The URI to request</param>
        /// <param name="parameters">The parameters of the request</param>
        /// <returns>The HttpResponseMessage of the request</returns>
        public Task<HttpResponseMessage> Delete(string uri, Dictionary<string, dynamic> parameters = null);

        public Task<HttpResponseMessage> Post(string uri, string json, bool signatureRequired = false);

        public Task<HttpResponseMessage> Put(string uri, string json);
    }
}