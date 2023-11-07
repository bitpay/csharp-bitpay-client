// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BitPay.Exceptions;
using BitPay.Models.Invoice;
using BitPay.Utils;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitPay.Clients
{
    public class RefundClient
    {
        private readonly IBitPayClient _bitPayClient;
        private readonly AccessTokens _accessTokens;
        private readonly IGuidGenerator _guidGenerator;

        public RefundClient(IBitPayClient bitPayClient, AccessTokens accessTokens, IGuidGenerator guidGenerator)
        {
            _bitPayClient = bitPayClient;
            _accessTokens = accessTokens;
            _guidGenerator = guidGenerator;
        }

        ///  <summary>
        ///      Create a refund for a BitPay invoice.
        ///  </summary>
        ///  <param name="refundToCreate">
        ///     Available params: amount, invoiceId, token, preview, immediate, buyerPaysRefundFee, reference.
        ///     See https://bitpay.readme.io/reference/create-a-refund-request
        /// </param>
        ///  <returns>An updated Refund Object</returns> 
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Refund> Create(Refund refundToCreate)
        {
            if (refundToCreate == null)
            {
                BitPayExceptionProvider.ThrowMissingParameterException();
                throw new InvalidOperationException();
            }
            if (refundToCreate.Amount == 0) BitPayExceptionProvider.ThrowValidationException("Wrong refund Amount");
            
            refundToCreate.ResourceGuid ??= _guidGenerator.Execute();
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
            parameters.Add("amount", refundToCreate.Amount);
            if (refundToCreate.InvoiceId != null) parameters.Add("invoiceId", refundToCreate.InvoiceId);
            if (refundToCreate.Currency != null) parameters.Add("currency", refundToCreate.Currency);
            parameters.Add("preview", refundToCreate.Preview);
            parameters.Add("immediate", refundToCreate.Immediate);
            parameters.Add("buyerPaysRefundFee", refundToCreate.BuyerPaysRefundFee);
            if (refundToCreate.Reference != null) parameters.Add("reference", refundToCreate.Reference);
            parameters.Add("guid", refundToCreate.ResourceGuid);

            string json;

            try
            {
                json = JsonConvert.SerializeObject(parameters);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowSerializeResourceException("Refund", e.Message);
                throw;
            }
            
            var response = await _bitPayClient.Post("refunds", json, true).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<Refund>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Refund", e.Message);
                throw;
            }
        }

        /// <summary>
        ///     Retrieve a previously made refund request on a BitPay invoice.
        /// </summary>
        ///<param name="refundId">The BitPay refund ID.</param>
        ///<returns>A BitPay Refund object with the associated Refund object.</returns> 
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Refund> GetById(string refundId)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));

            var response = await _bitPayClient.Get("refunds/" + refundId, parameters)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<Refund>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Refund", e.Message);
                throw;
            }
        }

        /// <summary>
        ///     Retrieve a previously made refund request on a BitPay invoice.
        /// </summary>
        ///<param name="refundGuid">The GUID of the refund request for which you want to receive.</param>
        ///<returns>A BitPay Refund object with the associated Refund object.</returns> 
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Refund> GetByGuid(string refundGuid)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));

            var response = await _bitPayClient.Get("refunds/guid/" + refundGuid, parameters)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<Refund>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Refund", e.Message);
                throw;
            }
        }
        
        /// <summary>
        ///     Retrieve all refund requests on a BitPay invoice.
        /// </summary>
        /// <param name="invoiceId">The id of the requested invoice.</param>
        /// <returns>A BitPay invoice object with the associated Refund objects updated.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<List<Refund>> GetByInvoiceId(string invoiceId)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
            parameters.Add("invoiceId", invoiceId);
               
            var response = await _bitPayClient.Get("refunds", parameters)
                .ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<List<Refund>>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Refund", e.Message);
                throw;
            }
        }
        
        /// <summary>
        ///     Update the status of a BitPay invoice.
        /// </summary>
        ///<param name="refundId">A BitPay refund ID</param> .
        ///<param name="status">The new status for the refund to be updated.</param>   
        /// <returns>A BitPay generated Refund object.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Refund> Update(string refundId, string status)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant)); 
            parameters.Add("status", status);
            
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

            var response = await _bitPayClient.Put("refunds/" + refundId, json).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<Refund>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Refund", e.Message);
                throw;
            }
        }
        
        /// <summary>
        ///     Update the status of a BitPay invoice.
        /// </summary>
        ///<param name="refundGuid">The GUID of the refund request for which you want to update.</param>
        ///<param name="status">The new status for the refund to be updated.</param>   
        /// <returns>A BitPay generated Refund object.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Refund> UpdateByGuid(string refundGuid, string status)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant)); 
            parameters.Add("status", status);
            
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
            
            var response = await _bitPayClient.Put("refunds/guid/" + refundGuid, json).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);

            try
            {
                return JsonConvert.DeserializeObject<Refund>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Refund", e.Message);
                throw;
            }
        }
        
        /// <summary>
        ///     Cancel a previously submitted refund request.
        /// </summary>
        /// <param name="refundId">The ID of the refund request for which you want to cancel.</param>
        /// <returns>Refund object.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Refund> Cancel(string refundId)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));

            var response = await _bitPayClient.Delete("refunds/" + refundId, parameters).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<Refund>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Refund", e.Message);
                throw;
            }
        }
        
        /// <summary>
        ///     Cancel a previously submitted refund request.
        /// </summary>
        /// <param name="refundGuid">The GUID of the refund request for which you want to cancel.</param>
        /// <returns>Refund object.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<Refund> CancelByGuid(string refundGuid)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));

            var response = await _bitPayClient.Delete("refunds/guid/" + refundGuid, parameters).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            
            try
            {
                return JsonConvert.DeserializeObject<Refund>(responseString)!;
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Refund", e.Message);
                throw;
            }
        }

        /// <summary>
        ///     Send a refund notification
        /// </summary>
        /// <param name="refundId">A BitPay refundId </param>
        /// <returns>An updated Refund Object </returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<bool> SendRefundNotification(string refundId)
        {
            var parameters = ResourceClientUtil.InitParams();
            parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
            
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
            
            var response = await _bitPayClient.Post("refunds/" + refundId + "/notifications", json, true)
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
                BitPayExceptionProvider.ThrowDeserializeResourceException("Refund", e.Message);
                throw;
            }
        }
    }
}