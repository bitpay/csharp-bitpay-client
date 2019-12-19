using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitPaySDK.Models.Payout
{
    public class PayoutInstruction
    {
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
        public PayoutInstruction(double amount, string address)
        {
            Amount = amount;
            Address = address;
        }

        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "walletProvider")]
        public string WalletProvider { get; set; }

        // Response fields
        //

        public string Id { get; set; }

        public PayoutInstructionBtcSummary Btc { get; set; }

        public List<PayoutInstructionTransaction> Transactions { get; set; }

        public string Status { get; set; }

        public bool ShouldSerializeId()
        {
            return false;
        }

        public bool ShouldSerializeAmount()
        {
            return true;
        }

        public bool ShouldSerializeBtc()
        {
            return false;
        }

        public bool ShouldSerializeAddress()
        {
            return !string.IsNullOrEmpty(Address);
        }

        public bool ShouldSerializeTransactions()
        {
            return false;
        }

        public bool ShouldSerializeLabel()
        {
            return !string.IsNullOrEmpty(Label);
        }

        public bool ShouldSerializeStatus()
        {
            return false;
        }

        public bool ShouldSerializeWalletProvider()
        {
            return !string.IsNullOrEmpty(WalletProvider);
        }
    }
}