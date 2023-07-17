// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BitPay.Exceptions;
using BitPay.Models.Payout;
using BitPay.Utils;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitPay.Clients
{
    public class PayoutRecipientsClient
    {
        private readonly IBitPayClient _bitPayClient;
        private readonly AccessTokens _accessTokens;
        private readonly IGuidGenerator _guidGenerator;

        public PayoutRecipientsClient(IBitPayClient bitPayClient, AccessTokens accessTokens, IGuidGenerator guidGenerator)
        {
            _bitPayClient = bitPayClient ?? throw new MissingRequiredField("bitPayClient");
            _accessTokens = accessTokens ?? throw new MissingRequiredField("accessTokens");
            _guidGenerator = guidGenerator ?? throw new MissingRequiredField("guidGenerator");
        }
        
        /// <summary>
        ///     Submit BitPay Payout Recipients.
        /// </summary>
        /// <param name="recipients">A PayoutRecipients object with request parameters defined.</param>
        /// <returns>A list of BitPay PayoutRecipients objects.</returns>
        /// <throws>PayoutRecipientCreationException PayoutRecipientCreationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<List<PayoutRecipient>> Submit(PayoutRecipients recipients)
        {
            if (recipients == null) throw new MissingFieldException(nameof(recipients));
            try
            {
                recipients.Token = _accessTokens.GetAccessToken(Facade.Payout);
                recipients.ResourceGuid ??= _guidGenerator.Execute();

                var json = JsonConvert.SerializeObject(recipients);
                var response = await _bitPayClient.Post("recipients", json, true)
                    .ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<List<PayoutRecipient>>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
            catch (BitPayException ex)
            {
                throw new PayoutRecipientCreationException(ex, ex.ApiCode);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new PayoutRecipientCreationException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve a BitPay payout recipient by recipient id using.
        ///     The client must have been previously authorized for the payout facade.
        /// </summary>
        /// <param name="recipientId">The id of the recipient to retrieve.</param>
        /// <returns>A BitPay PayoutRecipient object.</returns>
        /// <throws>PayoutRecipientQueryException PayoutRecipientQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<PayoutRecipient> Get(string recipientId)
        {
            if (recipientId == null) throw new MissingFieldException(nameof(recipientId));
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));

                var response = await _bitPayClient.Get("recipients/" + recipientId, parameters)
                    .ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<PayoutRecipient>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
            catch (BitPayException ex)
            {
                throw new PayoutRecipientQueryException(ex, ex.ApiCode);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new PayoutRecipientQueryException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve a collection of BitPay Payout Recipients.
        /// </summary>
        /// <param name="status">The recipient status you want to query on.</param>
        /// <param name="limit">Maximum results that the query will return (useful for paging results).</param>
        /// <param name="offset">Offset for paging</param>
        /// <returns>A list of BitPayRecipient objects.</returns>
        /// <throws>PayoutRecipientQueryException PayoutRecipientQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<List<PayoutRecipient>> GetByFilters(string? status, int limit, int offset)
        {
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                if (!string.IsNullOrEmpty(status))
                {
                    parameters.Add("status", status!);
                }
                parameters.Add("limit", limit.ToString());
                parameters.Add("offset", offset.ToString());
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));

                var response = await _bitPayClient.Get("recipients", parameters).ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<List<PayoutRecipient>>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
            catch (BitPayException ex)
            {
                throw new PayoutRecipientQueryException(ex, ex.ApiCode);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new PayoutRecipientQueryException();

                throw;
            }
        }

        /// <summary>
        ///     Update a Payout Recipient.
        /// </summary>
        /// <param name="recipientId">The recipient id for the recipient to be updated.</param>
        /// <param name="recipient">A PayoutRecipient object with updated parameters defined.
        ///     Available changes: label, token
        /// </param>
        /// <returns>The updated recipient object.</returns>
        /// <throws>PayoutRecipientUpdateException PayoutRecipientUpdateException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<PayoutRecipient> Update(string recipientId, PayoutRecipient recipient)
        {
            if (recipientId == null) throw new MissingFieldException(nameof(recipientId));
            if (recipient == null) throw new MissingFieldException(nameof(recipient));
            
            try
            {
                recipient.Token = _accessTokens.GetAccessToken(Facade.Payout);

                var json = JsonConvert.SerializeObject(recipient);
                var response = await _bitPayClient.Put("recipients/" + recipientId, json).ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false); 
                return JsonConvert.DeserializeObject<PayoutRecipient>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
            catch (BitPayException ex)
            {
                throw new PayoutRecipientUpdateException(ex, ex.ApiCode);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new PayoutRecipientUpdateException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Cancel a BitPay Payout recipient.
        /// </summary>
        /// <param name="recipientId">The id of the recipient to cancel.</param>
        /// <returns>True if the delete operation was successfully, false otherwise.</returns>
        /// <throws>PayoutRecipientCancellationException PayoutRecipientCancellationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<bool> Delete(string recipientId)
        {
            if (recipientId == null) throw new MissingFieldException(nameof(recipientId));
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));
                
                var response = await _bitPayClient.Delete("recipients/" + recipientId, parameters).ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                JObject responseObject = JsonConvert.DeserializeObject<dynamic>(responseString)!;
                return "success".Equals(
                    responseObject.GetValue("status", StringComparison.Ordinal)?.ToString(),
                    StringComparison.OrdinalIgnoreCase
                );
            }
            catch (BitPayException ex)
            {
                throw new PayoutRecipientCancellationException(ex, ex.ApiCode);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new PayoutRecipientCancellationException();

                throw;
            }
        }

        /// <summary>
        ///     Send a payout recipient notification
        /// </summary>
        /// <param name="recipientId">The id of the recipient to notify.</param>
        /// <returns>True if the notification was successfully sent, false otherwise.</returns>
        /// <throws>PayoutRecipientNotificationException PayoutRecipientNotificationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<bool> RequestPayoutRecipientNotification(string recipientId)
        {
           try
           {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));

                var json = JsonConvert.SerializeObject(parameters);
                var response = await _bitPayClient.Post("recipients/" + recipientId + "/notifications", json, true)
                    .ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                JObject responseObject = JsonConvert.DeserializeObject<dynamic>(responseString)!;
                return "success".Equals(
                    responseObject.GetValue("status", StringComparison.Ordinal)?.ToString(),
                    StringComparison.OrdinalIgnoreCase
                    );
           }
           catch (BitPayException ex)
           {
               throw new PayoutRecipientNotificationException(ex, ex.ApiCode);
           }
           catch (Exception ex)
           {
               if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                   throw new PayoutRecipientNotificationException(ex);

               throw;
           }
        }
    }
}