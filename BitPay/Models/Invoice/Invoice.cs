using System;
using System.Collections.Generic;
using BitPaySDK.Exceptions;
using Newtonsoft.Json;

namespace BitPaySDK.Models.Invoice
{
    public class Invoice
    {
        private string _currency = "";
        private dynamic _exchangeRates;
        private dynamic _refundAddresses;

        /// <summary>
        ///     Creates an uninitialized invoice request object.
        /// </summary>
        public Invoice()
        {
        }

        // Creates a minimal invoice request object.
        public Invoice(double price, string currency)
        {
            Price = price;
            Currency = currency;
        }

        // API fields
        //

        [JsonProperty(PropertyName = "guid")] public string Guid { get; set; }

        [JsonProperty(PropertyName = "token")] public string Token { get; set; }

        // Required fields
        //

        [JsonProperty(PropertyName = "price")] public double Price { get; set; }

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

        // Optional fields
        //

        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }

        [JsonProperty(PropertyName = "itemDesc")]
        public string ItemDesc { get; set; }

        [JsonProperty(PropertyName = "itemCode")]
        public string ItemCode { get; set; }

        [JsonProperty(PropertyName = "posData")]
        public string PosData { get; set; }

        [JsonProperty(PropertyName = "notificationURL")]
        public string NotificationUrl { get; set; }

        [JsonProperty(PropertyName = "transactionSpeed")]
        public string TransactionSpeed { get; set; }

        [JsonProperty(PropertyName = "fullNotifications")]
        public bool FullNotifications { get; set; }

        [JsonProperty(PropertyName = "extendedNotifications")]
        public bool ExtendedNotifications { get; set; }

        [JsonProperty(PropertyName = "notificationEmail")]
        public string NotificationEmail { get; set; }

        [JsonProperty(PropertyName = "redirectURL")]
        public string RedirectUrl { get; set; }

        [JsonProperty(PropertyName = "physical")]
        public bool Physical { get; set; }

        [JsonProperty(PropertyName = "paymentCurrencies")]
        public List<string> PaymentCurrencies { get; set; }

        [JsonProperty(PropertyName = "acceptanceWindow")]
        public long AcceptanceWindow { get; set; }

        // Buyer data
        //

        [JsonProperty(PropertyName = "buyer")] public Buyer Buyer { get; set; }

        // Response fields
        //

        public string Id { get; set; }

        public string Url { get; set; }

        public string Status { get; set; }

        public string LowFeeDetected { get; set; }

        public long InvoiceTime { get; set; }

        public long ExpirationTime { get; set; }

        public long CurrentTime { get; set; }

        public int TargetConfirmations { get; set; }

        public List<InvoiceTransaction> Transactions { get; set; }

        public string ExceptionStatus { get; set; }

        public dynamic RefundAddresses
        {
            get => _refundAddresses;
            set => _refundAddresses = JsonConvert.DeserializeObject(value.ToString(Formatting.None));
        }

        public string RefundAddressRequestPending { get; set; }

        public string BuyerProvidedEmail { get; set; }

        public InvoiceBuyerProvidedInfo BuyerProvidedInfo { get; set; }

        public SupportedTransactionCurrencies SupportedTransactionCurrencies { get; set; }

        public Shopper Shopper { get; set; }

        public MinerFees MinerFees { get; set; }

        public string TransactionCurrency { get; set; }

        public string BillId { get; set; }

        public RefundInfo RefundInfo { get; set; }

        private PaymentCodes _paymentCodes = null; //TODO remove on version 4.0

        [Obsolete("PaymentCodes will be deprecated on version 4.0", false)]
        public PaymentCodes PaymentCodes
        {
            get => _paymentCodes;
            set => _paymentCodes = null;
        }

        private PaymentTotal _paymentSubtotals = null; //TODO remove on version 4.0

        [Obsolete("PaymentSubtotals will be deprecated on version 4.0", false)]
        public PaymentTotal PaymentSubtotals
        {
            get => _paymentSubtotals;
            set => _paymentSubtotals = null;
        }

        private PaymentTotal _paymentTotals = null; //TODO remove on version 4.0

        [Obsolete("PaymentTotals will be deprecated on version 4.0", false)]
        public PaymentTotal PaymentTotals
        {
            get => _paymentTotals;
            set => _paymentTotals = null;
        }

        private PaymentTotal _paymentDisplayTotals = null; //TODO remove on version 4.0

        [Obsolete("PaymentDisplayTotals will be deprecated on version 4.0", false)]
        public PaymentTotal PaymentDisplayTotals
        {
            get => _paymentDisplayTotals;
            set => _paymentDisplayTotals = null;
        }

        private PaymentTotal _paymentDisplaySubTotals = null; //TODO remove on version 4.0

        [Obsolete("PaymentDisplaySubTotals will be deprecated on version 4.0", false)]
        public PaymentTotal PaymentDisplaySubTotals
        {
            get => _paymentDisplaySubTotals;
            set => _paymentDisplaySubTotals = null;
        }

        public double AmountPaid { get; set; }

        public dynamic ExchangeRates
        {
            get => _exchangeRates;
            set => _exchangeRates = JsonConvert.DeserializeObject(value.ToString(Formatting.None));
        }

        public bool ShouldSerializeOrderId()
        {
            return !string.IsNullOrEmpty(OrderId);
        }

        public bool ShouldSerializeItemDesc()
        {
            return !string.IsNullOrEmpty(ItemDesc);
        }

        public bool ShouldSerializeItemCode()
        {
            return !string.IsNullOrEmpty(ItemCode);
        }

        public bool ShouldSerializePosData()
        {
            return !string.IsNullOrEmpty(PosData);
        }

        public bool ShouldSerializeNotificationUrl()
        {
            return !string.IsNullOrEmpty(NotificationUrl);
        }

        public bool ShouldSerializeTransactionSpeed()
        {
            return !string.IsNullOrEmpty(TransactionSpeed);
        }

        public bool ShouldSerializeFullNotifications()
        {
            return FullNotifications;
        }

        public bool ShouldSerializeExtendedNotifications()
        {
            return ExtendedNotifications;
        }

        public bool ShouldSerializeNotificationEmail()
        {
            return !string.IsNullOrEmpty(NotificationEmail);
        }

        public bool ShouldSerializeRedirectUrl()
        {
            return !string.IsNullOrEmpty(RedirectUrl);
        }

        public bool ShouldSerializePhysical()
        {
            return Physical;
        }

        public bool ShouldSerializePaymentCurrencies()
        {
            return (PaymentCurrencies != null);
        }

        public bool ShouldSerializeBuyer()
        {
            return (Buyer != null);
        }

        public bool ShouldSerializeAcceptanceWindow()
        {
            return (AcceptanceWindow > 0);
        }

        public bool ShouldSerializeId()
        {
            return false;
        }

        public bool ShouldSerializeUrl()
        {
            return false;
        }

        public bool ShouldSerializeStatus()
        {
            return false;
        }

        public bool ShouldSerializeLowFeeDetected()
        {
            return false;
        }

        public bool ShouldSerializePaymentSubtotals()
        {
            return false;
        }

        public bool ShouldSerializeInvoiceTime()
        {
            return false;
        }

        public bool ShouldSerializeExpirationTime()
        {
            return false;
        }

        public bool ShouldSerializeCurrentTime()
        {
            return false;
        }

        public bool ShouldSerializeAmountPaid()
        {
            return false;
        }

        public bool ShouldSerializePaymentTotals()
        {
            return false;
        }

        public bool ShouldSerializePaymentDisplayTotals()
        {
            return false;
        }

        public bool ShouldSerializePaymentDisplaySubTotals()
        {
            return false;
        }

        public bool ShouldSerializeExchangeRates()
        {
            return false;
        }

        public bool ShouldSerializeSupportedTransactionCurrencies()
        {
            return false;
        }

        public bool ShouldSerializeShopper()
        {
            return false;
        }

        public bool ShouldSerializeMinerFees()
        {
            return false;
        }

        public bool ShouldSerializeTransactionCurrency()
        {
            return !string.IsNullOrEmpty(TransactionCurrency);
        }

        public bool ShouldSerializeBillId()
        {
            return false;
        }

        public bool ShouldSerializeRefundInfo()
        {
            return false;
        }

        public bool ShouldSerializePaymentCodes()
        {
            return false;
        }

        public bool ShouldIgnorePaymentCodes()
        {
            return true;
        }

        public bool ShouldSerializeTargetConfirmations()
        {
            return false;
        }

        public bool ShouldSerializeTransactions()
        {
            return false;
        }

        public bool ShouldSerializeExceptionStatus()
        {
            return false;
        }

        public bool ShouldSerializeRefundAddresses()
        {
            return false;
        }

        public bool ShouldSerializeRefundAddressRequestPending()
        {
            return false;
        }

        public bool ShouldSerializeBuyerProvidedEmail()
        {
            return false;
        }

        public bool ShouldSerializeBuyerProvidedInfo()
        {
            return false;
        }
    }
}