// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using BitPay.Exceptions;
using BitPay.Logger;

using Newtonsoft.Json.Linq;

namespace BitPay.Clients
{
    public static class HttpResponseParser
    {
        public static async Task<string> ResponseToJsonString(HttpResponseMessage? response)
        {
            if (response == null)
                BitPayExceptionProvider.ThrowApiExceptionWithMessage("HTTP response is null");
            
            string? responseString = null;
            
            try
            {
                // Get the response as a dynamic object for detecting possible error(s) or data object.
                // An error(s) object raises an exception.
                // A data object has its content extracted (throw away the data wrapper object).
                responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                HttpRequestMessage responseRequestMessage = response.RequestMessage;
                LoggerProvider.GetLogger().LogResponse(
                    responseRequestMessage.Method.ToString(), 
                    responseRequestMessage.RequestUri.ToString(),
                    responseString);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowApiExceptionWithMessage(e.Message);
            }
            
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

            JToken? value;
            JToken? code;

            if (jObj.TryGetValue("status", out value))
            {
               if ("error".Equals(value.ToString(), StringComparison.OrdinalIgnoreCase))
               {
                   jObj.TryGetValue("code", out code);
                   jObj.TryGetValue("message", out value);
                   
                   BitPayExceptionProvider.ThrowApiExceptionWithMessage(value!.ToString(), code!.ToString());
               }
            }
            
            if (jObj.TryGetValue("error", out value))
            {
                jObj.TryGetValue("code", out code);
                BitPayExceptionProvider.ThrowApiExceptionWithMessage(value!.ToString(), code!.ToString());
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
    }
}