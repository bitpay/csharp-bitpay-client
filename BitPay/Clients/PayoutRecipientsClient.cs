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
            _bitPayClient = bitPayClient;
            _accessTokens = accessTokens;
            _guidGenerator = guidGenerator;
        }
        
        /// <summary>
        ///     Submit BitPay Payout Recipients.
        /// </summary>
        /// <param name="recipients">A PayoutRecipients object with request parameters defined.</param>
        /// <returns>A list of BitPay PayoutRecipients objects.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<List<PayoutRecipient>> Submit(PayoutRecipients recipients)
        {
            if (recipients == null) BitPayExceptionProvider.ThrowMissingParameterException();

            recipients.Token = _accessTokens.GetAccessToken(Facade.Payout);
            recipients.ResourceGuid ??= _guidGenerator.Execute();

            string json;
            
            try
            {
                json = JsonConvert.SerializeObject(recipients);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowSerializeResourceException("Payout Recipient", e.Message);
                throw;
            }
            
            var response = await _bitPayClient.Post("recipients", json, true)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<List<PayoutRecipient>>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Payout Recipient", e.Message);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve a BitPay payout recipient by recipient id using.
        ///     The client must have been previously authorized for the payout facade.
        /// </summary>
        /// <param name="recipientId">The id of the recipient to retrieve.</param>
        /// <returns>A BitPay PayoutRecipient object.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<PayoutRecipient> Get(string recipientId)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));

            var response = await _bitPayClient.Get("recipients/" + recipientId, parameters)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<PayoutRecipient>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Payout Recipient", e.Message);

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
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<List<PayoutRecipient>> GetByFilters(string? status, int limit, int offset)
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
            
            try
            {
                return JsonConvert.DeserializeObject<List<PayoutRecipient>>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
         
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Payout Recipient", e.Message);

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
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<PayoutRecipient> Update(string recipientId, PayoutRecipient recipient)
        {
            if (recipient == null) BitPayExceptionProvider.ThrowMissingParameterException();
            
            recipient.Token = _accessTokens.GetAccessToken(Facade.Payout);

            string json;
            
            try
            {
                json = JsonConvert.SerializeObject(recipient);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowSerializeResourceException("Payout Recipient", e.Message);
                throw;
            }

            var response = await _bitPayClient.Put("recipients/" + recipientId, json).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false); 
            
            try
            {
                return JsonConvert.DeserializeObject<PayoutRecipient>(responseString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Payout Recipient", e.Message);

                throw;
            }
        }

        /// <summary>
        ///     Cancel a BitPay Payout recipient.
        /// </summary>
        /// <param name="recipientId">The id of the recipient to cancel.</param>
        /// <returns>True if the delete operation was successfully, false otherwise.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<bool> Delete(string recipientId)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));
                
            var response = await _bitPayClient.Delete("recipients/" + recipientId, parameters).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                JObject responseObject = JsonConvert.DeserializeObject<dynamic>(responseString)!;
                return "success".Equals(
                    responseObject.GetValue("status", StringComparison.Ordinal)?.ToString(),
                    StringComparison.OrdinalIgnoreCase
                );
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Payout Recipient", e.Message);

                throw;
            }
        }

        /// <summary>
        ///     Send a payout recipient notification
        /// </summary>
        /// <param name="recipientId">The id of the recipient to notify.</param>
        /// <returns>True if the notification was successfully sent, false otherwise.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<bool> RequestPayoutRecipientNotification(string recipientId)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Payout));
            
            string json;
            
            try
            {
                json = JsonConvert.SerializeObject(parameters);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowSerializeParamsException(e.Message);
                throw;
            }
            
            var response = await _bitPayClient.Post("recipients/" + recipientId + "/notifications", json, true)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
           try
           {
                JObject responseObject = JsonConvert.DeserializeObject<dynamic>(responseString)!;
                return "success".Equals(
                    responseObject.GetValue("status", StringComparison.Ordinal)?.ToString(),
                    StringComparison.OrdinalIgnoreCase
                    );
           }
           catch (Exception e)
           {
               BitPayExceptionProvider.ThrowDeserializeResourceException("Payout Recipient", e.Message);

               throw;
           }
        }
    }
}