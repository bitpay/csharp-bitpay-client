using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BitPay.Exceptions;
using Newtonsoft.Json.Linq;

namespace BitPay.Utils
{
    public class HttpResponseParser
    {
        public static async Task<string> ResponseToJsonString(HttpResponseMessage response)
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
                JToken code;

                if (jObj.TryGetValue("status", out value))
                {
                   if (value.ToString().Equals("error"))
                   {
                       jObj.TryGetValue("code", out code);
                       jObj.TryGetValue("message", out value);
                       throw new BitPayApiCommunicationException(code.ToString(), value.ToString());
                   }
                }

                // Check for error response.
                if (jObj.TryGetValue("error", out value))
                {
                    throw new BitPayApiCommunicationException(value.ToString());
                }
                
                if (jObj.TryGetValue("status", out value) && value.ToString() == "error")
                {
                    if (jObj.TryGetValue("message", out value)) throw new BitPayApiCommunicationException(value.ToString());
                }

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

                if (jObj.ContainsKey("status") && jObj.ContainsKey("data"))
                {
                    if(jObj.TryGetValue("data", out value))
                    {
                        if (value.ToString() == "{}") return Regex.Replace(responseString, @"\r\n", "");
                    }
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
    }
}