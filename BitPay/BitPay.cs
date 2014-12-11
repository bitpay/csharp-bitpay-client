using BitCoinSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using System.Web.Script.Serialization;

/**
 * @author Andy Phillipson
 * @date 9.3.2014
 * 
 * See bitpay.com/api for more information.
 */

namespace BitPayAPI
{

    public class BitPay
    {
        private const String BITPAY_API_VERSION = "2.0.0";
        private const String BITPAY_PLUGIN_INFO = "BitPay CSharp Client " + BITPAY_API_VERSION;
        private const String BITPAY_URL = "https://bitpay.com/";

        public const String FACADE_PAYROLL  = "payroll";
        public const String FACADE_POS = "pos";
        public const String FACADE_MERCHANT = "merchant";
        public const String FACADE_USER = "user";

        private HttpClient _httpClient = null;
        private String _baseUrl = BITPAY_URL;
        private EcKey _ecKey = null;
        private String _identity = "";
        private long _nonce = DateTime.Now.Ticks / 1000;
        private bool _disableNonce = false;
        private String _clientName = "";
        private Dictionary<string, string> _tokenCache; // {facade, token}

        /// <summary>
        /// Constructor for use if the keys and SIN are managed by this library.
        /// </summary>
        /// <param name="clientName">The label for this client.</param>
        /// <param name="envUrl">The target server URL.</param>
        public BitPay(String clientName = BITPAY_PLUGIN_INFO, String envUrl = BITPAY_URL)
        {
            // IgnoreBadCertificates();

            if (clientName.Equals(BITPAY_PLUGIN_INFO))
            {
                clientName += " on " + System.Environment.MachineName;
            }
            // Eliminate special characters from the client name (used as a token label).  Trim to 60 chars.
            string _clientName = new Regex("[^a-zA-Z0-9_ ]").Replace(clientName, "_");
            if (_clientName.Length > 60)
            {
                _clientName = _clientName.Substring(0, 60);
            }

            _baseUrl = envUrl;
    	    _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);

            this.initKeys();
            this.deriveIdentity();
            this.tryGetAccessTokens();
        }

        /// <summary>
        /// Constructor for use if the keys and SIN were derived external to this library.
        /// </summary>
        /// <param name="ecKey">An elliptical curve key.</param>
        /// <param name="clientName">The label for this client.</param>
        /// <param name="envUrl">The target server URL.</param>
        public BitPay(EcKey ecKey, String clientName = BITPAY_PLUGIN_INFO, String envUrl = BITPAY_URL)
        {
            // IgnoreBadCertificates();

            _ecKey = ecKey;
            this.deriveIdentity();
            _baseUrl = envUrl;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
            this.tryGetAccessTokens();
        }

        /// <summary>
        /// Return the identity of this client.
        /// </summary>
        public String Identity
        {
            get { return _identity; }
        }

        /// <summary>
        /// Disable use of the nonce for this client.  When disabled, the nonce will not be sent to the server in subsequent requests.
        /// </summary>
        public bool DisableNonce
        {
            get { return _disableNonce; }
            set { _disableNonce = DisableNonce; }
        }

        /// <summary>
        /// Authorize (pair) this client with the server using the specified pairing code.
        /// </summary>
        /// <param name="pairingCode">A code obtained from the server; typically from bitpay.com/api-tokens.</param>
        public void authorizeClient(String pairingCode)
        {
            Token token = new Token();
            token.Id = _identity;
            token.Guid = Guid.NewGuid().ToString();
            token.Nonce = NextNonce;
            token.PairingCode = pairingCode;
            token.Label = _clientName;
            String json = JsonConvert.SerializeObject(token);
            HttpResponseMessage response = this.post("tokens", json);
            List<Token> tokens = JsonConvert.DeserializeObject<List<Token>>(this.responseToJsonString(response));
            foreach (Token t in tokens)
            {
                _tokenCache.Add(t.Facade, t.Value);
            }
        }
        
        /// <summary>
        /// Request authorization (a token) for this client in the specified facade.
        /// </summary>
        /// <param name="facade">The facade for which authorization is requested.</param>
        /// <returns>A pairing code for this client.  This code must be used to authorize this client at BitPay.com/api-tokens.</returns>
        public String requestClientAuthorization(String facade)
        {
            Token token = new Token();
            token.Id = _identity;
            token.Guid = Guid.NewGuid().ToString();
            token.Nonce = NextNonce;
            token.Facade = facade;
            token.Count = 1;
            token.Label = _clientName;
            String json = JsonConvert.SerializeObject(token);
            HttpResponseMessage response = this.post("tokens", json);
            List<Token> tokens = JsonConvert.DeserializeObject<List<Token>>(this.responseToJsonString(response));
            // Expecting a single token resource.
            if (tokens.Count != 1)
            {
                throw new BitPayException("Error - failed to get token resource; expected 1 token, got " + tokens.Count);
            }
            _tokenCache.Add(tokens[0].Facade, tokens[0].Value);
            return tokens[0].PairingCode;
        }

        /// <summary>
        /// Specified whether the client has authorization (a token) for the specified facade.
        /// </summary>
        /// <param name="facade">The facade name for which authorization is tested.</param>
        /// <returns></returns>
        public bool clientIsAuthorized(String facade)
        {
            return _tokenCache.ContainsKey(facade);
        }

        /// <summary>
        /// Create an invoice using the specified facade.
        /// </summary>
        /// <param name="invoice">An invoice request object.</param>
        /// <returns>A new invoice object returned from the server.</returns>
        public Invoice createInvoice(Invoice invoice, String facade = FACADE_POS)
        {
            invoice.Token = this.getAccessToken(facade);
            invoice.Guid = Guid.NewGuid().ToString();
            invoice.Nonce = NextNonce;
            String json = JsonConvert.SerializeObject(invoice);
            HttpResponseMessage response = this.postWithSignature("invoices", json);
            JsonConvert.PopulateObject(this.responseToJsonString(response), invoice);
            return invoice;
        }

        /// <summary>
        /// Retrieve an invoice by id using the public facade.
        /// </summary>
        /// <param name="invoiceId">The id of the requested invoice.</param>
        /// <returns>The invoice object retrieved from the server.</returns>
        public Invoice getInvoice(String invoiceId)
        {
            HttpResponseMessage response = this.get("invoices/" + invoiceId);
            return JsonConvert.DeserializeObject<Invoice>(this.responseToJsonString(response));
        }

        /// <summary>
        /// Retrieve a list of invoice by date range using the merchant facade.
        /// </summary>
        /// <param name="dateStart">The start date for the query in javascript.</param>
        /// <param name="dateEnd">The end date for the query in javascript.</param>
        /// <returns>A list of invoice objects retrieved from the server.</returns>
        public List<Invoice> getInvoices(String dateStart, String dateEnd)
        {
            Dictionary<String, String> parameters = this.getParams();
            parameters.Add("token", this.getAccessToken(FACADE_MERCHANT));
            parameters.Add("dateStart", dateStart);
            parameters.Add("dateEnd", dateEnd);
            HttpResponseMessage response = this.get("invoices", parameters);
            return JsonConvert.DeserializeObject<List<Invoice>>(this.responseToJsonString(response));
        }

        /// <summary>
        /// Retrieve the exchange rate table using the public facade.
        /// </summary>
        /// <returns>The rate table as an object retrieved from the server.</returns>
        public Rates getRates()
        {
            HttpResponseMessage response = this.get("rates");
            List<Rate> rates = JsonConvert.DeserializeObject<List<Rate>>(this.responseToJsonString(response));
            return new Rates(rates, this);
        }

        private void initKeys()
        {
            if (KeyUtils.privateKeyExists())
            {
                _ecKey = KeyUtils.loadEcKey();

                // Alternatively, load your private key from a location you specify.
                // _ecKey = KeyUtils.createEcKeyFromHexStringFile("C:\\Users\\key.priv");
            }
            else
            {
                _ecKey = KeyUtils.createEcKey();
                KeyUtils.saveEcKey(_ecKey);
            }
        }

        private void deriveIdentity()
        {
            // Identity in this implementation is defined to be the SIN.
            _identity = KeyUtils.deriveSIN(_ecKey);
        }

        private long NextNonce
        {
            get
            {
                if (!DisableNonce)
                {
                    _nonce++;
                }
                else
                {
                    _nonce = 0;  // Nonce must be 0 when it has been disabled (0 value prevents serialization)
                }
                return _nonce;
            }
        }

        private Dictionary<string, string> responseToTokenCache(HttpResponseMessage response)
        {
            // The response is expected to be an array of key/value pairs (facade name = token).
            dynamic obj = Json.Decode(responseToJsonString(response));

            try
            {
                for (int i = 0; i < obj.Length; i++)
                {
                    Dictionary<string, object>.KeyCollection kc = obj[i].GetDynamicMemberNames();
                    if (kc.Count > 1)
                    {
                        throw new BitPayException("Size of Token object is unexpected.  Expected one entry, got " + kc.Count + " entries.");
                    }
                    foreach (string key in kc)
                    {
                        if (!_tokenCache.ContainsKey(key))
                        {
                            _tokenCache.Add(key, obj[i][key]);
                        }
                    }
                }
            }
            catch (BitPayException ex)
            {
                throw new BitPayException("Error: " + ex.ToString());
            }
            catch (Exception ex)
            {
                throw new BitPayException("Error: response to GET /tokens could not be parsed - " + ex.ToString());
            }
            return _tokenCache;
        }

        private void clearAccessTokenCache() {
            _tokenCache = new Dictionary<string, string>();
        }

        private bool tryGetAccessTokens()
        {
            // Attempt to get access tokens for this client identity.
            try
            {
                // Success if at least one access token was returned.
                return this.getAccessTokens() > 0;
            }
            catch (BitPayException ex)
            {
                // If the error states that the identity is invalid then this client has not been
                // registered with the BitPay account.
                if (ex.getMessage().Contains("Unauthorized sin"))
                {
                    this.clearAccessTokenCache();
                    return false;
                }
                else
                {
                    // Propagate all other errors.
                    throw ex;
                }
            }
        }

        private int getAccessTokens()
        {
            this.clearAccessTokenCache();
            Dictionary<String, String> parameters = this.getParams();
            HttpResponseMessage response = this.get("tokens", parameters);
            _tokenCache = responseToTokenCache(response);
            return _tokenCache.Count;
        }

        private String getAccessToken(String facade)
        {
            if (!_tokenCache.ContainsKey(facade))
            {
                throw new BitPayException("Error: You do not have access to facade: " + facade);
            }
            return _tokenCache[facade];
        }

        private Dictionary<string, string> getParams()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("nonce", NextNonce + "");
            return parameters;
        }

        private HttpResponseMessage get(String uri, Dictionary<string, string> parameters = null)
        {
            try
            {
                String fullURL = _baseUrl + uri;
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-accept-version", BITPAY_API_VERSION);
                _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", BITPAY_PLUGIN_INFO);
                if (parameters != null)
                {
                    fullURL += "?";
                    foreach (KeyValuePair<string, string> entry in parameters)
                    {
                        fullURL += entry.Key + "=" + entry.Value + "&";
                    }
                    fullURL = fullURL.Substring(0, fullURL.Length - 1);
                    String signature = KeyUtils.sign(_ecKey, fullURL);
                    _httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                    _httpClient.DefaultRequestHeaders.Add("x-identity", KeyUtils.bytesToHex(_ecKey.PubKey));
                }

                var result = _httpClient.GetAsync(fullURL).Result;
                return result;
            }
            catch (Exception ex)
            {
                throw new BitPayException("Error: " + ex.ToString());
            }
        }

        private HttpResponseMessage postWithSignature(String uri, String json)
        {
            return this.post(uri, json, true);
        }

        private HttpResponseMessage post(String uri, String json, bool signatureRequired = false)
        {
            try
            {
                var bodyContent = new StringContent(json);
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-accept-version", BITPAY_API_VERSION);
                _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", BITPAY_PLUGIN_INFO);
                bodyContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                if (signatureRequired)
                {
                    String signature = KeyUtils.sign(_ecKey, _baseUrl + uri + json);
                    _httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                    _httpClient.DefaultRequestHeaders.Add("x-identity", KeyUtils.bytesToHex(_ecKey.PubKey));
                }
                var result = _httpClient.PostAsync(uri, bodyContent).Result;
                return result;
            }
            catch (Exception ex)
            {
                throw new BitPayException("Error: " + ex.ToString());
            }
        }

        private String responseToJsonString(HttpResponseMessage response)
        {
            if (response == null)
            {
                throw new BitPayException("Error: HTTP response is null");
            }

            // Get the response as a dynamic object for detecting a possible "error" or "data" object.
            // An "error" object raises an exception.
            // A "data" object has its content extracted (we throw away the "data" wrapper object).
            String responseString = response.Content.ReadAsStringAsync().Result;
            dynamic obj = Json.Decode(responseString);

            // Check for error response.
            if (dynamicObjectHasProperty(obj, "error"))
            {
                throw new BitPayException("Error: " + obj.error);
            }
            if (dynamicObjectHasProperty(obj, "errors"))
            {
                String message = "Multiple errors:";
                foreach (var errorItem in obj.errors)
                {
                    message += "\n" + errorItem.error + " " + errorItem.param;
                }
                throw new BitPayException(message);
            }

            // Get a JSON string representation of the object.
            Newtonsoft.Json.Linq.JObject j = Newtonsoft.Json.Linq.JObject.Parse(responseString);
            String jsonString = j.ToString();

            // Check for and exclude a "data" object from the response.
            if (dynamicObjectHasProperty(obj, "data"))
            {
                jsonString = (String)j.SelectToken("data").ToString();
            }

            return Regex.Replace(jsonString, @"\r\n", "");
        }

        private static bool dynamicObjectHasProperty(dynamic obj, string name)
        {
            Dictionary<string, object>.KeyCollection kc = obj.GetDynamicMemberNames();
            return kc.Contains(name);
        }

        private void IgnoreBadCertificates()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        }

        private bool AcceptAllCertifications(
            object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certification,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
