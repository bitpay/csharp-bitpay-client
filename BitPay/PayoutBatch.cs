using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BitPayAPI
{
    public class PayoutBatch
    {
        public const String STATUS_NEW = "new";
	    public const String STATUS_FUNDED = "funded";
	    public const String STATUS_PROCESSING = "processing";
	    public const String STATUS_COMPLETE = "complete";
	    public const String STATUS_FAILED = "failed";
	    public const String STATUS_CANCELLED = "cancelled";

	    public const String METHOD_MANUAL2 = "manual_2";
	    public const String METHOD_VWAP24 = "vwap_24hr";    

        /// <summary>
        /// Constructor, create an empty PayoutBatch object.
        /// </summary>
        public PayoutBatch() {
            Amount = 0.0;
            Currency = "USD";
            Reference = "";
            BankTransferId = "";
            NotificationEmail = "";
            NotificationURL = "";
            PricingMethod = METHOD_VWAP24;
        }

        /// <summary>
        /// Constructor, create an instruction-full request PayoutBatch object.
        /// </summary>
        /// <param name="effectiveDate">Date when request is effective. Note that the time of day will automatically be set to 09:00:00.000 UTC time for the given day. Only requests submitted before 09:00:00.000 UTC are guaranteed to be processed on the same day.</param>
        /// <param name="reference">Merchant-provided data.</param>
        /// <param name="bankTransferId">Merchant-provided data, to help match funding payments to payout batches.</param>
        /// <param name="instructions">Payout instructions.</param>
        public PayoutBatch(String currency, DateTime effectiveDate, String bankTransferId, String reference, List<PayoutInstruction> instructions) : this() {
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
        public String Reference { get; set; }

        [JsonProperty(PropertyName = "bankTransferId")]
        public String BankTransferId { get; set; }

        // Optional fields
        //

        [JsonProperty(PropertyName = "instructions")]
        public List<PayoutInstruction> Instructions { get; set; }
        public bool ShouldSerializeInstructions() { return (Instructions != null && Instructions.Count > 0); }

        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }
        public bool ShouldSerializeAmount() { return true; }

        String _currency = "";
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
        public bool ShouldSerializePricingMethod() { return !String.IsNullOrEmpty(PricingMethod); }

        [JsonProperty(PropertyName = "notificationEmail")]
        public string NotificationEmail { get; set; }
        public bool ShouldSerializeNotificationEmail() { return !String.IsNullOrEmpty(NotificationEmail); }

        [JsonProperty(PropertyName = "notificationURL")]
        public string NotificationURL { get; set; }
        public bool ShouldSerializeNotificationURL() { return !String.IsNullOrEmpty(NotificationURL); }

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
