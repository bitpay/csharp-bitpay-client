using System;
using System.Collections.Generic;
using System.Linq;
using BitPaySDK.Exceptions;
using Newtonsoft.Json;

namespace BitPaySDK.Models.Payout
{
    public class PayoutBatch
    {
        public const string MethodManual2 = "manual_2";
        public const string MethodVwap24 = "vwap_24hr";

        private string _currency = "";

        /// <summary>
        ///     Constructor, create an empty PayoutBatch object.
        /// </summary>
        public PayoutBatch()
        {
            Amount = 0.0;
            Currency = "USD";
            NotificationEmail = "";
            NotificationUrl = "";
            PricingMethod = MethodVwap24;
        }

        /// <summary>
        ///     Constructor, create an instruction-full request PayoutBatch object.
        /// </summary>
        /// <param name="currency">Currency to use on payout.</param>
        /// <param name="effectiveDate">
        ///     Date when request is effective. Note that the time of day will automatically be set to
        ///     09:00:00.000 UTC time for the given day. Only requests submitted before 09:00:00.000 UTC are guaranteed to be
        ///     processed on the same day.
        /// </param>
        /// <param name="instructions">Payout instructions.</param>
        public PayoutBatch(string currency, DateTime effectiveDate, List<PayoutInstruction> instructions) : this()
        {
            Currency = currency;
            EffectiveDate = effectiveDate;
            Instructions = instructions;
            _computeAndSetAmount();
        }

        // API fields
        //

        [JsonProperty(PropertyName = "guid")] public string Guid { get; set; }

        [JsonProperty(PropertyName = "token")] public string Token { get; set; }

        // Required fields
        //

        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency
        {
            get => _currency;
            set
            {
                if (!Models.Currency.isValid(value))
                    throw new BitPayException("Error: currency code must be a type of BitPayAPI.Models.Currency");

                _currency = value;
            }
        }

        [JsonProperty(PropertyName = "effectiveDate")]
        [JsonConverter(typeof(Converters.DateStringConverter))]
        public DateTime EffectiveDate { get; set; }

        [JsonProperty(PropertyName = "instructions")]
        public List<PayoutInstruction> Instructions { get; set; }

        // Optional fields
        //

        [JsonProperty(PropertyName = "reference")]
        public string Reference { get; set; }

        [JsonProperty(PropertyName = "notificationEmail")]
        public string NotificationEmail { get; set; }

        [JsonProperty(PropertyName = "notificationURL")]
        public string NotificationUrl { get; set; }

        [JsonProperty(PropertyName = "pricingMethod")]
        public string PricingMethod { get; set; }

        // Response fields
        //

        public string Id { get; set; }

        public string Account { get; set; }

        public string SupportPhone { get; set; }

        public string Status { get; set; }

        public double PercentFee { get; set; }

        public double Fee { get; set; }

        public double DepositTotal { get; set; }

        public double Rate { get; set; }

        public double Btc { get; set; }

        [JsonConverter(typeof(Converters.DateStringConverter))]
        public DateTime RequestDate { get; set; }

        [JsonConverter(typeof(Converters.DateStringConverter))]
        public DateTime DateExecuted { get; set; }

        // Private methods
        //

        private void _computeAndSetAmount()
        {
            var amount = 0.0;
            amount += Instructions.Select(i => i.Amount).Sum();
            Amount = amount;
        }

        public bool ShouldSerializeInstructions()
        {
            return Instructions != null && Instructions.Count > 0;
        }

        public bool ShouldSerializeAmount()
        {
            return true;
        }

        public bool ShouldSerializePricingMethod()
        {
            return !string.IsNullOrEmpty(PricingMethod);
        }

        public bool ShouldSerializeNotificationEmail()
        {
            return !string.IsNullOrEmpty(NotificationEmail);
        }

        public bool ShouldSerializeNotificationUrl()
        {
            return !string.IsNullOrEmpty(NotificationUrl);
        }

        public bool ShouldSerializeId()
        {
            return false;
        }

        public bool ShouldSerializeAccount()
        {
            return false;
        }

        public bool ShouldSerializeStatus()
        {
            return false;
        }

        public bool ShouldSerializeRate()
        {
            return false;
        }

        public bool ShouldSerializeBtc()
        {
            return false;
        }

        public bool ShouldSerializeRequestDate()
        {
            return false;
        }

        public bool ShouldSerializePercentFee()
        {
            return false;
        }

        public bool ShouldSerializeFee()
        {
            return false;
        }

        public bool ShouldSerializeDepositTotal()
        {
            return false;
        }

        public bool ShouldSerializeSupportPhone()
        {
            return false;
        }

        public bool ShouldSerializeReference()
        {
            return !string.IsNullOrEmpty(Reference);
        }

        public bool ShouldSerializeDateExecuted()
        {
            return false;
        }
    }
}