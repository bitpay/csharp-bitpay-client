using System;
using System.Collections.Generic;
using BitPayAPI.Exceptions;
using Newtonsoft.Json;

namespace BitPayAPI.Models
{
    public class PayoutBatch
    {
        public const string StatusNew = "new";
	    public const string StatusFunded = "funded";
	    public const string StatusProcessing = "processing";
	    public const string StatusComplete = "complete";
	    public const string StatusFailed = "failed";
	    public const string StatusCancelled = "cancelled";

	    public const string MethodManual2 = "manual_2";
	    public const string MethodVwap24 = "vwap_24hr";    

        /// <summary>
        /// Constructor, create an empty PayoutBatch object.
        /// </summary>
        public PayoutBatch() {
            Amount = 0.0;
            Currency = "USD";
            Reference = "";
            BankTransferId = "";
            NotificationEmail = "";
            NotificationUrl = "";
            PricingMethod = MethodVwap24;
        }

        /// <summary>
        /// Constructor, create an instruction-full request PayoutBatch object.
        /// </summary>
        /// <param name="effectiveDate">Date when request is effective. Note that the time of day will automatically be set to 09:00:00.000 UTC time for the given day. Only requests submitted before 09:00:00.000 UTC are guaranteed to be processed on the same day.</param>
        /// <param name="reference">Merchant-provided data.</param>
        /// <param name="bankTransferId">Merchant-provided data, to help match funding payments to payout batches.</param>
        /// <param name="instructions">Payout instructions.</param>
        public PayoutBatch(string currency, DateTime effectiveDate, string bankTransferId, string reference, List<PayoutInstruction> instructions) : this() {
            Currency = currency;
            EffectiveDate = effectiveDate;
            BankTransferId = bankTransferId;
            Reference = reference;
            Instructions = instructions;
            _computeAndSetAmount();
        }

        // Private methods
        //

        private void _computeAndSetAmount() {
    	    double amount = 0.0;
            for (int i = 0; i < Instructions.Count; i++)
            {
    	        amount += Instructions[i].Amount;
    	    }
    	    Amount = amount;
        }

        // API fields
        //

        [JsonProperty(PropertyName = "guid")]
        public string Guid { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
	
        // Required fields
        //

        [JsonProperty(PropertyName = "effectiveDate")]
        [JsonConverter(typeof(Converters.DateStringConverter))]
        public DateTime EffectiveDate { get; set; }

        [JsonProperty(PropertyName = "reference")]
        public string Reference { get; set; }

        [JsonProperty(PropertyName = "bankTransferId")]
        public string BankTransferId { get; set; }

        // Optional fields
        //

        [JsonProperty(PropertyName = "instructions")]
        public List<PayoutInstruction> Instructions { get; set; }
        public bool ShouldSerializeInstructions() { return (Instructions != null && Instructions.Count > 0); }

        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }
        public bool ShouldSerializeAmount() { return true; }

        string _currency = "";
        [JsonProperty(PropertyName = "currency")]
        public string Currency
        {
            get { return _currency; }
            set
            {
                if (value.Length != 3)
                {
                    throw new BitPayException("Error: currency code must be exactly three characters");
                }
                _currency = value;
            }
        }

        [JsonProperty(PropertyName = "pricingMethod")]
        public string PricingMethod { get; set; }
        public bool ShouldSerializePricingMethod() { return !string.IsNullOrEmpty(PricingMethod); }

        [JsonProperty(PropertyName = "notificationEmail")]
        public string NotificationEmail { get; set; }
        public bool ShouldSerializeNotificationEmail() { return !string.IsNullOrEmpty(NotificationEmail); }

        [JsonProperty(PropertyName = "notificationURL")]
        public string NotificationUrl { get; set; }
        public bool ShouldSerializeNotificationUrl() { return !string.IsNullOrEmpty(NotificationUrl); }

        // Response fields
        //

        public string Id { get; set; }
        public bool ShouldSerializeId() { return false; }

        public string Account { get; set; }
        public bool ShouldSerializeAccount() { return false; }

        public string Status { get; set; }
        public bool ShouldSerializeStatus() { return false; }

        public double Btc { get; set; }
        public bool ShouldSerializeBtc() { return false; }

        [JsonConverter(typeof(Converters.DateStringConverter))]
        public DateTime RequestDate { get; set; }
        public bool ShouldSerializeRequestDate() { return false; }

        public double PercentFee { get; set; }
        public bool ShouldSerializePercentFee() { return false; }

        public double Fee { get; set; }
        public bool ShouldSerializeFee() { return false; }

        public double DepositTotal { get; set; }
        public bool ShouldSerializeDepositTotal() { return false; }

        public string SupportPhone { get; set; }
        public bool ShouldSerializeSupportPhone() { return false; }
    
    }
}
