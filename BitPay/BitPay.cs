using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Helpers;

using System.Web.Script.Serialization;

/**
 * @author Andy Phillipson
 * @date 6.4.2014
 * 
 * Wrapper for BitPay's *new* BitAuth API.
 * 
 * In order to authenticate with the new API, you must generate a
 * public/private key pair using ECDSA curve secp256k1 and derive 
 * your SIN. The SIN is defined as the base58 check representation 
 * of: 	0x0F + 0x02 + RIPEMD-160(SHA-256(public key)), and using
 * the submiyKey method, then approve your key at 
 * bitpay.com/key-manager.
 * 
 * See bitpay.com/api for more information.
 */

namespace BitPayAPI
{

    public class BitPay
    {

        private static readonly String BASE_URL = "https://test.bitpay.com/";
	    private HttpClient client;
        private ECKey privateKey;
        private long nonce;
        private String SIN;
        private String merchantToken;

        public BitPay(ECKey privateKey, String SIN)
        {
            this.SIN = SIN;
            this.nonce = DateTime.Now.Ticks / 1000;
            this.privateKey = privateKey;
		    client = new HttpClient();
            client.BaseAddress = new Uri(BASE_URL);
            this.merchantToken = this.getToken(this.getTokens(), "merchant");
	    }

        public Key submitKey(String accountEmail, String SIN, String label)
        {
            Dictionary<String, String> parameters = this.getParams(accountEmail, SIN, label);
            HttpContent response = this.post("keys", parameters, false);
            return responseToKey(response);
        }

        public List<Key> getKeys()
        {
            Dictionary<String, String> parameters = this.getParams();
            HttpContent response = this.get("keys", parameters);
            return responseToKeyList(response);
        }

        public List<Token> getTokens()
        {
            Dictionary<String, String> parameters = this.getParams();
            HttpContent response = this.get("tokens", parameters);
            return responseToTokenList(response);
        }

        public String getToken(List<Token> tokens, String key) 
        {
            string tokenValue = "";
            foreach (Token t in tokens)
            {
                if (t.facade.Equals(key))
                {
                    tokenValue = t.token;
                }
            }
            return tokenValue;
	    }

        public Invoice createInvoice(double price, String currency)
        {
		    if(currency.Length > 3)
            {
			    throw new ArgumentException("Must be a valid currency code");
		    }
            Dictionary<String, String> parameters = this.getParams(price, currency);
            parameters.Add("token", this.merchantToken);
            HttpContent response = this.post("invoices", parameters, true);
            return responseToInvoice(response);
	    }

        public Invoice createInvoice(double price, String currency, InvoiceParams invoiceParams)
        {
            if (currency.Length > 3)
            {
                throw new ArgumentException("Must be a valid currency code");
            }
            Dictionary<String, String> parameters = this.getParams(price, currency, invoiceParams);
            parameters.Add("token", this.merchantToken);
            HttpContent response = this.post("invoices", parameters, true);
            return responseToInvoice(response);
        }

        public Invoice getInvoice(String invoiceId)
        {
            Dictionary<String, String> parameters = this.getParams();
            parameters.Add("token", this.merchantToken);
            HttpContent response = this.get("invoices/" + invoiceId, parameters);
            return responseToInvoice(response);
        }

        public List<Invoice> getInvoices(String javascriptDateString)
        {
            Dictionary<String, String> parameters = this.getParams();
            parameters.Add("dateStart", javascriptDateString);
            parameters.Add("token", this.merchantToken);
            HttpContent response = this.get("invoices", parameters);
            return responseToInvoiceList(response);
        }

        public Rates getRates()
        {
            HttpContent response = this.get("rates");
            return responseToRatesObject(response);
        }

        private Dictionary<string, string> getParams()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("nonce", this.nonce + "");
            this.nonce++;
            return parameters;
        }

        private Dictionary<string, string> getParams(string email, string SIN, string label)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("nonce", this.nonce + "");
            parameters.Add("sin", SIN);
            parameters.Add("email", email);
            parameters.Add("label", label);
            this.nonce++;
            return parameters;
        }

	    private Dictionary<string, string> getParams(double price, String currency)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("nonce", this.nonce + "");
            parameters.Add("price", price + "");
            parameters.Add("currency", currency);
            this.nonce++;
            return parameters;
	    }

        private Dictionary<string, string> getParams(double price, string currency, InvoiceParams invoiceParams)
        {
            var parameters = invoiceParams.getDictionary();
            parameters.Add("nonce", this.nonce + "");
            parameters.Add("price", price.ToString());
            parameters.Add("currency", currency);
            this.nonce++;
            return parameters;
	    }

        private HttpContent get(String uri)
        {
            try
            {
                String fullURL = BASE_URL + uri;
                client.DefaultRequestHeaders.Clear();
                var result = client.GetAsync(fullURL).Result;
                HttpContent response = result.Content;
                return response;
            }
            catch (Exception e)
            {
                throw new BitPayException("Error: " + e.ToString());
            }
        }

        private HttpContent get(String uri, Dictionary<string, string> parameters)
        {
            try
            {
                String fullURL = BASE_URL + uri + "?";
		        foreach (KeyValuePair<string, string> entry in parameters)
                {
			        fullURL += entry.Key + "=" + entry.Value + "&";
		        }
		        fullURL = fullURL.Substring(0, fullURL.Length - 1);
                String signature = KeyUtils.signString(privateKey, fullURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("X-signature", signature);
                client.DefaultRequestHeaders.Add("X-pubkey", KeyUtils.bytesToHex(privateKey.pubKey));
                client.DefaultRequestHeaders.Add("X-BitPay-Plugin-Info", "CSharplib");
                var result = client.GetAsync(fullURL).Result;
                HttpContent response = result.Content;
                return response;
            }
            catch (Exception e)
            {
                throw new BitPayException("Error: " + e.ToString());
            }
        }

        private HttpContent post(String uri, Dictionary<string, string> parameters, bool signatureRequired)
        {
            try
            {
                parameters.Add("guid", Guid.NewGuid().ToString());
                string json = new JavaScriptSerializer().Serialize(parameters);
                var bodyContent = new StringContent(json);
                client.DefaultRequestHeaders.Clear();
                if (signatureRequired)
                {
                    String signature = KeyUtils.signString(privateKey, BASE_URL + uri + json);
                    client.DefaultRequestHeaders.Add("X-signature", signature);
                    client.DefaultRequestHeaders.Add("X-pubkey", KeyUtils.bytesToHex(privateKey.pubKey));
                }
                client.DefaultRequestHeaders.Add("X-BitPay-Plugin-Info", "CSharplib");
                bodyContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = client.PostAsync(uri, bodyContent).Result;
                HttpContent response = result.Content;
                return response;
            }
            catch (Exception e)
            {
                throw new BitPayException("Error: " + e.ToString());
            }
        }

        private static bool dynamicObjectHasProperty(dynamic obj, string name)
        {
            Dictionary<string, object>.KeyCollection kc = obj.GetDynamicMemberNames();
            return kc.Contains(name);
        }

        private dynamic responseToObject(HttpContent response)
        {
            if (response == null)
            {
                throw new BitPayException("Error: HTTP response is null");
            }
            dynamic obj = Json.Decode(response.ReadAsStringAsync().Result);
            if (dynamicObjectHasProperty(obj, "error"))
            {
                throw new BitPayException("Error: " + obj.error);
            }
            return obj;
        }

        private List<Token> responseToTokenList(HttpContent response)
        {
            dynamic obj = responseToObject(response);
            List<Token> tokens = new List<Token>();
            for (int i = 0; i < obj.data.Length; i++)
            {
                Token token = new Token(obj.data[i]);
                tokens.Add(token);
            }
            return tokens;
        }

        private List<Key> responseToKeyList(HttpContent response)
        {
            dynamic obj = responseToObject(response);
            List<Key> keys = new List<Key>();
            for (int i = 0; i < obj.data.Length; i++)
            {
                Key key = new Key(obj.data[i]);
                key.facade = obj.facade;
                keys.Add(key);
            }
            return keys;
        }

        private Key responseToKey(HttpContent response)
        {
            dynamic obj = this.responseToObject(response);
            Key key = new Key(obj.data);
            key.facade = obj.facade;
            return key;
        }

	    private Invoice responseToInvoice(HttpContent response)
        {
            dynamic obj = this.responseToObject(response);
            Invoice invoice = new Invoice(obj.data);
            invoice.facade = obj.facade;
            return invoice;
	    }

        private List<Invoice> responseToInvoiceList(HttpContent response)
        {
            dynamic obj = responseToObject(response);
            List<Invoice> invoices = new List<Invoice>();
            for (int i = 0; i < obj.data.Length; i++)
            {
                Invoice invoice = new Invoice(obj.data[i]);
                invoices.Add(invoice);
            }
            return invoices;
        }

        private Rates responseToRatesObject(HttpContent response)
        {
            dynamic obj = responseToObject(response);
            return new Rates(obj.data, this);
        }

    }
}
