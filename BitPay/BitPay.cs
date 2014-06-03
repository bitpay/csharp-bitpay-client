using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Helpers;

namespace BitPayAPI
{
    /// <summary>
    /// Provides an abstraction of the BitPay server.
    /// </summary>
    public class BitPay
    {

        private static readonly string BASE_URL = "https://test.bitpay.com/";
	
	    private HttpClient client;
        private ECKey privateKey;
        private long nonce;
        private String SIN;

        /// <summary>
        /// Constructor. Baselines the cryptographic key and SIN for all API calls using this instance.
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="SIN"></param>
        public BitPay(ECKey privateKey, String SIN)
        {
            this.SIN = SIN;
            this.nonce = DateTime.Now.Ticks;
            this.privateKey = privateKey;
		    client = new HttpClient();
            client.BaseAddress = new Uri(BASE_URL);
	    }

        public List<AccessKey> getAccessKeys()
        {
            HttpContent response = this.get("keys", this.getParams(), true);
            return createAccessKeyListFromResponse(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountEmail"></param>
        /// <param name="SIN"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public AccessKey submitAccessKey(String accountEmail, String SIN, String label)
        {
            HttpContent response = this.post("keys", this.getParams(accountEmail, SIN, label), false);
            return createAccessKeyObjectFromResponse(response);
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Token> getTokens()
        {
            HttpContent response = this.get("tokens", this.getParams(), true);
            return createTokenListFromResponse(response);
    	}

	    /// <summary>
        /// Creates an invoice using the BitPay Payment Gateway API.
	    /// </summary>
	    /// <param name="price">This is the amount that is required to be collected from the buyer. Note, if this
        /// is specified in a currency other than BTC, the price will be converted into BTC at market exchange
        /// rates to determine the amount collected from the buyer.</param>
	    /// <returns>A BitPay server populated Invoice object.</returns>
        /// <exception cref="BitPayAPI.BitPayException">Handles only errors that occur in the returned data.
        /// Does not handle programming or communication errors.</exception>
        public Invoice createInvoice(double price, string currency)
        {
		    if(currency.Length > 3)
            {
			    throw new ArgumentException("Must be a valid currency code");
		    }

            HttpContent response = this.post("invoices", this.getParams(price, currency), true);
            return createInvoiceObjectFromResponse(response);
	    }

	    /// <summary>
        /// Creates an invoice using the BitPay Payment Gateway API.
	    /// </summary>
        /// <param name="price">This is the amount that is required to be collected from the buyer. Note, if this
        /// is specified in a currency other than BTC, the price will be converted into BTC at market exchange
        /// rates to determine the amount collected from the buyer.</param>
        /// <param name="parameters">Optional payment notification (IPN) parameters.</param>
        /// <returns>A BitPay server populated Invoice object.</returns>
        /// <exception cref="BitPayAPI.BitPayException">Handles only errors that occur in the returned data.
        /// Does not handle programming or communication errors.</exception>
        public Invoice createInvoice(double price, string currency, InvoiceParams parameters)
        {
            if (currency.Length > 3)
            {
                throw new ArgumentException("Must be a valid currency code");
            }

            HttpContent response = this.post("invoices", this.getParams(price, currency, parameters), true);
            return createInvoiceObjectFromResponse(response);
        }

	    /// <summary>
	    /// Get an existing Invoice by it's Id. The Id is used in the url: "https://bitpay.com/invoice?id=<ID>".
	    /// </summary>
	    /// <param name="invoiceId">The Id for the invoice to fetch from the BitPay server.</param>
	    /// <returns>A BitPay server populated Invoice object.</returns>
        /// <exception cref="BitPayAPI.BitPayException">Handles only errors that occur in the returned data.
        /// Does not handle programming or communication errors.</exception>
        public Invoice getInvoice(string invoiceId)
        {
            string url = BASE_URL + "invoices/" + invoiceId;
            client.DefaultRequestHeaders.Add("X-BitPay-Plugin-Info", "CSharplib");

            var result = client.GetAsync(url).Result;
            HttpContent response = result.Content;

            return createInvoiceObjectFromResponse(response);
        }

	    /// <summary>
        /// Get the current Bitcoin Exchange rates in dozens of currencies based on several exchanges.
	    /// </summary>
	    /// <returns>A BitPay server populated Rates object.</returns>
        public Rates getRates()
        {
            string url = BASE_URL + "rates";
            var result = client.GetAsync(url).Result;
            HttpContent response = result.Content;

            return new Rates(response, this);
        }

        private Dictionary<string, string> getParams()
        {
            return new Dictionary<string, string>();
        }

        private Dictionary<string, string> getParams(string email, string SIN, string label)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("sin", SIN);
            parameters.Add("email", email);
            parameters.Add("label", label);
            return parameters;
        }

        /// <summary>
        /// Creates a list of key/value parameters.
        /// </summary>
        /// <param name="price">The invoice price.</param>
        /// <param name="currency">The invoice currency.</param>
        /// <returns>A list of key/value pairs.</returns>
	    private Dictionary<string, string> getParams(double price, String currency)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
		    parameters.Add("price", price + "");
		    parameters.Add("currency", currency);
		    return parameters;
	    }

        /// <summary>
        /// Creates a list of key/value parameters including optional API parameters.
        /// </summary>
        /// <param name="price">The invoice price.</param>
        /// <param name="currency">The invoice currency.</param>
        /// <param name="optionalParams">A populated InvoiceParams object.</param>
        /// <returns>A list of key/value pairs.</returns>
        private Dictionary<string, string> getParams(double price, string currency, InvoiceParams invoiceParams)
        {
            var parameters = invoiceParams.getDictionary();
            parameters.Add("price", price.ToString());
            parameters.Add("currency", currency);
		    return parameters;
	    }

        private String signData(String url)
        {
            return KeyUtils.signString(privateKey, url);
        }

        private String signData(String url, Dictionary<string, string> body)
        {
            String data = url + "{";
            foreach (KeyValuePair<string, string> entry in body)
            {
                data += '"' + entry.Key + "\":\"" + entry.Value + "\",";
            }
            data = data.Substring(0, data.Length - 1);
            data += "}";
            return KeyUtils.signString(privateKey, data);
        }

        private HttpContent get(String uri, Dictionary<string, string> parameters, bool shouldSignData)
        {
            try
            {
                parameters.Add("nonce", this.nonce + "");
                this.nonce++;

                String fullURL = BASE_URL + uri + "?";
		        foreach (KeyValuePair<string, string> entry in parameters)
                {
			        fullURL += entry.Key + "=" + entry.Value + "&";
		        }
		        fullURL = fullURL.Substring(0, fullURL.Length - 1);

                if (shouldSignData)
                {
                    String signature = signData(fullURL);
                    client.DefaultRequestHeaders.Add("X-signature", signature);
                    client.DefaultRequestHeaders.Add("X-pubkey", KeyUtils.bytesToHex(privateKey.pubKey));
                }

                client.DefaultRequestHeaders.Add("X-BitPay-Plugin-Info", "CSharplib");
                var result = client.GetAsync(fullURL).Result;
                HttpContent response = result.Content;
                return response;
            }
            catch (Exception e)
            {
                Console.Out.Write(e.ToString());
            }

            return null;
        }

        private HttpContent post(String uri, Dictionary<string, string> parameters, bool shouldSignData)
        {
            try
            {
                parameters.Add("guid", Guid.NewGuid().ToString());
                parameters.Add("nonce", this.nonce + "");
                this.nonce++;

                var content = new FormUrlEncodedContent(parameters);

                if (shouldSignData)
                {
                    String signature = signData(BASE_URL + uri, parameters);
                    client.DefaultRequestHeaders.Add("X-signature", signature);
                    client.DefaultRequestHeaders.Add("X-pubkey", KeyUtils.bytesToHex(privateKey.pubKey));
                }

                client.DefaultRequestHeaders.Add("X-BitPay-Plugin-Info", "CSharplib");
                var result = client.PostAsync(uri, content).Result;
                HttpContent response = result.Content;
                return response;
            }
            catch (Exception e)
            {
                Console.Out.Write(e.ToString());
            }

            return null;
        }

        /// <summary>
        /// Determines whether or not the given dynamic object key collection includes the specified member name.
        /// </summary>
        /// <param name="obj">Expected to be a JSON decoded object.</param>
        /// <param name="name">The name of a key in the JSON object.</param>
        /// <returns></returns>
        private static bool dynamicObjectHasProperty(dynamic obj, string name)
        {
            Dictionary<string, object>.KeyCollection kc = obj.GetDynamicMemberNames();
            return kc.Contains(name);
        }

        private List<Token> createTokenListFromResponse(HttpContent response)
        {
            if (response == null)
            {
                return null;
            }

            dynamic obj = Json.Decode(response.ReadAsStringAsync().Result);
            if (dynamicObjectHasProperty(obj, "error"))
            {
                throw new BitPayException("Error: " + obj.error);
            }

            // Sample JSON response
            // {
            //   "data":
            //     [
            //       {"merchant": "5PAgL9amg5EgEzrvR2NBvwprC2XEVvpFQoLpQVu9x3Ck"}
            //     ]
            // }
            List<Token> tokens = new List<Token>();

            for (int i = 0; i < obj.data.Length; i++)
            {
                Token t = new Token();
                t.updateWithObject(obj.data[i]);
                tokens.Add(t);
            }

            return tokens;
        }

        private List<AccessKey> createAccessKeyListFromResponse(HttpContent response)
        {
            if (response == null)
            {
                return null;
            }

            dynamic obj = Json.Decode(response.ReadAsStringAsync().Result);
            if (dynamicObjectHasProperty(obj, "error"))
            {
                throw new BitPayException("Error: " + obj.error);
            }

            // Sample JSON response
            // {
            //     "facade": "user/sin",
            //     "data": [
            //         {
            //             "id": "Teys7dby6EXdxDGnypFozhtMbvYNydxbaXf",
            //             "label": "CSharp API Tester",
            //             "approved": true,
            //             "token": "2XmXFUpsApquF83qNWHVsDF2DoZ1iTDPCao4hMMLgTfjKgznHU9QnTJh9RJqVq2Lkk"
            //         }
            //     ]
            // }
            List<AccessKey> accessKeys = new List<AccessKey>();

            for (int i = 0; i < obj.data.Length; i++)
            {
                AccessKey k = new AccessKey();
                k.updateWithObject(obj.data[i]);
                accessKeys.Add(k);
            }

            return accessKeys;
        }

        private AccessKey createAccessKeyObjectFromResponse(HttpContent response)
        {
            if (response == null)
            {
                return null;
            }

            dynamic obj = Json.Decode(response.ReadAsStringAsync().Result);
            if (dynamicObjectHasProperty(obj, "error"))
            {
                throw new BitPayException("Error: " + obj.error);
            }

            AccessKey accessKey = new AccessKey();
            return accessKey.updateWithObject(obj);
        }

        /// <summary>
        /// Creates an invoice object given the BitPay server response.
        /// Throws a BitPayException if the content of the response indicates an error occurred.
        /// </summary>
        /// <param name="response">The HTTP response object from the BitPay server when attempting to create
        /// an invoice.</param>
        /// <returns>A populated Inovice object.</returns>
	    private Invoice createInvoiceObjectFromResponse(HttpContent response)
        {
            dynamic obj = Json.Decode(response.ReadAsStringAsync().Result);
            if (dynamicObjectHasProperty(obj, "error"))
            {
                throw new BitPayException("Error: " + obj.error);
            }

            return new Invoice(obj);
	    }

    }
}
