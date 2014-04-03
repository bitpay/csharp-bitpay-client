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
        private static readonly string BASE_URL = "https://bitpay.com/api/";
	
	    private string apiKey;
	    private HttpClient client;
	    private string auth;

        /// <summary>
        /// Constructor. Baselines the API key and currencies for all invoices created using this instance.
        /// </summary>
        /// <param name="apiKey">Your API access key as defined at https://bitpay.com/api-keys. </param>
        /// <param name="currency">This is the currency code set for the price setting.  The pricing currencies
        /// currently supported are USD, EUR, BTC, and all of the codes listed on this page:
        /// https://bitpay.com/bitcoin­exchange­rates. </param>
	    public BitPay(string apiKey) {
		    this.apiKey = apiKey;
            byte[] encodedByte = System.Text.ASCIIEncoding.ASCII.GetBytes(this.apiKey + ": ");
            this.auth = Convert.ToBase64String(encodedByte);
		    client = new HttpClient();
            client.BaseAddress = new Uri(BASE_URL);
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
		    if(currency.Length > 3) {
			    throw new ArgumentException("Must be a valid currency code");
		    }

            var content = new FormUrlEncodedContent(this.getParams(price, currency));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", this.auth);
            client.DefaultRequestHeaders.Add("X-BitPay-Plugin-Info", "CSharplib");

            var result = client.PostAsync("invoice", content).Result;
            HttpContent response = result.Content;

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

            var content = new FormUrlEncodedContent(this.getParams(price, currency, parameters));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", this.auth);
            client.DefaultRequestHeaders.Add("X-BitPay-Plugin-Info", "CSharplib");

            var result = client.PostAsync("invoice", content).Result;
            HttpContent response = result.Content;

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
            string url = BASE_URL + "invoice/" + invoiceId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", this.auth);
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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", this.auth);

            var result = client.GetAsync(url).Result;
            HttpContent response = result.Content;

            return new Rates(response, this);
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
                throw new BitPayException("Error: " + obj.error.message);
            }

            return new Invoice(obj);
	    }
    }
}
