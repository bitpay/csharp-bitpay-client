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
            _bitPayClient = bitPayClient ?? throw new MissingRequiredField("bitPayClient");
            _accessTokens = accessTokens ?? throw new MissingRequiredField("accessTokens");
            _guidGenerator = guidGenerator ?? throw new MissingRequiredField("guidGenerator");
        }

        ///  <summary>
        ///      Create a refund for a BitPay invoice.
        ///  </summary>
        ///  <param name="refundToCreate">
        ///     Available params: amount, invoiceId, token, preview, immediate, buyerPaysRefundFee, reference.
        ///     See https://bitpay.readme.io/reference/create-a-refund-request
        /// </param>
        ///  <returns>An updated Refund Object</returns> 
        /// <throws>RefundCreationException RefundCreationException class</throws> 
        /// <throws>BitPayException BitPayException class</throws> 
        public async Task<Refund> Create(Refund refundToCreate)
        {
            if (refundToCreate == null) throw new MissingFieldException(nameof(refundToCreate));
            if (refundToCreate.Amount == 0) throw new RefundCreationException(null, "Wrong refund Amount");
            
            try
            {
                refundToCreate.Guid = refundToCreate.Guid ?? _guidGenerator.Execute();
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
                parameters.Add("amount", refundToCreate.Amount);
                if (refundToCreate.InvoiceId != null) parameters.Add("invoiceId", refundToCreate.InvoiceId);
                if (refundToCreate.Currency != null) parameters.Add("currency", refundToCreate.Currency);
                parameters.Add("preview", refundToCreate.Preview);
                parameters.Add("immediate", refundToCreate.Immediate);
                parameters.Add("buyerPaysRefundFee", refundToCreate.BuyerPaysRefundFee);
                if (refundToCreate.Reference != null) parameters.Add("reference", refundToCreate.Reference);
                parameters.Add("guid", refundToCreate.Guid);
             
 
                var json = JsonConvert.SerializeObject(parameters);
                var response = await _bitPayClient.Post("refunds", json, true);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<Refund>(responseString);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new RefundCreationException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve a previously made refund request on a BitPay invoice.
        /// </summary>
        ///<param name="refundId">The BitPay refund ID.</param>
        ///<returns>A BitPay Refund object with the associated Refund object.</returns> 
        ///<throws>RefundQueryException RefundQueryException class</throws> 
        public async Task<Refund> GetById(string refundId)
        {
            if (refundId == null) throw new MissingFieldException(nameof(refundId));
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));

                var response = await _bitPayClient.Get("refunds/" + refundId, parameters, true);
                var responseString = await HttpResponseParser.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Refund>(responseString);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new RefundQueryException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve a previously made refund request on a BitPay invoice.
        /// </summary>
        ///<param name="guid"></param>The BitPay guid.
        ///<returns>A BitPay Refund object with the associated Refund object.</returns> 
        ///<throws>RefundQueryException RefundQueryException class</throws> 
        public async Task<Refund> GetByGuid(string guid)
        {
            if (guid == null) throw new MissingFieldException(nameof(guid));
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));

                var response = await _bitPayClient.Get("refunds/guid/" + guid, parameters, true);
                var responseString = await HttpResponseParser.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Refund>(responseString);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new RefundQueryException(ex);

                throw;
            }
        }
        
        /// <summary>
        ///     Retrieve all refund requests on a BitPay invoice.
        /// </summary>
        /// <param name="invoiceId">The id of the requested invoice.</param>
        /// <returns>A BitPay invoice object with the associated Refund objects updated.</returns>
        /// <throws>RefundQueryException RefundQueryException class</throws>
        public async Task<List<Refund>> GetByInvoiceId(string invoiceId)
        {
            if (invoiceId == null) throw new MissingFieldException(nameof(invoiceId));
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));
                parameters.Add("invoiceId", invoiceId);
               
                var response = await _bitPayClient.Get("refunds", parameters, true);
                var responseString = await HttpResponseParser.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<List<Refund>>(responseString);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new RefundQueryException(ex);

                throw;
            }
        }
        
        /// <summary>
        ///     Update the status of a BitPay invoice.
        /// </summary>
        ///<param name="refundId">A BitPay refund ID</param> .
        ///<param name="status">The new status for the refund to be updated.</param>   
        /// <returns>A BitPay generated Refund object.</returns>
        ///<throws>RefundUpdateException class</throws> 
        public async Task<Refund> Update(string refundId, string status)
        {
            if (refundId == null) throw new MissingFieldException(nameof(refundId));
            if (status == null) throw new MissingFieldException(nameof(status));

            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant)); 
                parameters.Add("status", status);

                var json = JsonConvert.SerializeObject(parameters);
                var response = await _bitPayClient.Put("refunds/" + refundId, json).ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<Refund>(responseString);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new RefundUpdateException(ex);

                throw;
            }
        }
        
        /// <summary>
        ///     Update the status of a BitPay invoice.
        /// </summary>
        ///<param name="guid">A BitPay Guid.</param>
        ///<param name="status">The new status for the refund to be updated.</param>   
        /// <returns>A BitPay generated Refund object.</returns>
        ///<throws>RefundUpdateException class</throws> 
        public async Task<Refund> UpdateByGuid(string guid, string status)
        {
            if (guid == null) throw new MissingFieldException(nameof(guid));
            if (status == null) throw new MissingFieldException(nameof(status));

            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant)); 
                parameters.Add("status", status);

                var json = JsonConvert.SerializeObject(parameters);
                var response = await _bitPayClient.Put("refunds/guid/" + guid, json).ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<Refund>(responseString);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new RefundUpdateException(ex);

                throw;
            }
        }
        
        /// <summary>
        ///     Cancel a previously submitted refund request.
        /// </summary>
        /// <param name="refundId">The ID of the refund request for which you want to cancel.</param>
        /// <returns>Refund object.</returns>
        /// <throws>RefundCancellationException RefundCancellationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Refund> Cancel(string refundId)
        {
            if (refundId == null) throw new ArgumentNullException(nameof(refundId));
            
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));

                var response = await _bitPayClient.Delete("refunds/" + refundId, parameters);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<Refund>(responseString);
            }
            catch (BitPayException ex)
            {
                throw new RefundCancellationException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new RefundCancellationException(ex);

                throw;
            }
        }
        
        /// <summary>
        ///     Cancel a previously submitted refund request.
        /// </summary>
        /// <param name="guid">The GUID of the refund request for which you want to cancel.</param>
        /// <returns>Refund object.</returns>
        /// <throws>RefundCancellationException RefundCancellationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Refund> CancelByGuid(string guid)
        {
            if (guid == null) throw new ArgumentNullException(nameof(guid));
            
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));

                var response = await _bitPayClient.Delete("refunds/guid/" + guid, parameters);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<Refund>(responseString);
            }
            catch (BitPayException ex)
            {
                throw new RefundCancellationException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new RefundCancellationException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Send a refund notification
        /// </summary>
        /// <param name="refundId">A BitPay refundId </param>
        /// <returns>An updated Refund Object </returns>
        /// <throws>RefundCreationException RefundCreationException class </throws>
        /// <throws>BitPayException BitPayException class </throws>
        public async Task<bool> SendRefundNotification(string refundId)
        {
            if (refundId == null) throw new MissingFieldException(nameof(refundId));
            
            try
            {
                var parameters = ResourceClientUtil.InitParams();
                parameters.Add("token", _accessTokens.GetAccessToken(Facade.Merchant));

                var json = JsonConvert.SerializeObject(parameters);
                var response = await _bitPayClient.Post("refunds/" + refundId + "/notifications", json, true);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                JObject responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);
                return responseObject.GetValue("status").ToString() == "success";
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new RefundNotificationException(ex);

                throw;
            }
        }
    }
}