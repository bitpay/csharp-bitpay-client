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

        private readonly HttpClient _httpClient = null;
        private readonly string _baseUrl;
        private EcKey _ecKey;
        private readonly string _clientName = "";
        private Dictionary<string, string> _tokenCache; // {facade, token}

        /// <summary>
        /// Constructor for use if the keys and SIN are managed by this library.
        /// </summary>
        /// <param name="clientName">The label for this client.</param>
        /// <param name="envUrl">The target server URL.</param>
        public BitPay(string clientName = BitpayPluginInfo, string envUrl = BitpayUrl) {
            // IgnoreBadCertificates();

            if (clientName.Equals(BitpayPluginInfo)) {
                clientName += " on " + Environment.MachineName;
            }

            // Eliminate special characters from the client name (used as a token label).  Trim to 60 chars.
            _clientName = new Regex("[^a-zA-Z0-9_ ]").Replace(clientName, "_");
            if (_clientName.Length > 60) {
                _clientName = _clientName.Substring(0, 60);
            }

            _baseUrl = envUrl;
            _httpClient = new HttpClient {BaseAddress = new Uri(_baseUrl)};

            InitKeys();
            DeriveIdentity();
            GetAccessTokens();

        }

        /// <summary>
        /// Constructor for use if the keys and SIN were derived external to this library.
        /// </summary>
        /// <param name="ecKey">An elliptical curve key.</param>
        /// <param name="clientName">The label for this client.</param>
        /// <param name="envUrl">The target server URL.</param>
        public BitPay(EcKey ecKey, string clientName = BitpayPluginInfo, string envUrl = BitpayUrl) {
            // IgnoreBadCertificates();

            _ecKey = ecKey;
            _baseUrl = envUrl;
            _httpClient = new HttpClient {BaseAddress = new Uri(_baseUrl)};

            DeriveIdentity();
            GetAccessTokens();

        }

        /// <summary>
        /// Return the identity of this client.
        /// </summary>
        public string Identity { get; private set; }

        /// <summary>
        /// Authorize (pair) this client with the server using the specified pairing code.
        /// </summary>
        /// <param name="pairingCode">A code obtained from the server; typically from bitpay.com/api-tokens.</param>
        public void authorizeClient(string pairingCode) {
            Token token = new Token();
            token.Id = Identity;
            token.Guid = Guid.NewGuid().ToString();
            token.PairingCode = pairingCode;
            token.Label = _clientName;
            string json = JsonConvert.SerializeObject(token);
            HttpResponseMessage response = Post("tokens", json);
            List<Token> tokens = JsonConvert.DeserializeObject<List<Token>>(ResponseToJsonString(response));
            foreach (Token t in tokens) {
                CacheToken(t.Facade, t.Value);
            }
        }

        /// <summary>
        /// Request authorization (a token) for this client in the specified facade.
        /// </summary>
        /// <param name="facade">The facade for which authorization is requested.</param>
        /// <returns>A pairing code for this client.  This code must be used to authorize this client at BitPay.com/api-tokens.</returns>
        public string requestClientAuthorization(string facade) {
            Token token = new Token();
            token.Id = Identity;
            token.Guid = Guid.NewGuid().ToString();
            token.Facade = facade;
            token.Count = 1;
            token.Label = _clientName;
            string json = JsonConvert.SerializeObject(token);
            HttpResponseMessage response = Post("tokens", json);
            List<Token> tokens = JsonConvert.DeserializeObject<List<Token>>(ResponseToJsonString(response));
            // Expecting a single token resource.
            if (tokens.Count != 1) {
                throw new BitPayException("Error - failed to get token resource; expected 1 token, got " +
                                          tokens.Count);
            }

            CacheToken(tokens[0].Facade, tokens[0].Value);
            return tokens[0].PairingCode;
        }

        /// <summary>
        /// Specified whether the client has authorization (a token) for the specified facade.
        /// </summary>
        /// <param name="facade">The facade name for which authorization is tested.</param>
        /// <returns></returns>
        public bool clientIsAuthorized(string facade) {
            return _tokenCache.ContainsKey(facade);
        }

        /// <summary>
        /// Create an invoice using the specified facade.
        /// </summary>
        /// <param name="invoice">An invoice request object.</param>
        /// <returns>A new invoice object returned from the server.</returns>
        public Invoice createInvoice(Invoice invoice, string facade = FacadePos) {
            invoice.Token = GetAccessToken(facade);
            invoice.Guid = Guid.NewGuid().ToString();
            string json = JsonConvert.SerializeObject(invoice);
            HttpResponseMessage response = PostWithSignature("invoices", json);
            JsonConvert.PopulateObject(ResponseToJsonString(response), invoice);

            // Track the token for this invoice
            CacheToken(invoice.Id, invoice.Token);

            return invoice;
        }

        /// <summary>
        /// Retrieve an invoice by id and token.
        /// </summary>
        /// <param name="invoiceId">The id of the requested invoice.</param>
        /// <returns>The invoice object retrieved from the server.</returns>
        public Invoice getInvoice(string invoiceId, string facade = FacadePos) {
            // Provide the merchant token when the merchant facade is being used.
            // GET/invoices expects the merchant token and not the merchant/invoice token.
            Dictionary<string, string> parameters = null;
            if (facade == FacadeMerchant) {
                try {
                    parameters = new Dictionary<string, string>();
                    parameters.Add("token", GetAccessToken(FacadeMerchant));
                } catch (BitPayException) {
                    // No token for invoice.
                    parameters = null;
                }
            }

            HttpResponseMessage response = Get("invoices/" + invoiceId, parameters);
            return JsonConvert.DeserializeObject<Invoice>(ResponseToJsonString(response));
        }

        /// <summary>
        /// Retrieve a list of invoices by date range using the merchant facade.
        /// </summary>
        /// <param name="dateStart">The start date for the query.</param>
        /// <param name="dateEnd">The end date for the query.</param>
        /// <returns>A list of invoice objects retrieved from the server.</returns>
        public List<Invoice> getInvoices(DateTime dateStart, DateTime dateEnd) {
            Dictionary<string, string> parameters = GetParams();
            parameters.Add("token", GetAccessToken(FacadeMerchant));
            parameters.Add("dateStart", dateStart.ToShortDateString());
            parameters.Add("dateEnd", dateEnd.ToShortDateString());
            HttpResponseMessage response = Get("invoices", parameters);
            return JsonConvert.DeserializeObject<List<Invoice>>(ResponseToJsonString(response));
        }

        /// <summary>
        /// Retrieve the exchange rate table using the public facade.
        /// </summary>
        /// <returns>The rate table as an object retrieved from the server.</returns>
        public Rates getRates() {
            HttpResponseMessage response = Get("rates");
            List<Rate> rates = JsonConvert.DeserializeObject<List<Rate>>(ResponseToJsonString(response));
            return new Rates(rates, this);
        }

        /// <summary>
        /// Retrieve a list of ledgers by date range using the merchant facade.
        /// </summary>
        /// <param name="currency">The three digit currency string for the ledger to retrieve.</param>
        /// <param name="dateStart">The start date for the query.</param>
        /// <param name="dateEnd">The end date for the query.</param>
        /// <returns>A list of invoice objects retrieved from the server.</returns>
        public Ledger getLedger(string currency, DateTime dateStart, DateTime dateEnd) {
            Dictionary<string, string> parameters = GetParams();
            parameters.Add("token", GetAccessToken(FacadeMerchant));
            parameters.Add("startDate", "" + dateStart.ToShortDateString());
            parameters.Add("endDate", "" + dateEnd.ToShortDateString());
            HttpResponseMessage response = Get("ledgers/" + currency, parameters);
            List<LedgerEntry> entries =
                JsonConvert.DeserializeObject<List<LedgerEntry>>(ResponseToJsonString(response));
            return new Ledger(entries);
        }

        /// <summary>
        /// Submit a BitPay Payout batch.
        /// </summary>
        /// <param name="batch">A PayoutBatch object with request parameters defined.</param>
        /// <returns>A BitPay generated PayoutBatch object.</param>
        public PayoutBatch submitPayoutBatch(PayoutBatch batch) {
            batch.Token = GetAccessToken(FacadePayroll);
            batch.Guid = Guid.NewGuid().ToString();
            string json = JsonConvert.SerializeObject(batch);
            HttpResponseMessage response = PostWithSignature("payouts", json);

            // To avoid having to merge instructions in the response with those we sent, we just remove the instructions
            // we sent and replace with those in the response.
            batch.Instructions = new List<PayoutInstruction>();

            JsonConvert.PopulateObject(ResponseToJsonString(response), batch, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore
            });

            // Track the token for this batch
            CacheToken(batch.Id, batch.Token);

            return batch;
        }

        /// <summary>
        /// Retrieve a collection of BitPay payout batches.
        /// </summary>
        /// <returns>A list of BitPay PayoutBatch objects.</param>
        public List<PayoutBatch> getPayoutBatches() {
            Dictionary<string, string> parameters = GetParams();
            parameters.Add("token", GetAccessToken(FacadePayroll));
            HttpResponseMessage response = Get("payouts", parameters);
            return JsonConvert.DeserializeObject<List<PayoutBatch>>(ResponseToJsonString(response),
                new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Retrieve a BitPay payout batch by batch id using.  The client must have been previously authorized for the payroll facade.
        /// </summary>
        /// <param name="batchId">The id of the batch to retrieve.</param>
        /// <returns>A BitPay PayoutBatch object.</param>
        public PayoutBatch getPayoutBatch(string batchId) {
            Dictionary<string, string> parameters = null;
            try {
                parameters = new Dictionary<string, string>();
                parameters.Add("token", GetAccessToken(FacadePayroll));
            } catch (BitPayException) {
                // No token for batch.
                parameters = null;
            }

            HttpResponseMessage response = Get("payouts/" + batchId, parameters);
            return JsonConvert.DeserializeObject<PayoutBatch>(ResponseToJsonString(response),
                new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Cancel a BitPay Payout batch.
        /// </summary>
        /// <param name="batchId">The id of the batch to cancel.</param>
        /// <returns> A BitPay generated PayoutBatch object.</param>
        public PayoutBatch cancelPayoutBatch(string batchId) {
            PayoutBatch b = getPayoutBatch(batchId);

            Dictionary<string, string> parameters = null;
            try {
                parameters = new Dictionary<string, string>();
                parameters.Add("token", b.Token);
            } catch (BitPayException) {
                // No token for batch.
                parameters = null;
            }

            HttpResponseMessage response = Delete("payouts/" + batchId, parameters);
            return JsonConvert.DeserializeObject<PayoutBatch>(ResponseToJsonString(response),
                new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                });
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
        public List<Settlement> getSettlements(string currency, DateTime dateStart, DateTime dateEnd,
            string status = "", int limit = 100, int offset = 0) {
            var parameters = new Dictionary<string, string> {
                {"token", GetAccessToken(FacadeMerchant)},
                {"startDate", $"{dateStart.ToShortDateString()}"},
                {"endDate", $"{dateEnd.ToShortDateString()}"},
                {"currency", currency},
                {"status", status},
                {"limit", $"{limit}"},
                {"offset", $"{offset}"}
            };

            HttpResponseMessage response = Get("settlements", parameters);
            return JsonConvert.DeserializeObject<List<Settlement>>(ResponseToJsonString(response));
        }

        /// <summary>
        /// Retrieves a summary of the specified settlement.
        /// </summary>
        /// <param name="settlementId">Settlement Id</param>
        /// <returns>A BitPay Settlement object.</returns>
        public Settlement getSettlement(string settlementId) {
            var parameters = new Dictionary<string, string> {
                {"token", GetAccessToken(FacadeMerchant)}
            };

            HttpResponseMessage response = Get($"settlements/{settlementId}", parameters);
            return JsonConvert.DeserializeObject<Settlement>(ResponseToJsonString(response));
        }

        /// <summary>
        /// Gets a detailed reconciliation report of the activity within the settlement period
        /// </summary>
        /// <param name="settlement">Settlement to generate report for.</param>
        /// <returns>A detailed BitPay Settlement object.</returns>
        public Settlement getSettlementReconciliationReport(Settlement settlement) {
            var parameters = new Dictionary<string, string> {
                {"token", settlement.Token}
            };

            HttpResponseMessage response = Get($"settlements/{settlement.Id}/reconciliationReport", parameters);
            return JsonConvert.DeserializeObject<Settlement>(ResponseToJsonString(response));
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void InitKeys() {
            if (KeyUtils.privateKeyExists()) {
                _ecKey = KeyUtils.loadEcKey();

                // Alternatively, load your private key from a location you specify.
                //_ecKey = KeyUtils.createEcKeyFromHexStringFile("C:\\Users\\Andy\\Documents\\private-key.txt");
            } else {
                _ecKey = KeyUtils.createEcKey();
                KeyUtils.saveEcKey(_ecKey);
            }
        }

        private void DeriveIdentity() {
            // Identity in this implementation is defined to be the SIN.
            Identity = KeyUtils.deriveSIN(_ecKey);
        }

        private void ClearAccessTokenCache() {
            _tokenCache = new Dictionary<string, string>();
        }

        private void CacheToken(string key, string token) {
            _tokenCache.Add(key, token);
            WriteTokenCache();
        }

        private void WriteTokenCache() {
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
        }

        private int GetAccessTokens() {
            ClearAccessTokenCache();
            if (File.Exists(TokensFile)) {
                using (FileStream fs = File.OpenRead(TokensFile)) {
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

            return _tokenCache.Count;
        }

        private string GetAccessToken(string key) {
            if (!_tokenCache.ContainsKey(key)) {
                throw new BitPayException("Error: You do not have access to facade: " + key);
            }

            return _tokenCache[key];
        }

        private Dictionary<string, string> GetParams() {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            return parameters;
        }

        private HttpResponseMessage Get(string uri, Dictionary<string, string> parameters = null) {
            try {
                string fullURL = _baseUrl + uri;
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-accept-version", BitpayApiVersion);
                _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", BitpayPluginInfo);
                if (parameters != null) {
                    fullURL += "?";
                    foreach (KeyValuePair<string, string> entry in parameters) {
                        fullURL += entry.Key + "=" + entry.Value + "&";
                    }

                    fullURL = fullURL.Substring(0, fullURL.Length - 1);
                    string signature = KeyUtils.sign(_ecKey, fullURL);
                    _httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                    _httpClient.DefaultRequestHeaders.Add("x-identity", KeyUtils.bytesToHex(_ecKey.PubKey));
                }

                var result = _httpClient.GetAsync(fullURL).Result;
                return result;
            } catch (Exception ex) {
                throw new BitPayException("Error: " + ex.ToString());
            }
        }

        private HttpResponseMessage Delete(string uri, Dictionary<string, string> parameters = null) {
            try {
                string fullURL = _baseUrl + uri;
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-accept-version", BitpayApiVersion);
                _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", BitpayPluginInfo);

                if (parameters != null) {
                    fullURL += "?";
                    foreach (KeyValuePair<string, string> entry in parameters) {
                        fullURL += entry.Key + "=" + entry.Value + "&";
                    }

                    fullURL = fullURL.Substring(0, fullURL.Length - 1);
                    string signature = KeyUtils.sign(_ecKey, fullURL);
                    _httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                    _httpClient.DefaultRequestHeaders.Add("x-identity", KeyUtils.bytesToHex(_ecKey.PubKey));
                }

                var result = _httpClient.DeleteAsync(fullURL).Result;
                return result;
            } catch (Exception ex) {
                throw new BitPayException("Error: " + ex.ToString());
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
                    string signature = KeyUtils.sign(_ecKey, _baseUrl + uri + json);
                    _httpClient.DefaultRequestHeaders.Add("x-signature", signature);
                    _httpClient.DefaultRequestHeaders.Add("x-identity", KeyUtils.bytesToHex(_ecKey.PubKey));
                }

                var result = _httpClient.PostAsync(uri, bodyContent).Result;
                return result;
            } catch (Exception ex) {
                throw new BitPayException("Error: " + ex.ToString());
            }
        }

        private string ResponseToJsonString(HttpResponseMessage response) {
            if (response == null) {
                throw new BitPayException("Error: HTTP response is null");
            }

            // Get the response as a dynamic object for detecting possible error(s) or data object.
            // An error(s) object raises an exception.
            // A data object has its content extracted (throw away the data wrapper object).
            string responseString = response.Content.ReadAsStringAsync().Result;
            dynamic obj = Json.Decode(responseString);

            // Check for error response.
            if (DynamicObjectHasProperty(obj, "error")) {
                throw new BitPayException("Error: " + obj.error);
            }

            if (DynamicObjectHasProperty(obj, "errors")) {
                string message = "Multiple errors:";
                foreach (var errorItem in obj.errors) {
                    message += "\n" + errorItem.error + " " + errorItem.param;
                }

                throw new BitPayException(message);
            }

            // Check for and exclude a "data" object from the response.
            if (DynamicObjectHasProperty(obj, "data")) {
                responseString = JObject.Parse(responseString).SelectToken("data").ToString();
            }

            return Regex.Replace(responseString, @"\r\n", "");
        }

        private static bool DynamicObjectHasProperty(dynamic obj, string name) {
            bool result = false;
            if (obj.GetType() == typeof(DynamicJsonObject)) {
                Dictionary<string, object>.KeyCollection kc = obj.GetDynamicMemberNames();
                result = kc.Contains(name);
            }

            return result;
        }

        private void IgnoreBadCertificates() {
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        }

        private bool AcceptAllCertifications(
            object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certification,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors) {
            return true;
        }

        private string UnicodeToAscii(string json) {
            byte[] unicodeBytes = Encoding.Unicode.GetBytes(json);
            byte[] asciiBytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, unicodeBytes);
            char[] asciiChars = new char[Encoding.ASCII.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            Encoding.ASCII.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            return new string(asciiChars);
        }
    }
}
