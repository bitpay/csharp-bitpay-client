// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BitPay.Exceptions;
using BitPay.Models.Payout;

using Newtonsoft.Json;

namespace BitPay.Clients
{
    public class PayoutGroupClient
    {
        private readonly IBitPayClient _bitPayClient;
        private readonly AccessTokens _accessTokens;

        public PayoutGroupClient(IBitPayClient bitPayClient, AccessTokens accessTokens)
        {
            _bitPayClient = bitPayClient;
            _accessTokens = accessTokens;
        }

        /// <summary>
        ///     Submit a BitPay Payouts.
        /// </summary>
        /// <param name="payouts">Collection of Payout object with request parameters defined.</param>
        /// <returns>Object with created payouts and information's about not created payouts.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<PayoutGroup> Submit(ICollection<Payout> payouts)
        {
            var request = new Dictionary<string, object>
            {
                {"instructions", payouts},
                {"token", _accessTokens.GetAccessToken(Facade.Payout)}
            };

            string json = null!;

            try
            {
                json = JsonConvert.SerializeObject(request);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowSerializeResourceException("Payout Group", e.Message);

                throw;
            }
            
            var response = await _bitPayClient.Post("payouts/group", json, true).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<PayoutGroup>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Payout Group", e.Message);

                throw;
            }
        }

        /// <summary>
        ///     Cancel a BitPay Payouts.
        /// </summary>
        /// <param name="groupId">The id of the payout group to cancel.</param>
        /// <returns>Object with cancelled payouts and information's about not cancelled payouts.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<PayoutGroup> Cancel(string groupId)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));

            var response = await _bitPayClient.Delete("payouts/group/" + groupId, parameters)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<PayoutGroup>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Payout Group", e.Message);

                throw;
            }
        }
    }
}