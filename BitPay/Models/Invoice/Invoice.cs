using System;
using System.Collections.Generic;
using BitPayAPI.Exceptions;
using Newtonsoft.Json;

namespace BitPayAPI.Models.Invoice
{
    public class Invoice
    {
        private string _currency = "";

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

        [JsonProperty(PropertyName = "guid")]
        public string Guid { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        // Required fields
        //

        [JsonProperty(PropertyName = "price")]
        public double Price { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency
        {
            get => _currency;
            set
            {
                if (typeof(Currency).GetMember(value).Length == 0)
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

        [JsonProperty(PropertyName = "buyer")]
        public Buyer Buyer { get; set; }

        // Response fields
        //

        public string Id { get; set; }

        public string Url { get; set; }

        public string Status { get; set; }
        
        public string LowFeeDetected { get; set; }

        public long InvoiceTime { get; set; }

        public long ExpirationTime { get; set; }

        public long CurrentTime { get; set; }

        public List<InvoiceTransaction> Transactions { get; set; }

        public string ExceptionStatus { get; set; }
        
        public string RefundAddressRequestPending { get; set; }
        
        public InvoiceBuyerProvidedInfo BuyerProvidedInfo { get; set; }

        [Obsolete("To be removed")]
        private InvoiceFlags Flags { get; set; } = new InvoiceFlags();

        public SupportedTransactionCurrencies SupportedTransactionCurrencies { get; set; }

        public MinerFees MinerFees { get; set; }

        public PaymentCodes PaymentCodes { get; set; }

        public PaymentTotal PaymentSubtotals { get; set; }

        public PaymentTotal PaymentTotals { get; set; }

        public PaymentTotal PaymentDisplayTotals { get; set; }

        public PaymentTotal PaymentDisplaySubTotals { get; set; }

        public double AmountPaid { get; set; }

        public ExchangeRates ExchangeRates { get; set; }

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

        public bool ShouldSerializeAcceptanceWindow()
        {
            return true;
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

        public bool ShouldSerializeMinerFees()
        {
            return false;
        }

        public bool ShouldSerializePaymentCodes()
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

        public bool ShouldSerializeRefundAddressRequestPending()
        {
            return false;
        }

        public bool ShouldSerializeBuyerProvidedInfo()
        {
            return false;
        }

        public bool ShouldSerializeFlags()
        {
            return false;
        }
    }
}