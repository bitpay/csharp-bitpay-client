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
            _bitPayClient = bitPayClient ?? throw new MissingRequiredField("bitPayClient");
            _accessTokens = accessTokens ?? throw new MissingRequiredField("accessTokens");
        }

        /// <summary>
        ///     Submit a BitPay Payouts.
        /// </summary>
        /// <param name="payouts">Collection of Payout object with request parameters defined.</param>
        /// <returns>Object with created payouts and information's about not created payouts.</returns>
        /// <throws>PayoutCreationException PayoutCreationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<PayoutGroup> Submit(ICollection<Payout> payouts)
        {
            if (payouts == null) throw new MissingRequiredField(nameof(payouts));
            
            try
            {
                var request = new Dictionary<string, object>
                {
                    {"instructions", payouts},
                    {"token", _accessTokens.GetAccessToken(Facade.Payout)}
                };

                var json = JsonConvert.SerializeObject(request);
                var response = await _bitPayClient.Post("payouts/group", json, true).ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<PayoutGroup>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
            catch (BitPayException ex)
            {
                throw new PayoutCreationException(ex, ex.ApiCode);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new PayoutCreationException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Cancel a BitPay Payouts.
        /// </summary>
        /// <param name="groupId">The id of the payout group to cancel.</param>
        /// <returns>Object with cancelled payouts and information's about not cancelled payouts.</returns>
        /// <throws>PayoutCancellationException PayoutCancellationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<PayoutGroup> Cancel(string groupId)
        {
            if (groupId == null) throw new MissingFieldException(nameof(groupId));
            
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));

                var response = await _bitPayClient.Delete("payouts/group/" + groupId, parameters)
                    .ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<PayoutGroup>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
            catch (BitPayException ex)
            {
                throw new PayoutCancellationException(ex, ex.ApiCode);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new PayoutCancellationException(ex);

                throw;
            }
        }
    }
}