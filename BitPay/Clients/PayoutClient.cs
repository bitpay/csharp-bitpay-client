using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BitPay.Exceptions;
using BitPay.Models.Payout;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitPay.Clients
{
    public class PayoutClient
    {
        private readonly BitPayClient _bitPayClient;
        private readonly AccessTokens _accessTokens;

        public PayoutClient(BitPayClient bitPayClient, AccessTokens accessTokens)
        {
            _bitPayClient = bitPayClient ?? throw new MissingRequiredField("bitPayClient");
            _accessTokens = accessTokens ?? throw new MissingRequiredField("accessTokens");
        }

        /// <summary>
        ///     Submit a BitPay Payout.
        /// </summary>
        /// <param name="payout">A Payout object with request parameters defined.</param>
        /// <returns>A BitPay generated Payout object.</returns>
        /// <throws>PayoutCreationException PayoutCreationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Payout> Submit(Payout payout)
        {
            if (payout == null) throw new MissingRequiredField(nameof(payout));
            try
            {
                payout.Token = _accessTokens.GetAccessToken(Facade.Payout);

                var json = JsonConvert.SerializeObject(payout);
                var response = await _bitPayClient.Post("payouts", json, true);
                var responseString = await _bitPayClient.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Payout>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }
            catch (BitPayException ex)
            {
                throw new PayoutCreationException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new PayoutCreationException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve a BitPay payout by payout id using.  The client must have been previously authorized for the 
        ///     payout facade.
        /// </summary>
        /// <param name="payoutId">The id of the payout to retrieve.</param>
        /// <returns>A BitPay generated Payout object.</returns>
        /// <throws>PayoutQueryException PayoutQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Payout> Get(string payoutId)
        {
            if (payoutId == null) throw new MissingFieldException(nameof(payoutId));
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));

                var response = await _bitPayClient.Get("payouts/" + payoutId, parameters);
                var responseString = await _bitPayClient.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Payout>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }
            catch (BitPayException ex)
            {
                throw new PayoutQueryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new PayoutQueryException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Cancel a BitPay Payout.
        /// </summary>
        /// <param name="payoutId">The id of the payout to cancel.</param>
        /// <returns>True if payout was successfully canceled, false otherwise.</returns>
        /// <throws>PayoutCancellationException PayoutCancellationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<bool> Cancel(string payoutId)
        {
            if (payoutId == null) throw new MissingFieldException(nameof(payoutId));
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));

                var response = await _bitPayClient.Delete("payouts/" + payoutId, parameters);
                var responseString = await _bitPayClient.ResponseToJsonString(response).ConfigureAwait(false);
                JObject responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);
                return responseObject.GetValue("status").ToString() == "success";
            }
            catch (BitPayException ex)
            {
                throw new PayoutCancellationException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new PayoutCancellationException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve a collection of BitPay payouts.
        /// </summary>
        /// <param name="filters">
        /// Filters available: startDate, endDate, status, reference, limit, offset.
        /// See https://bitpay.com/api/#rest-api-resources-payouts-retrieve-payouts-filtered-by-query
        /// </param>
        /// <returns>A list of BitPay Payout objects.</returns>
        /// <throws>PayoutQueryException PayoutQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<List<Payout>> GetPayouts(Dictionary<string, dynamic> filters)
        {
            if (filters == null) throw new MissingFieldException(nameof(filters));
            
            try
            {
                filters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));

                var response = await _bitPayClient.Get("payouts", filters).ConfigureAwait(false); 
                var responseString = await _bitPayClient.ResponseToJsonString(response).ConfigureAwait(false); 
                return JsonConvert.DeserializeObject<List<Payout>>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }
            catch (BitPayException ex)
            {
                throw new PayoutQueryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new PayoutQueryException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Send a payout notification    
        /// </summary>
        /// <param name="payoutId">The id of the payout to notify.</param>
        /// <returns>True if the notification was successfully sent, false otherwise.</returns>
        /// <throws>PayoutNotificationException PayoutNotificationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<bool> RequestPayoutNotification(string payoutId)
        {
            if (payoutId == null) throw new MissingFieldException(nameof(payoutId));
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));

                var json = JsonConvert.SerializeObject(parameters);
                var response = await _bitPayClient.Post("payouts/" + payoutId + "/notifications", json, true);
                var responseString = await _bitPayClient.ResponseToJsonString(response).ConfigureAwait(false);
                JObject responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);
                return responseObject.GetValue("status").ToString() == "success";
            }
            catch (BitPayException ex)
            {
                throw new PayoutNotificationException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new PayoutNotificationException(ex);

                throw;
            }
        }
    }
}