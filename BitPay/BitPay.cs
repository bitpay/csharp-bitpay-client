using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BitPayAPI.Exceptions;
using BitPayAPI.Models;
using BitPayAPI.Models.Invoice;
using BitPayAPI.Models.Ledger;
using BitPayAPI.Models.Rate;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;

/**
 * @author Antonio Buedo
 * @date 4.15.2019
 * @version 2.0.1904
 *
 * See bitpay.com/api for more information.
 */

namespace BitPayAPI
{
    public class BitPay
    {
        private static IConfiguration _configuration { get; set; }
        private static string _env;
        private Dictionary<string, string> _tokenCache; // {facade, token}
        private static string _configFilePath;
        
        private const string BitpayApiVersion = "2.0.0";
        private const string BitpayPluginInfo = "BitPay_DotNet_Client_v2.0.1904";

        private string _baseUrl;
        private string _clientName;
        private EcKey _ecKey;

        private HttpClient _httpClient;
        
        /// <summary>
        ///     Constructor for use if the keys and SIN are managed by this library.
        /// </summary>
        /// <param name="ConfigFilePath">The path to the configuration file.</param>
        public BitPay(string ConfigFilePath)
        {
            _configFilePath = ConfigFilePath;
            GetConfig();
            InitKeys().Wait();
            Init().Wait();
        }
        
        /// <summary>
        ///     Constructor for use if the keys and SIN are managed by this library.
        /// </summary>
        /// <param name="config">IConfiguration with loaded configuration.</param>
        public BitPay(IConfiguration config)
        {
            _configuration = config;
            _env = _configuration.GetSection("BitPayConfiguration:Environment").Value;
            InitKeys().Wait();
            Init().Wait();
        }

        /// <summary>
        ///     Return the identity of this client (i.e. the public key).
        /// </summary>
        public string Identity { get; private set; }

        /// <summary>
        ///     Authorize (pair) this client with the server using the specified pairing code.
        /// </summary>
        /// <param name="pairingCode">A code obtained from the server; typically from bitpay.com/api-tokens.</param>
        public async Task AuthorizeClient(string pairingCode)
        {
            try
            {
                var token = new Token
                {
                    Id = Identity, Guid = Guid.NewGuid().ToString(), PairingCode = pairingCode, Label = _clientName
                };
                var json = JsonConvert.SerializeObject(token);
                var response = await Post("tokens", json);
                var responseString = await ResponseToJsonString(response);
                var tokens = JsonConvert.DeserializeObject<List<Token>>(responseString);
                foreach (var t in tokens) CacheToken(t.Facade, t.Value);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new ClientAuthorizationException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Request authorization (a token) for this client in the specified facade.
        /// </summary>
        /// <param name="facade">The facade for which authorization is requested.</param>
        /// <returns>A pairing code for this client. This code must be used to authorize this client at BitPay.com/api-tokens.</returns>
        public async Task<string> RequestClientAuthorization(string facade)
        {
            try
            {
                var token = new Token
                {
                    Id = Identity,
                    Guid = Guid.NewGuid().ToString(),
                    Facade = facade,
                    Label = _clientName
                };
                var json = JsonConvert.SerializeObject(token);
                var response = await Post("tokens", json).ConfigureAwait(false);
                var responseString = await ResponseToJsonString(response).ConfigureAwait(false);
                var tokens = JsonConvert.DeserializeObject<List<Token>>(responseString);
                CacheToken(tokens[0].Facade, tokens[0].Value);

                return tokens[0].PairingCode;
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new ClientAuthorizationException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Specified whether the client has authorization (a token) for the specified facade.
        /// </summary>
        /// <param name="facade">The facade name for which authorization is tested.</param>
        /// <returns></returns>
        public bool tokenExist(string facade)
        {
            return _tokenCache.ContainsKey(facade);
        }


        /// <summary>
        ///     Returns the token for the specified facade.
        /// </summary>
        /// <param name="facade">The facade name for which the token is requested.</param>
        /// <returns></returns>
        public string GetTokenByFacade(string facade)
        {
            if (!_tokenCache.ContainsKey(facade))
                return "";

            return _tokenCache[facade];
        }

        /// <summary>
        ///     Create an invoice using the specified facade.
        /// </summary>
        /// <param name="invoice">An invoice request object.</param>
        /// <param name="facade">The facade to create the invoice against</param>
        /// <param name="signRequest">Allow unsigned request</param>
        /// <returns>A new invoice object returned from the server.</returns>
        public async Task<Invoice> CreateInvoice(Invoice invoice, string facade = Facade.PointOfSale, bool signRequest = true)
        {
            try
            {
                invoice.Token = GetAccessToken(facade);
                invoice.Guid = Guid.NewGuid().ToString();
                var json = JsonConvert.SerializeObject(invoice);
                var response = await Post("invoices", json, signRequest).ConfigureAwait(false);
                var responseString = await ResponseToJsonString(response).ConfigureAwait(false);
                JsonConvert.PopulateObject(responseString, invoice);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new InvoiceCreationException(ex);

                throw;
            }

            // Track the token for this invoice
            CacheToken(invoice.Id, invoice.Token);

            return invoice;
        }

        /// <summary>
        ///     Retrieve an invoice by id and token.
        /// </summary>
        /// <param name="invoiceId">The id of the requested invoice.</param>
        /// <param name="facade">The facade to get the invoice from</param>
        /// <returns>The invoice object retrieved from the server.</returns>
        public async Task<Invoice> GetInvoice(string invoiceId, string facade = Facade.Merchant, bool signRequest = true)
        {
            Dictionary<string, string> parameters = null;
            try
            {
                if (signRequest)
                {
                    // Provide the merchant token when the merchant facade is being used.
                    // GET/invoices expects the merchant token and not the merchant/invoice token.
                    try
                    {
                        parameters = new Dictionary<string, string>
                        {
                            {"token", GetAccessToken(facade)}
                        };
                    }
                    catch (BitPayException)
                    {
                        // No token for invoice.
                        parameters = null;
                    }
                }

                var response = await Get("invoices/" + invoiceId, parameters, signRequest);
                var responseString = await ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Invoice>(responseString);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new InvoiceQueryException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve a list of invoices by date range using the merchant facade.
        /// </summary>
        /// <param name="dateStart">The start date for the query.</param>
        /// <param name="dateEnd">The end date for the query.</param>
        /// <returns>A list of invoice objects retrieved from the server.</returns>
        public async Task<List<Invoice>> GetInvoices(DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                // UTC date, ISO-8601 format yyyy-mm-dd or yyyy-mm-ddThh:mm:ssZ. Default is current time.
                var parameters = InitParams();
                parameters.Add("token", GetAccessToken(Facade.Merchant));
                parameters.Add("dateStart", dateStart.ToString("yyyy-MM-dd"));
                parameters.Add("dateEnd", dateEnd.ToString("yyyy-MM-dd"));
                var response = await Get("invoices", parameters);
                var responseString = await ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<List<Invoice>>(responseString);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new InvoiceQueryException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve the exchange rate table using the public facade.
        /// </summary>
        /// <returns>The rate table as an object retrieved from the server.</returns>
        public async Task<Rates> GetRates()
        {
            try
            {
                var response = await Get("rates", signatureRequired: false);
                var responseString = await ResponseToJsonString(response);
                var rates = JsonConvert.DeserializeObject<List<Rate>>(responseString);
                return new Rates(rates, this);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new RatesQueryException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve a list of ledgers by date range using the merchant facade.
        /// </summary>
        /// <param name="currency">The three digit currency string for the ledger to retrieve.</param>
        /// <param name="dateStart">The start date for the query.</param>
        /// <param name="dateEnd">The end date for the query.</param>
        /// <returns>A list of invoice objects retrieved from the server.</returns>
        public async Task<Ledger> GetLedger(string currency, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var parameters = InitParams();
                parameters.Add("token", GetAccessToken(Facade.Merchant));
                parameters.Add("startDate", "" + dateStart.ToString("yyyy-MM-dd"));
                parameters.Add("endDate", "" + dateEnd.ToString("yyyy-MM-dd"));
                var response = await Get("ledgers/" + currency, parameters);
                var responseString = await ResponseToJsonString(response);
                var entries = JsonConvert.DeserializeObject<List<LedgerEntry>>(responseString);
                return new Ledger(entries);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new LedgerQueryException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Submit a BitPay Payout batch.
        /// </summary>
        /// <param name="batch">A PayoutBatch object with request parameters defined.</param>
        /// <returns>A BitPay generated PayoutBatch object.</returns>
        public async Task<PayoutBatch> SubmitPayoutBatch(PayoutBatch batch)
        {
            try
            {
                batch.Token = GetAccessToken(Facade.Payroll);
                batch.Guid = Guid.NewGuid().ToString();
                var json = JsonConvert.SerializeObject(batch);
                var response = await PostWithSignature("payouts", json);

                // To avoid having to merge instructions in the response with those we sent, we just remove the instructions
                // we sent and replace with those in the response.
                batch.Instructions = new List<PayoutInstruction>();
                var responseString = await ResponseToJsonString(response);
                JsonConvert.PopulateObject(responseString, batch, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                // Track the token for this batch
                CacheToken(batch.Id, batch.Token);

                return batch;
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BatchException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve a collection of BitPay payout batches.
        /// </summary>
        /// <returns>A list of BitPay PayoutBatch objects.</returns>
        public async Task<List<PayoutBatch>> GetPayoutBatches()
        {
            try
            {
                var parameters = InitParams();
                parameters.Add("token", GetAccessToken(Facade.Payroll));
                var response = await Get("payouts", parameters);
                var responseString = await ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<List<PayoutBatch>>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BatchException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve a BitPay payout batch by batch id using.  The client must have been previously authorized for the payroll
        ///     facade.
        /// </summary>
        /// <param name="batchId">The id of the batch to retrieve.</param>
        /// <returns>A BitPay PayoutBatch object.</returns>
        public async Task<PayoutBatch> GetPayoutBatch(string batchId)
        {
            try
            {
                Dictionary<string, string> parameters;
                try
                {
                    parameters = new Dictionary<string, string> {{"token", GetAccessToken(Facade.Payroll)}};
                }
                catch (BitPayException)
                {
                    // No token for batch.
                    parameters = null;
                }

                var response = await Get("payouts/" + batchId, parameters);
                var responseString = await ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<PayoutBatch>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BatchException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Cancel a BitPay Payout batch.
        /// </summary>
        /// <param name="batchId">The id of the batch to cancel.</param>
        /// <returns> A BitPay generated PayoutBatch object.</returns>
        public async Task<PayoutBatch> CancelPayoutBatch(string batchId)
        {
            try
            {
                var b = await GetPayoutBatch(batchId);

                Dictionary<string, string> parameters;
                try
                {
                    parameters = new Dictionary<string, string> {{"token", b.Token}};
                }
                catch (BitPayException)
                {
                    // No token for batch.
                    parameters = null;
                }

                var response = await Delete("payouts/" + batchId, parameters);
                var responseString = await ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<PayoutBatch>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BatchException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieves settlement reports for the calling merchant filtered by query. The `limit` and `offset` parameters
        ///     specify pages for large query sets.
        /// </summary>
        /// <param name="currency">The three digit currency string for the ledger to retrieve.</param>
        /// <param name="dateStart">The start date for the query.</param>
        /// <param name="dateEnd">The end date for the query.</param>
        /// <param name="status">Can be `processing`, `completed`, or `failed`.</param>
        /// <param name="limit">Maximum number of settlements to retrieve.</param>
        /// <param name="offset">Offset for paging</param>
        /// <returns>A list of BitPay Settlement objects</returns>
        public async Task<List<Settlement>> GetSettlements(string currency, DateTime dateStart, DateTime dateEnd,
            string status = "", int limit = 100, int offset = 0)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    {"token", GetAccessToken(Facade.Merchant)},
                    {"startDate", $"{dateStart.ToString("yyyy-MM-dd")}"},
                    {"endDate", $"{dateEnd.ToString("yyyy-MM-dd")}"},
                    {"currency", currency},
                    {"status", status},
                    {"limit", $"{limit}"},
                    {"offset", $"{offset}"}
                };

                var response = await Get("settlements", parameters);
                var responseString = await ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<List<Settlement>>(responseString);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new SettlementException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieves a summary of the specified settlement.
        /// </summary>
        /// <param name="settlementId">Settlement Id</param>
        /// <returns>A BitPay Settlement object.</returns>
        public async Task<Settlement> GetSettlement(string settlementId)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    {"token", GetAccessToken(Facade.Merchant)}
                };

                var response = await Get($"settlements/{settlementId}", parameters);
                var responseString = await ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Settlement>(responseString);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new SettlementException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Gets a detailed reconciliation report of the activity within the settlement period
        /// </summary>
        /// <param name="settlement">Settlement to generate report for.</param>
        /// <returns>A detailed BitPay Settlement object.</returns>
        public async Task<Settlement> GetSettlementReconciliationReport(Settlement settlement)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    {"token", settlement.Token}
                };

                var response = await Get($"settlements/{settlement.Id}/reconciliationReport", parameters);
                var responseString = await ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Settlement>(responseString);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new SettlementException(ex);

                throw;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///     Initialize this object with the client name and the environment Url
        /// </summary>
        /// <returns></returns>
        private async Task Init()
        {
            try
            {
                _baseUrl = _configuration.GetSection("BitPayConfiguration:EnvConfig:"+ _env +":ApiUrl").Value;
                _httpClient = new HttpClient {BaseAddress = new Uri(_baseUrl)};
                NormalizeClientName(_configuration.GetSection("BitPayConfiguration:EnvConfig:"+ _env +":ClientDescription").Value);
                DeriveIdentity();
                await LoadAccessTokens();
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BitPayException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Initialize the public/private key pair by either loading the existing one or by creating a new one
        /// </summary>
        /// <returns></returns>
        private async Task InitKeys()
        {
            if (KeyUtils.PrivateKeyExists(_configuration.GetSection("BitPayConfiguration:EnvConfig:"+ _env +":PrivateKeyPath").Value))
            {
                _ecKey = await KeyUtils.LoadEcKey();
            }
            else
            {
                _ecKey = KeyUtils.CreateEcKey();
                await KeyUtils.SaveEcKey(_ecKey);
            }
        }

        /// <summary>
        ///     Set the public key as the Identity of this object
        /// </summary>
        private void DeriveIdentity()
        {
            // Identity in this implementation is defined to be the SIN.
            Identity = KeyUtils.DeriveSin(_ecKey);
        }

        private void ClearAccessTokenCache()
        {
            _tokenCache = new Dictionary<string, string>();
        }

        /// <summary>
        ///     Add this token to the token cache
        /// </summary>
        /// <param name="key">The token type</param>
        /// <param name="token">The token value</param>
        /// <returns></returns>
        private void CacheToken(string key, string token)
        {
            // we add the token to the runtime dictionary
            if (tokenExist(key))
            {
                _tokenCache[key] = token;
            }
            else
            {
                _tokenCache.Add(key, token);
            }

            // we also persist the token
            WriteTokenCache();
        }

        /// <summary>
        ///     Persist the token cache to disk
        /// </summary>
        /// <returns></returns>
        private void WriteTokenCache()
        {
            try
            {
                foreach (var key in _tokenCache.Keys)
                {
                    _configuration["BitPayConfiguration:EnvConfig:" + _env + ":ApiTokens:" + key] =
                        key + ":" + _tokenCache[key];
                }
            }
            catch (Exception ex)
            {
                throw new TokensCacheWriteException(ex);
            }
        }

        /// <summary>
        ///     Load the access tokens from persistent storage
        /// </summary>
        /// <returns></returns>
        private async Task LoadAccessTokens()
        {
            try
            {
                ClearAccessTokenCache();
                
                IConfigurationSection tokenList = _configuration.GetSection("BitPayConfiguration:EnvConfig:" + _env + ":ApiTokens");
                foreach (IConfigurationSection token in tokenList.GetChildren().ToArray())
                {
                    if (_configuration["BitPayConfiguration:EnvConfig:" + _env + ":ApiTokens:" + token.Key] != null && !string.IsNullOrEmpty(token.Value)) {
                        _tokenCache.Add(token.Key, token.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new TokensCacheLoadException(ex);
            }
        }

        private string GetAccessToken(string key)
        {
            if (!_tokenCache.ContainsKey(key))
                throw new TokenNotFoundException(key);

            return _tokenCache[key];
        }

        /// <summary>
        ///     Just empty and parameters dictionary
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> InitParams()
        {
            var parameters = new Dictionary<string, string>();
            return parameters;
        }

        /// <summary>
        ///     Make a GET request
        /// </summary>
        /// <param name="uri">The URI to query</param>
        /// <param name="parameters">The request parameters</param>
        /// <returns>The HttpResponseMessage of the request</returns>
        private async Task<HttpResponseMessage> Get(string uri, Dictionary<string, string> parameters = null, bool signatureRequired = true)
        {
            try
            {
                var fullUrl = _baseUrl + uri;
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-accept-version", BitpayApiVersion);
                _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", BitpayPluginInfo);
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
                    _httpClient.DefaultRequestHeaders.Add("x-identity", KeyUtils.BytesToHex(_ecKey.PublicKey));
                }

                var result = await _httpClient.GetAsync(fullUrl);
                return result;
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
        private async Task<HttpResponseMessage> Delete(string uri, Dictionary<string, string> parameters = null)
        {
            try
            {
                var fullUrl = _baseUrl + uri;
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-accept-version", BitpayApiVersion);
                _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", BitpayPluginInfo);

                if (parameters != null)
                {
                    fullUrl += "?";
                    foreach (var entry in parameters) fullUrl += entry.Key + "=" + entry.Value + "&";

                    fullUrl = fullUrl.Substring(0, fullUrl.Length - 1);
                    var signature = KeyUtils.Sign(_ecKey, fullUrl);
                    _httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                    _httpClient.DefaultRequestHeaders.Add("x-identity", KeyUtils.BytesToHex(_ecKey.PublicKey));
                }

                var result = await _httpClient.DeleteAsync(fullUrl);
                return result;
            }
            catch (Exception ex)
            {
                throw new BitPayApiCommunicationException(ex);
            }
        }


        private async Task<HttpResponseMessage> PostWithSignature(string uri, string json)
        {
            return await Post(uri, json, true);
        }

        private async Task<HttpResponseMessage> Post(string uri, string json, bool signatureRequired = false)
        {
            try
            {
                var bodyContent = new StringContent(UnicodeToAscii(json));
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-accept-version", BitpayApiVersion);
                _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", BitpayPluginInfo);
                bodyContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                if (signatureRequired)
                {
                    var signature = KeyUtils.Sign(_ecKey, _baseUrl + uri + json);
                    _httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                    _httpClient.DefaultRequestHeaders.Add("x-identity", _ecKey?.PublicKeyHexBytes);
                }

                var result = await _httpClient.PostAsync(uri, bodyContent).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                throw new BitPayApiCommunicationException(ex);
            }
        }

        private async Task<string> ResponseToJsonString(HttpResponseMessage response)
        {
            if (response == null)
                throw new BitPayApiCommunicationException(new NullReferenceException("Response is null"));

            try
            {
                // Get the response as a dynamic object for detecting possible error(s) or data object.
                // An error(s) object raises an exception.
                // A data object has its content extracted (throw away the data wrapper object).
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                JObject jObj;
                if (!string.IsNullOrEmpty(responseString) && responseString != "[]")
                    try
                    {
                        jObj = JObject.Parse(responseString);
                    }
                    catch (Exception)
                    {
                        var jArray = JArray.Parse(responseString);
                        jObj = JObject.Parse(jArray[0].ToString());
                    }
                else
                    jObj = new JObject();

                JToken value;

                // Check for error response.
                if (jObj.TryGetValue("error", out value)) throw new BitPayApiCommunicationException(value.ToString());

                if (jObj.TryGetValue("errors", out value))
                {
                    var errors = value.Children().ToList();
                    var message = "Multiple errors:";
                    foreach (var errorItem in errors)
                    {
                        var error = errorItem.ToObject<JProperty>();
                        message += "\n" + error.Name + ": " + error.Value;
                    }

                    throw new BitPayApiCommunicationException(message);
                }

                // Check for and exclude a "data" object from the response.
                if (jObj.TryGetValue("data", out value)) responseString = value.ToString();

                return Regex.Replace(responseString, @"\r\n", "");
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BitPayApiCommunicationException(ex);

                throw;
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

        private void NormalizeClientName(string clientName)
        {
            if (clientName.Equals(BitpayPluginInfo)) clientName += " on " + Environment.MachineName;

            // Eliminate special characters from the client name (used as a token label).  Trim to 60 chars.
            _clientName = new Regex("[^a-zA-Z0-9_ ]").Replace(clientName, "_");
            if (_clientName.Length > 60) _clientName = _clientName.Substring(0, 60);
        }

        /// <summary>
        ///     Loads the configuration file (JSON)
        /// </summary>
        /// <returns></returns>
        private void GetConfig()
        {
            try
            {
                if (!File.Exists(_configFilePath))
                {
                    throw new Exception("Configuration file not found");
                }
                var builder = new ConfigurationBuilder().AddJsonFile(_configFilePath, false, true);
                _configuration = builder.Build();
                _env = _configuration.GetSection("BitPayConfiguration:Environment").Value;
            }
            catch (Exception ex)
            {
                throw new ConfigNotFoundException(ex);
            }
        }
    }
}