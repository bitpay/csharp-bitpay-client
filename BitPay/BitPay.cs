using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using BitPayAPI.Exceptions;
using BitPayAPI.Models;

/**
 * @author Andy Phillipson
 * @date 9.3.2014
 * 
 * See bitpay.com/api for more information.
 */

namespace BitPayAPI {

    public class BitPay {
        private const string BitpayApiVersion = "2.0.0";
        private const string BitpayPluginInfo = "BitPay CSharp Client " + BitpayApiVersion;
        private const string BitpayUrl = "https://bitpay.com/";
        private const string TokensFile = "bitpay_tokens";

        public const string FacadePayroll = "payroll";
        public const string FacadePos = "pos";
        public const string FacadeMerchant = "merchant";
        public const string FacadeUser = "user";

        private HttpClient _httpClient;
        private string _baseUrl;
        private EcKey _ecKey;
        private string _clientName = "";
        private Dictionary<string, string> _tokenCache; // {facade, token}

        /// <summary>
        /// Constructor for use if the keys and SIN are managed by this library.
        /// </summary>
        /// <param name="clientName">The label for this client.</param>
        /// <param name="envUrl">The target server URL.</param>
        public BitPay(string clientName = BitpayPluginInfo, string envUrl = BitpayUrl) {
            InitKeys();
            Init(clientName, envUrl);
        }

        /// <summary>
        /// Constructor for use if the keys and SIN were derived external to this library.
        /// </summary>
        /// <param name="ecKey">An elliptical curve key.</param>
        /// <param name="clientName">The label for this client.</param>
        /// <param name="envUrl">The target server URL.</param>
        public BitPay(EcKey ecKey, string clientName = BitpayPluginInfo, string envUrl = BitpayUrl) {
            _ecKey = ecKey;
            Init(clientName, envUrl);
        }

        /// <summary>
        /// Return the identity of this client.
        /// </summary>
        public string Identity { get; private set; }

        /// <summary>
        /// Authorize (pair) this client with the server using the specified pairing code.
        /// </summary>
        /// <param name="pairingCode">A code obtained from the server; typically from bitpay.com/api-tokens.</param>
        public void AuthorizeClient(string pairingCode) {
            try {
                var token = new Token {
                    Id = Identity, Guid = Guid.NewGuid().ToString(), PairingCode = pairingCode, Label = _clientName
                };
                var json = JsonConvert.SerializeObject(token);
                var response = Post("tokens", json);
                var tokens = JsonConvert.DeserializeObject<List<Token>>(ResponseToJsonString(response));
                foreach (var t in tokens) {
                    CacheToken(t.Facade, t.Value);
                }
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new ClientAuthorizationException(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Request authorization (a token) for this client in the specified facade.
        /// </summary>
        /// <param name="facade">The facade for which authorization is requested.</param>
        /// <returns>A pairing code for this client.  This code must be used to authorize this client at BitPay.com/api-tokens.</returns>
        public string RequestClientAuthorization(string facade) {
            try {
                var token = new Token {
                    Id = Identity,
                    Guid = Guid.NewGuid().ToString(),
                    Facade = facade,
                    Count = 1,
                    Label = _clientName
                };
                var json = JsonConvert.SerializeObject(token);
                var response = Post("tokens", json);
                var tokens = JsonConvert.DeserializeObject<List<Token>>(ResponseToJsonString(response));
                if(!tokens.Any())
                    throw new TokenRegistrationException();

                // why would we care if it's more than one token? In what case could this happen?
                //if (tokens.Count != 1)
                //    throw new BitPayException("Error - failed to get token resource; expected 1 token, got " +
                //                              tokens.Count);

                CacheToken(tokens[0].Facade, tokens[0].Value);
                return tokens[0].PairingCode;
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new ClientAuthorizationException(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Specified whether the client has authorization (a token) for the specified facade.
        /// </summary>
        /// <param name="facade">The facade name for which authorization is tested.</param>
        /// <returns></returns>
        public bool ClientIsAuthorized(string facade) {
            return _tokenCache.ContainsKey(facade);
        }

        /// <summary>
        /// Create an invoice using the specified facade.
        /// </summary>
        /// <param name="invoice">An invoice request object.</param>
        /// <param name="facade">The facade to create the invoice against</param>
        /// <returns>A new invoice object returned from the server.</returns>
        public Invoice CreateInvoice(Invoice invoice, string facade = FacadePos) {
            try {
                invoice.Token = GetAccessToken(facade);
                invoice.Guid = Guid.NewGuid().ToString();
                var json = JsonConvert.SerializeObject(invoice);
                var response = PostWithSignature("invoices", json);
                JsonConvert.PopulateObject(ResponseToJsonString(response), invoice);
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new InvoiceCreationException(ex);
                }

                throw;
            }

            // Track the token for this invoice
            CacheToken(invoice.Id, invoice.Token);

            return invoice;
        }

        /// <summary>
        /// Retrieve an invoice by id and token.
        /// </summary>
        /// <param name="invoiceId">The id of the requested invoice.</param>
        /// <param name="facade">The facade to get the invoice from</param>
        /// <returns>The invoice object retrieved from the server.</returns>
        public Invoice GetInvoice(string invoiceId, string facade = FacadeMerchant) {
            try {
                // Provide the merchant token when the merchant facade is being used.
                // GET/invoices expects the merchant token and not the merchant/invoice token.
                Dictionary<string, string> parameters;
                try {
                    parameters = new Dictionary<string, string> {
                        {"token", GetAccessToken(facade)}
                    };
                } catch (BitPayException) {
                    // No token for invoice.
                    parameters = null;
                }

                var response = Get("invoices/" + invoiceId, parameters);
                return JsonConvert.DeserializeObject<Invoice>(ResponseToJsonString(response));
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new InvoiceQueryException(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Retrieve a list of invoices by date range using the merchant facade.
        /// </summary>
        /// <param name="dateStart">The start date for the query.</param>
        /// <param name="dateEnd">The end date for the query.</param>
        /// <returns>A list of invoice objects retrieved from the server.</returns>
        public List<Invoice> GetInvoices(DateTime dateStart, DateTime dateEnd) {
            try {
                var parameters = GetParams();
                parameters.Add("token", GetAccessToken(FacadeMerchant));
                parameters.Add("dateStart", dateStart.ToShortDateString());
                parameters.Add("dateEnd", dateEnd.ToShortDateString());
                var response = Get("invoices", parameters);
                return JsonConvert.DeserializeObject<List<Invoice>>(ResponseToJsonString(response));
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new InvoiceQueryException(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Retrieve the exchange rate table using the public facade.
        /// </summary>
        /// <returns>The rate table as an object retrieved from the server.</returns>
        public Rates GetRates() {
            try {
                var response = Get("rates");
                var rates = JsonConvert.DeserializeObject<List<Rate>>(ResponseToJsonString(response));
                return new Rates(rates, this);
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new RatesQueryException(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Retrieve a list of ledgers by date range using the merchant facade.
        /// </summary>
        /// <param name="currency">The three digit currency string for the ledger to retrieve.</param>
        /// <param name="dateStart">The start date for the query.</param>
        /// <param name="dateEnd">The end date for the query.</param>
        /// <returns>A list of invoice objects retrieved from the server.</returns>
        public Ledger GetLedger(string currency, DateTime dateStart, DateTime dateEnd) {
            try {
                var parameters = GetParams();
                parameters.Add("token", GetAccessToken(FacadeMerchant));
                parameters.Add("startDate", "" + dateStart.ToShortDateString());
                parameters.Add("endDate", "" + dateEnd.ToShortDateString());
                var response = Get("ledgers/" + currency, parameters);
                var entries =
                    JsonConvert.DeserializeObject<List<LedgerEntry>>(ResponseToJsonString(response));
                return new Ledger(entries);
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new LedgerQueryException(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Submit a BitPay Payout batch.
        /// </summary>
        /// <param name="batch">A PayoutBatch object with request parameters defined.</param>
        /// <returns>A BitPay generated PayoutBatch object.</returns>
        public PayoutBatch SubmitPayoutBatch(PayoutBatch batch) {
            try {
                batch.Token = GetAccessToken(FacadePayroll);
                batch.Guid = Guid.NewGuid().ToString();
                var json = JsonConvert.SerializeObject(batch);
                var response = PostWithSignature("payouts", json);

                // To avoid having to merge instructions in the response with those we sent, we just remove the instructions
                // we sent and replace with those in the response.
                batch.Instructions = new List<PayoutInstruction>();

                JsonConvert.PopulateObject(ResponseToJsonString(response), batch, new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                });

                // Track the token for this batch
                CacheToken(batch.Id, batch.Token);

                return batch;
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new BatchException(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Retrieve a collection of BitPay payout batches.
        /// </summary>
        /// <returns>A list of BitPay PayoutBatch objects.</returns>
        public List<PayoutBatch> GetPayoutBatches() {
            try {
                var parameters = GetParams();
                parameters.Add("token", GetAccessToken(FacadePayroll));
                var response = Get("payouts", parameters);
                return JsonConvert.DeserializeObject<List<PayoutBatch>>(ResponseToJsonString(response),
                    new JsonSerializerSettings {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new BatchException(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Retrieve a BitPay payout batch by batch id using.  The client must have been previously authorized for the payroll facade.
        /// </summary>
        /// <param name="batchId">The id of the batch to retrieve.</param>
        /// <returns>A BitPay PayoutBatch object.</returns>
        public PayoutBatch GetPayoutBatch(string batchId) {
            try {
                Dictionary<string, string> parameters;
                try {
                    parameters = new Dictionary<string, string> {{"token", GetAccessToken(FacadePayroll)}};
                } catch (BitPayException) {
                    // No token for batch.
                    parameters = null;
                }

                var response = Get("payouts/" + batchId, parameters);
                return JsonConvert.DeserializeObject<PayoutBatch>(ResponseToJsonString(response),
                    new JsonSerializerSettings {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new BatchException(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Cancel a BitPay Payout batch.
        /// </summary>
        /// <param name="batchId">The id of the batch to cancel.</param>
        /// <returns> A BitPay generated PayoutBatch object.</returns>
        public PayoutBatch CancelPayoutBatch(string batchId) {
            try {
                var b = GetPayoutBatch(batchId);

                Dictionary<string, string> parameters;
                try {
                    parameters = new Dictionary<string, string> {{"token", b.Token}};
                } catch (BitPayException) {
                    // No token for batch.
                    parameters = null;
                }

                var response = Delete("payouts/" + batchId, parameters);
                return JsonConvert.DeserializeObject<PayoutBatch>(ResponseToJsonString(response),
                    new JsonSerializerSettings {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new BatchException(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Retrieves settlement reports for the calling merchant filtered by query. The `limit` and `offset` parameters specify pages for large query sets.
        /// </summary>
        /// <param name="currency">The three digit currency string for the ledger to retrieve.</param>
        /// <param name="dateStart">The start date for the query.</param>
        /// <param name="dateEnd">The end date for the query.</param>
        /// <param name="status">Can be `processing`, `completed`, or `failed`.</param>
        /// <param name="limit">Maximum number of settlements to retrieve.</param>
        /// <param name="offset">Offset for paging</param>
        /// <returns>A list of BitPay Settlement objects</returns>
        public List<Settlement> GetSettlements(string currency, DateTime dateStart, DateTime dateEnd,
            string status = "", int limit = 100, int offset = 0) {
            try {
                var parameters = new Dictionary<string, string> {
                    {"token", GetAccessToken(FacadeMerchant)},
                    {"startDate", $"{dateStart.ToShortDateString()}"},
                    {"endDate", $"{dateEnd.ToShortDateString()}"},
                    {"currency", currency},
                    {"status", status},
                    {"limit", $"{limit}"},
                    {"offset", $"{offset}"}
                };

                var response = Get("settlements", parameters);
                return JsonConvert.DeserializeObject<List<Settlement>>(ResponseToJsonString(response));
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new SettlementException(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Retrieves a summary of the specified settlement.
        /// </summary>
        /// <param name="settlementId">Settlement Id</param>
        /// <returns>A BitPay Settlement object.</returns>
        public Settlement GetSettlement(string settlementId) {
            try {
                var parameters = new Dictionary<string, string> {
                    {"token", GetAccessToken(FacadeMerchant)}
                };

                var response = Get($"settlements/{settlementId}", parameters);
                return JsonConvert.DeserializeObject<Settlement>(ResponseToJsonString(response));
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new SettlementException(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Gets a detailed reconciliation report of the activity within the settlement period
        /// </summary>
        /// <param name="settlement">Settlement to generate report for.</param>
        /// <returns>A detailed BitPay Settlement object.</returns>
        public Settlement GetSettlementReconciliationReport(Settlement settlement) {
            try {
                var parameters = new Dictionary<string, string> {
                    {"token", settlement.Token}
                };

                var response = Get($"settlements/{settlement.Id}/reconciliationReport", parameters);
                return JsonConvert.DeserializeObject<Settlement>(ResponseToJsonString(response));
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new SettlementException(ex);
                }

                throw;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Init(string clientName, string envUrl) {
            try {
                // IgnoreBadCertificates();
                _baseUrl = envUrl;
                _httpClient = new HttpClient {BaseAddress = new Uri(_baseUrl)};
                NormalizeClientName(clientName);
                DeriveIdentity();
                LoadAccessTokens();
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new BitPayException(ex);
                }

                throw;
            }
        }

        private void InitKeys() {
            if (KeyUtils.PrivateKeyExists()) {
                _ecKey = KeyUtils.LoadEcKey();

                // TODO: Alternatively, load your private key from a location you specify.
                //_ecKey = KeyUtils.createEcKeyFromHexStringFile("C:\\Users\\Andy\\Documents\\private-key.txt");
            } else {
                _ecKey = KeyUtils.CreateEcKey();
                KeyUtils.SaveEcKey(_ecKey);
            }
        }

        private void DeriveIdentity() {
            // Identity in this implementation is defined to be the SIN.
            Identity = KeyUtils.DeriveSin(_ecKey);
        }

        private void ClearAccessTokenCache() {
            _tokenCache = new Dictionary<string, string>();
        }

        private void CacheToken(string key, string token) {
            _tokenCache.Add(key, token);
            WriteTokenCache();
        }

        private void WriteTokenCache() {
            try {
                using (var fs = File.OpenWrite(TokensFile)) {
                    using (var writer = new StreamWriter(fs)) {
                        var toWrite = "";
                        foreach (var key in _tokenCache.Keys) {
                            toWrite += ";" + key + "=" + _tokenCache[key];
                        }

                        if (!"".Equals(toWrite)) {
                            toWrite = toWrite.Substring(1);
                        }

                        writer.Write(toWrite);
                    }
                }
            } catch (Exception ex) {
                throw new TokensCacheWriteException(ex);
            }
        }

        private void LoadAccessTokens() {
            try {
                ClearAccessTokenCache();
                if (File.Exists(TokensFile)) {
                    using (var fs = File.OpenRead(TokensFile)) {
                        using (var reader = new StreamReader(fs)) {
                            var tokens = reader.ReadToEnd().Split(char.Parse(";"));
                            foreach (var tokenPair in tokens) {
                                var items = tokenPair.Split(char.Parse("="));
                                if (items.Length == 2 && !"".EndsWith(items[1].Trim())) {
                                    _tokenCache.Add(items[0], items[1]);
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                throw new TokensCacheLoadException(ex);
            }
        }

        private string GetAccessToken(string key) {
            if (!_tokenCache.ContainsKey(key))
                throw new TokenNotFoundException(key);
            
            return _tokenCache[key];
        }

        private Dictionary<string, string> GetParams() {
            var parameters = new Dictionary<string, string>();
            return parameters;
        }

        private HttpResponseMessage Get(string uri, Dictionary<string, string> parameters = null) {
            try {
                var fullUrl = _baseUrl + uri;
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-accept-version", BitpayApiVersion);
                _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", BitpayPluginInfo);
                if (parameters != null) {
                    fullUrl += "?";
                    foreach (var entry in parameters) {
                        fullUrl += entry.Key + "=" + entry.Value + "&";
                    }

                    fullUrl = fullUrl.Substring(0, fullUrl.Length - 1);
                    var signature = KeyUtils.Sign(_ecKey, fullUrl);
                    _httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                    _httpClient.DefaultRequestHeaders.Add("x-identity", KeyUtils.BytesToHex(_ecKey.PublicKey));
                }

                var result = _httpClient.GetAsync(fullUrl).Result;
                return result;
            } catch (Exception ex) {
                throw new BitPayApiCommunicationException(ex);
            }
        }

        private HttpResponseMessage Delete(string uri, Dictionary<string, string> parameters = null) {
            try {
                var fullUrl = _baseUrl + uri;
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-accept-version", BitpayApiVersion);
                _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", BitpayPluginInfo);

                if (parameters != null) {
                    fullUrl += "?";
                    foreach (var entry in parameters) {
                        fullUrl += entry.Key + "=" + entry.Value + "&";
                    }

                    fullUrl = fullUrl.Substring(0, fullUrl.Length - 1);
                    var signature = KeyUtils.Sign(_ecKey, fullUrl);
                    _httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                    _httpClient.DefaultRequestHeaders.Add("x-identity", KeyUtils.BytesToHex(_ecKey.PublicKey));
                }

                var result = _httpClient.DeleteAsync(fullUrl).Result;
                return result;
            } catch (Exception ex) {
                throw new BitPayApiCommunicationException(ex);
            }
        }

        private HttpResponseMessage PostWithSignature(string uri, string json) {
            return Post(uri, json, true);
        }

        private HttpResponseMessage Post(string uri, string json, bool signatureRequired = false) {
            try {
                var bodyContent = new StringContent(UnicodeToAscii(json));
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-accept-version", BitpayApiVersion);
                _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", BitpayPluginInfo);
                bodyContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                if (signatureRequired) {
                    var signature = KeyUtils.Sign(_ecKey, _baseUrl + uri + json);
                    _httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                    _httpClient.DefaultRequestHeaders.Add("x-identity", KeyUtils.BytesToHex(_ecKey.PublicKey));
                }

                var result = _httpClient.PostAsync(uri, bodyContent).Result;
                return result;
            } catch (Exception ex) {
                throw new BitPayApiCommunicationException(ex);
            }
        }

        private string ResponseToJsonString(HttpResponseMessage response) {
            if (response == null)
                throw new BitPayApiCommunicationException(new NullReferenceException("Response is null"));

            try {
                // Get the response as a dynamic object for detecting possible error(s) or data object.
                // An error(s) object raises an exception.
                // A data object has its content extracted (throw away the data wrapper object).
                var responseString = response.Content.ReadAsStringAsync().Result;
                var obj = Json.Decode(responseString);

                // Check for error response.
                if (DynamicObjectHasProperty(obj, "error")) {
                    throw new BitPayApiCommunicationException(obj.error.ToString());
                }

                if (DynamicObjectHasProperty(obj, "errors")) {
                    var message = "Multiple errors:";
                    foreach (var errorItem in obj.errors) {
                        message += "\n" + errorItem.error + " " + errorItem.param;
                    }

                    throw new BitPayApiCommunicationException(message);
                }

                // Check for and exclude a "data" object from the response.
                if (DynamicObjectHasProperty(obj, "data")) {
                    responseString = JObject.Parse(responseString).SelectToken("data").ToString();
                }

                return Regex.Replace(responseString, @"\r\n", "");
            } catch (Exception ex) {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException))) {
                    throw new BitPayApiCommunicationException(ex);
                }

                throw;
            }
        }

        private static bool DynamicObjectHasProperty(dynamic obj, string name) {
            var result = false;
            if (obj.GetType() == typeof(DynamicJsonObject)) {
                Dictionary<string, object>.KeyCollection kc = obj.GetDynamicMemberNames();
                result = kc.Contains(name);
            }

            return result;
        }

        //private void IgnoreBadCertificates() {
        //    System.Net.ServicePointManager.ServerCertificateValidationCallback =
        //        new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        //}

        //private bool AcceptAllCertifications(
        //    object sender,
        //    System.Security.Cryptography.X509Certificates.X509Certificate certification,
        //    System.Security.Cryptography.X509Certificates.X509Chain chain,
        //    System.Net.Security.SslPolicyErrors sslPolicyErrors) {
        //    return true;
        //}

        private string UnicodeToAscii(string json) {
            var unicodeBytes = Encoding.Unicode.GetBytes(json);
            var asciiBytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, unicodeBytes);
            var asciiChars = new char[Encoding.ASCII.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            Encoding.ASCII.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            return new string(asciiChars);
        }

        private void NormalizeClientName(string clientName) {
            if (clientName.Equals(BitpayPluginInfo)) {
                clientName += " on " + Environment.MachineName;
            }

            // Eliminate special characters from the client name (used as a token label).  Trim to 60 chars.
            _clientName = new Regex("[^a-zA-Z0-9_ ]").Replace(clientName, "_");
            if (_clientName.Length > 60) {
                _clientName = _clientName.Substring(0, 60);
            }
        }

        //// Deprecate non standard naming
        //[Obsolete("Use AuthorizeClient instead")]
        //public void authorizeClient(string pairingCode) {
        //    AuthorizeClient(pairingCode);
        //}

        //[Obsolete("Use RequestClientAuthorization instead")]
        //public string requestClientAuthorization(string facade) {
        //    return RequestClientAuthorization(facade);
        //}

        //[Obsolete("Use ClientIsAuthorized instead")]
        //public bool clientIsAuthorized(string facade) {
        //    return ClientIsAuthorized(facade);
        //}

        //[Obsolete("Use CreateInvoice instead")]
        //public Invoice createInvoice(Invoice invoice, string facade = FacadePos) {
        //    return CreateInvoice(invoice, facade);
        //}

        //[Obsolete("Use GetInvoice instead")]
        //public Invoice getInvoice(string invoiceId, string facade = FacadePos) {
        //    return GetInvoice(invoiceId, facade);
        //}

        //[Obsolete("Use GetInvoices instead")]
        //public List<Invoice> getInvoices(DateTime dateStart, DateTime dateEnd) {
        //    return GetInvoices(dateStart, dateEnd);
        //}

        //[Obsolete("Use GetRates instead")]
        //public Rates getRates() {
        //    return GetRates();
        //}

        //[Obsolete("Use GetLedger instead")]
        //public Ledger getLedger(string currency, DateTime dateStart, DateTime dateEnd) {
        //    return GetLedger(currency, dateStart, dateEnd);
        //}

        //[Obsolete("Use SubmitPayoutBatchInstead")]
        //public PayoutBatch submitPayoutBatch(PayoutBatch batch) {
        //    return SubmitPayoutBatch(batch);
        //}

        //[Obsolete("USe GetPayoutBatches")]
        //public List<PayoutBatch> getPayoutBatches() {
        //    return GetPayoutBatches();
        //}

        //[Obsolete("Use GetPayoutBatch")]
        //public PayoutBatch getPayoutBatch(string batchId) {
        //    return GetPayoutBatch(batchId);
        //}

        //[Obsolete("Use CancelPayoutBatch")]
        //public PayoutBatch cancelPayoutBatch(string batchId) {
        //    return CancelPayoutBatch(batchId);
        //}

        //[Obsolete("Use GetSettlements")]
        //public List<Settlement> getSettlements(string currency, DateTime dateStart, DateTime dateEnd,
        //    string status = "", int limit = 100, int offset = 0) {
        //    return GetSettlements(currency, dateStart, dateEnd, status, limit, offset);
        //}

        //[Obsolete("USe GetSettlement")]
        //public Settlement getSettlement(string settlementId) {
        //    return GetSettlement(settlementId);
        //}

        //[Obsolete("Use GetSettlementReconciliationReport")]
        //public Settlement getSettlementReconciliationReport(Settlement settlement) {
        //    return GetSettlementReconciliationReport(settlement);
        //}
    }

}