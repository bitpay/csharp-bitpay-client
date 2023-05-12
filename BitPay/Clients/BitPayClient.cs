// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using BitPay.Exceptions;

namespace BitPay.Clients
{
    public class BitPayClient : IBitPayClient
    {
        private HttpClient httpClient;
        private string baseUrl;
        private EcKey? ecKey;

        public BitPayClient(HttpClient httpClient, string baseUrl, EcKey? ecKey)
        {
            this.httpClient = httpClient;
            this.baseUrl = baseUrl;
            this.ecKey = ecKey;
        }

        /// <summary>
        ///     Make a GET request
        /// </summary>
        /// <param name="uri">The URI to query</param>
        /// <param name="parameters">The request parameters</param>
        /// <param name="signatureRequired">Required signature</param>
        /// <returns>The HttpResponseMessage of the request</returns>
        public async Task<HttpResponseMessage> Get(string uri, Dictionary<string, dynamic?>? parameters = null,
            bool signatureRequired = true)
        {
            try
            {
                var fullUrl = baseUrl + uri;
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("x-accept-version", Config.BitPayApiVersion);
                httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", Config.BitPayPluginInfo);
                httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame", Config.BitPayApiFrame);
                httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame-version", Config.BitPayApiFrameVersion);
                if (parameters != null)
                {
                    fullUrl += "?";
                    foreach (var entry in parameters) fullUrl += entry.Key + "=" + entry.Value + "&";

                    fullUrl = fullUrl.Substring(0, fullUrl.Length - 1);
                }

                if (signatureRequired)
                {
                    var signature = KeyUtils.Sign(ecKey, fullUrl);
                    httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                    httpClient.DefaultRequestHeaders.Add("x-identity", KeyUtils.BytesToHex(ecKey?.GetPublicKey()));
                }

                return await httpClient.GetAsync(new Uri(fullUrl)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new BitPayApiCommunicationException(ex);
            }
        }

        /// <summary>
        ///     Make a DELETE request
        /// </summary>
        /// <param name="uri">The URI to request</param>
        /// <param name="parameters">The parameters of the request</param>
        /// <returns>The HttpResponseMessage of the request</returns>
        public async Task<HttpResponseMessage> Delete(string uri, Dictionary<string, dynamic?>? parameters = null)
        {
            try
            {
                var fullUrl = baseUrl + uri;
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("x-accept-version", Config.BitPayApiVersion);
                httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", Config.BitPayPluginInfo);
                httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame", Config.BitPayApiFrame);
                httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame-version", Config.BitPayApiFrameVersion);

                if (parameters != null)
                {
                    fullUrl += "?";
                    foreach (var entry in parameters) fullUrl += entry.Key + "=" + entry.Value + "&";

                    fullUrl = fullUrl.Substring(0, fullUrl.Length - 1);
                    var signature = KeyUtils.Sign(ecKey, fullUrl);
                    httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                    httpClient.DefaultRequestHeaders.Add("x-identity", KeyUtils.BytesToHex(ecKey?.GetPublicKey()));
                }

                return await httpClient.DeleteAsync(new Uri(fullUrl)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new BitPayApiCommunicationException(ex);
            }
        }

        public async Task<HttpResponseMessage> Post(string uri, string json, bool signatureRequired = false)
        {
            try
            {
                using (var bodyContent = new StringContent(UnicodeToAscii(json)))
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Add("x-accept-version", Config.BitPayApiVersion);
                    httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", Config.BitPayPluginInfo);
                    httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame", Config.BitPayApiFrame);
                    httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame-version", Config.BitPayApiFrameVersion);
                    bodyContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    if (signatureRequired)
                    {
                        var signature = KeyUtils.Sign(ecKey, baseUrl + uri + json);
                        httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                        httpClient.DefaultRequestHeaders.Add("x-identity", ecKey?.PublicKeyHexBytes);
                    }

                    return await httpClient.PostAsync(new Uri(baseUrl + uri), bodyContent).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new BitPayApiCommunicationException(ex);
            }
        }

        public async Task<HttpResponseMessage> Put(string uri, string json)
        {
            try
            {
                using (var bodyContent = new StringContent(UnicodeToAscii(json)))
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Add("x-accept-version", Config.BitPayApiVersion);
                    httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", Config.BitPayPluginInfo);
                    httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame", Config.BitPayApiFrame);
                    httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame-version", Config.BitPayApiFrameVersion);
                    bodyContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var signature = KeyUtils.Sign(ecKey, baseUrl + uri + json);
                    httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                    httpClient.DefaultRequestHeaders.Add("x-identity", ecKey?.PublicKeyHexBytes);

                    return await httpClient.PutAsync(new Uri(baseUrl + uri), bodyContent).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new BitPayApiCommunicationException(ex);
            }
        }

        private string UnicodeToAscii(string json)
        {
            var unicodeBytes = Encoding.Unicode.GetBytes(json);
            var asciiBytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, unicodeBytes);
            var asciiChars = new char[Encoding.ASCII.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            Encoding.ASCII.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            return new string(asciiChars);
        }
    }
}