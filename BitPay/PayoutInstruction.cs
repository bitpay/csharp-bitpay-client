using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace BitPayAPI
{
    public class PayoutInstruction
    {
        public const String STATUS_PAID = "paid";
	    public const String STATUS_UNPAID = "unpaid";
	
        /// <summary>
        /// Constructor, create an empty PayoutInstruction object.
        /// </summary>
        public PayoutInstruction() {}

        /// <summary>
        /// Constructor, create a PayoutInstruction object.
        /// </summary>
        /// <param name="amount">BTC amount.</param>
        /// <param name="address">Bitcoin address.</param>
        /// <param name="label">Label.</param>
        public PayoutInstruction(double amount, String address, String label)
        {
            Amount = amount;
            Address = address;
            Label = label;
        }

        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }
        public bool ShouldSerializeAmount() { return true; }

        [JsonProperty(PropertyName = "address")]
        public String Address { get; set; }
        public bool ShouldSerializeAddress() { return !String.IsNullOrEmpty(Address); }

        [JsonProperty(PropertyName = "label")]
        public String Label { get; set; }
        public bool ShouldSerializeLabel() { return !String.IsNullOrEmpty(Label); }

        // Response fields
        //

        public string Id { get; set; }
        public bool ShouldSerializeId() { return false; }

        public string Status { get; set; }
        public bool ShouldSerializeStatus() { return false; }

        public PayoutInstructionBtcSummary Btc { get; set; }
        public bool ShouldSerializeBtc() { return false; }

        public List <PayoutInstructionTransaction> Transactions { get; set; }
        public bool ShouldSerializeTransactions() { return false; }

    }
}
