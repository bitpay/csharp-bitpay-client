using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitPayAPI.Models.Payout
{
    public class PayoutInstruction
    {
        public const string StatusPaid = "paid";
        public const string StatusUnpaid = "unpaid";

        /// <summary>
        ///     Constructor, create an empty PayoutInstruction object.
        /// </summary>
        public PayoutInstruction()
        {
        }

        /// <summary>
        ///     Constructor, create a PayoutInstruction object.
        /// </summary>
        /// <param name="amount">BTC amount.</param>
        /// <param name="address">Bitcoin address.</param>
        /// <param name="label">Label.</param>
        public PayoutInstruction(double amount, string address, string label)
        {
            Amount = amount;
            Address = address;
            Label = label;
        }

        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "label")] public string Label { get; set; }

        // Response fields
        //

        public string Id { get; set; }

        public string Status { get; set; }

        public PayoutInstructionBtcSummary Btc { get; set; }

        public List<PayoutInstructionTransaction> Transactions { get; set; }

        public bool ShouldSerializeAmount()
        {
            return true;
        }

        public bool ShouldSerializeAddress()
        {
            return !string.IsNullOrEmpty(Address);
        }

        public bool ShouldSerializeLabel()
        {
            return !string.IsNullOrEmpty(Label);
        }

        public bool ShouldSerializeId()
        {
            return false;
        }

        public bool ShouldSerializeStatus()
        {
            return false;
        }

        public bool ShouldSerializeBtc()
        {
            return false;
        }

        public bool ShouldSerializeTransactions()
        {
            return false;
        }
    }
}