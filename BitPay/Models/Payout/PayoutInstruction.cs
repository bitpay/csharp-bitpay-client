using System;
using System.Collections.Generic;
using BitPay.Exceptions;
using Newtonsoft.Json;

namespace BitPay.Models.Payout
{
    public class PayoutInstruction
    {
        /// <summary>
        ///     Constructor, create an empty PayoutInstruction object.
        /// </summary>
        public PayoutInstruction()
        {
        }

        /**
         * Constructor, create a PayoutInstruction object.
         *
         * @param amount      float amount (in currency of batch).
         * @param method      int Method used to target the recipient.
         * @param methodValue string value for the choosen target method.
         * @throws PayoutCreationException BitPayException class
         */
        public PayoutInstruction(decimal amount, int method, string methodValue)
        {
            try
            {
                Amount = amount;
                switch (method) {
                    case RecipientReferenceMethod.EMAIL:
                        Email = methodValue;
                        break;
                    case RecipientReferenceMethod.RECIPIENT_ID:
                        RecipientId = methodValue;
                        break;
                    case RecipientReferenceMethod.SHOPPER_ID:
                        ShopperId = methodValue;
                        break;
                    default:
                        throw new PayoutCreationException();
                }
            }
            catch (Exception e)
            {
                throw new BitPayException(e);
            }
        }

        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "recipientId")]
        public string RecipientId { get; set; }

        [JsonProperty(PropertyName = "shopperId")]
        public string ShopperId { get; set; }

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

        public bool ShouldSerializeEmail()
        {
            return !string.IsNullOrEmpty(Email);
        }

        public bool ShouldSerializeRecipientId()
        {
            return !string.IsNullOrEmpty(RecipientId);
        }

        public bool ShouldSerializeShopperId()
        {
            return !string.IsNullOrEmpty(ShopperId);
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
