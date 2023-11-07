// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using BitPay.Exceptions;
using BitPay.Logger;

namespace BitPay.Clients
{
    public class BitPayClient : IBitPayClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly EcKey? _ecKey;

        public BitPayClient(HttpClient httpClient, string baseUrl, EcKey? ecKey)
        {
            this._httpClient = httpClient;
            this._baseUrl = baseUrl;
            this._ecKey = ecKey;
        }

        /// <summary>
        ///     Make a GET request
        /// </summary>
        /// <param name="uri">The URI to query</param>
        /// <param name="parameters">The request parameters</param>
        /// <param name="signatureRequired">Required signature</param>
        /// <returns>The HttpResponseMessage of the request</returns>
        /// <throws>BitPayApiException BitPayApiException class</throws>
        /// <throws>BitPayGenericException BitPayGenericException class</throws>
        public async Task<HttpResponseMessage> Get(string uri, Dictionary<string, dynamic?>? parameters = null,
            bool signatureRequired = true)
        {
            var fullUrl = _baseUrl + uri;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-accept-version", Config.BitPayApiVersion);
            _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", Config.BitPayPluginInfo);
            _httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame", Config.BitPayApiFrame);
            _httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame-version", Config.BitPayApiFrameVersion);
            if (parameters != null)
            {
                fullUrl += "?";
                foreach (var entry in parameters) fullUrl += entry.Key + "=" + entry.Value + "&";

                fullUrl = fullUrl.Substring(0, fullUrl.Length - 1);
            }
            
            if (signatureRequired)
            {
                var signature = KeyUtils.Sign(_ecKey, fullUrl);
                _httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                _httpClient.DefaultRequestHeaders.Add("x-identity", KeyUtils.BytesToHex(_ecKey?.GetPublicKey()));
            }
            
            LoggerProvider.GetLogger().LogRequest("GET", fullUrl, null);

            HttpResponseMessage result = null!;

            try
            {
                result = await _httpClient.GetAsync(new Uri(fullUrl)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowApiExceptionWithMessage(e.Message);
            }

            return result;
        }

        /// <summary>
        ///     Make a DELETE request
        /// </summary>
        /// <param name="uri">The URI to request</param>
        /// <param name="parameters">The parameters of the request</param>
        /// <returns>The HttpResponseMessage of the request</returns>
        public async Task<HttpResponseMessage> Delete(string uri, Dictionary<string, dynamic?>? parameters = null)
        {
            var fullUrl = _baseUrl + uri;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-accept-version", Config.BitPayApiVersion);
            _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", Config.BitPayPluginInfo);
            _httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame", Config.BitPayApiFrame);
            _httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame-version", Config.BitPayApiFrameVersion);

            if (parameters != null)
            {
                fullUrl += "?";
                foreach (var entry in parameters) fullUrl += entry.Key + "=" + entry.Value + "&";

                fullUrl = fullUrl.Substring(0, fullUrl.Length - 1);
                var signature = KeyUtils.Sign(_ecKey, fullUrl);
                _httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                _httpClient.DefaultRequestHeaders.Add("x-identity", KeyUtils.BytesToHex(_ecKey?.GetPublicKey()));
            }
            
            LoggerProvider.GetLogger().LogRequest("DELETE", fullUrl, null);
            
            HttpResponseMessage result = null!;
            
            try
            {
                result = await _httpClient.DeleteAsync(new Uri(fullUrl)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowApiExceptionWithMessage(e.Message);
            }

            return result;
        }

        public async Task<HttpResponseMessage> Post(string uri, string json, bool signatureRequired = false)
        {
            using var bodyContent = new StringContent(UnicodeToAscii(json));
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-accept-version", Config.BitPayApiVersion);
            _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", Config.BitPayPluginInfo);
            _httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame", Config.BitPayApiFrame);
            _httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame-version", Config.BitPayApiFrameVersion);
            bodyContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            if (signatureRequired)
            {
                var signature = KeyUtils.Sign(_ecKey, _baseUrl + uri + json);
                _httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                _httpClient.DefaultRequestHeaders.Add("x-identity", _ecKey?.PublicKeyHexBytes);
            }

            Uri requestUri = new Uri(_baseUrl + uri);
            
            HttpResponseMessage result = null!;
            
            LoggerProvider.GetLogger().LogRequest("POST", requestUri.ToString(), bodyContent.ToString());
            
            try
            {
                result = await _httpClient.PostAsync(requestUri, bodyContent).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowApiExceptionWithMessage(e.Message);
            }

            return result;
        }

        public async Task<HttpResponseMessage> Put(string uri, string json)
        {
            using var bodyContent = new StringContent(UnicodeToAscii(json));
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-accept-version", Config.BitPayApiVersion);
            _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", Config.BitPayPluginInfo);
            _httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame", Config.BitPayApiFrame);
            _httpClient.DefaultRequestHeaders.Add("x-bitpay-api-frame-version", Config.BitPayApiFrameVersion);
            bodyContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var signature = KeyUtils.Sign(_ecKey, _baseUrl + uri + json);
            _httpClient.DefaultRequestHeaders.Add("x-signature", signature);
            _httpClient.DefaultRequestHeaders.Add("x-identity", _ecKey?.PublicKeyHexBytes);

            Uri requestUri = new Uri(_baseUrl + uri);
            
            HttpResponseMessage result = null!;
            
            LoggerProvider.GetLogger().LogRequest("PUT", requestUri.ToString(), bodyContent.ToString());
            
            try
            {
                result = await _httpClient.PutAsync(new Uri(_baseUrl + uri), bodyContent).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowApiExceptionWithMessage(e.Message);
            }

            return result;
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