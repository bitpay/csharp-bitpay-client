using System;
using System.Collections.Generic;
using BitPayAPI.Exceptions;
using Newtonsoft.Json;

namespace BitPayAPI.Models.Invoice
{
    public class Invoice
    {
        public const string StatusNew = "new";
        public const string StatusPaid = "paid";
        public const string StatusConfirmed = "confirmed";
        public const string StatusComplete = "complete";
        public const string StatusInvalid = "invalid";
        public const string ExstatusFalse = "false";
        public const string ExstatusPaidOver = "paidOver";
        public const string ExstatusPaidPartial = "paidPartial";

        private string _currency = "";

        /// <summary>
        ///     Creates an uninitialized invoice request object.
        /// </summary>
        public Invoice()
        {
        }

        // Creates a minimal inovice request object.
        public Invoice(double price, string currency)
        {
            Price = price;
            Currency = currency;
        }

        // API fields
        //

        [JsonProperty(PropertyName = "guid")] public string Guid { get; set; }

        [JsonProperty(PropertyName = "nonce")] public long Nonce { get; set; }

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
                if (value.Length != 3)
                    throw new BitPayException("Error: currency code must be exactly three characters");

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

        [JsonProperty(PropertyName = "notificationEmail")]
        public string NotificationEmail { get; set; }

        [JsonProperty(PropertyName = "redirectURL")]
        public string RedirectUrl { get; set; }

        [JsonProperty(PropertyName = "physical")]
        public bool Physical { get; set; }

        [JsonProperty(PropertyName = "buyerName")]
        public string BuyerName { get; set; }

        [JsonProperty(PropertyName = "buyerAddress1")]
        public string BuyerAddress1 { get; set; }

        [JsonProperty(PropertyName = "buyerAddress2")]
        public string BuyerAddress2 { get; set; }

        [JsonProperty(PropertyName = "buyerCity")]
        public string BuyerCity { get; set; }

        [JsonProperty(PropertyName = "buyerState")]
        public string BuyerState { get; set; }

        [JsonProperty(PropertyName = "buyerZip")]
        public string BuyerZip { get; set; }

        [JsonProperty(PropertyName = "buyerCountry")]
        public string BuyerCountry { get; set; }

        [JsonProperty(PropertyName = "buyerEmail")]
        public string BuyerEmail { get; set; }

        [JsonProperty(PropertyName = "buyerPhone")]
        public string BuyerPhone { get; set; }

        // Response fields
        //

        public string Id { get; set; }

        public string Url { get; set; }

        public string Status { get; set; }

        [Obsolete("To be removed, use PaymentSubtotals")]
        public double BtcPrice { get; set; }

        public long InvoiceTime { get; set; }

        public long ExpirationTime { get; set; }

        public long CurrentTime { get; set; }

        [Obsolete("To be removed, use AmountPaid")]
        public double BtcPaid { get; set; }

        [Obsolete("To be removed, use PaymentTotals")]
        public double BtcDue { get; set; }

        public List<InvoiceTransaction> Transactions { get; set; }

        [Obsolete("To be removed, use ExchangeRates")]
        public double Rate { get; set; }

        [Obsolete("To be removed, use ExchangeRates")]
        public Dictionary<string, string> ExRates { get; set; }

        public string ExceptionStatus { get; set; }

        [Obsolete("To be removed, use PaymentCodes")]
        public InvoicePaymentUrls PaymentUrls { get; set; }

        [Obsolete("To be removed")] public bool Refundable => Flags.Refundable;

        [JsonProperty] private InvoiceFlags Flags { get; set; } = new InvoiceFlags();

        public string TransactionCurrency { get; set; }

        public SupportedTransactionsCurrencies SupportedTransactionsCurrencies { get; set; }

        public MinerFees MinerFees { get; set; }

        public PaymentCodes PaymentCodes { get; set; }

        public PaymentTotal PaymentSubtotals { get; set; }

        public PaymentTotal PaymentTotals { get; set; }

        public double AmountPaid { get; set; }

        public ExchangeRates ExchangeRates { get; set; }

        public bool ShouldSerializeNonce()
        {
            return Nonce != 0;
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

        public bool ShouldSerializeBuyerName()
        {
            return !string.IsNullOrEmpty(BuyerName);
        }

        public bool ShouldSerializeBuyerAddress1()
        {
            return !string.IsNullOrEmpty(BuyerAddress1);
        }

        public bool ShouldSerializeBuyerAddress2()
        {
            return !string.IsNullOrEmpty(BuyerAddress2);
        }

        public bool ShouldSerializeBuyerCity()
        {
            return !string.IsNullOrEmpty(BuyerCity);
        }

        public bool ShouldSerializeBuyerState()
        {
            return !string.IsNullOrEmpty(BuyerState);
        }

        public bool ShouldSerializeBuyerZip()
        {
            return !string.IsNullOrEmpty(BuyerZip);
        }

        public bool ShouldSerializeBuyerCountry()
        {
            return !string.IsNullOrEmpty(BuyerCountry);
        }

        public bool ShouldSerializeBuyerEmail()
        {
            return !string.IsNullOrEmpty(BuyerEmail);
        }

        public bool ShouldSerializeBuyerPhone()
        {
            return !string.IsNullOrEmpty(BuyerPhone);
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

        public bool ShouldSerializeBtcPrice()
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

        public bool ShouldSerializeBtcPaid()
        {
            return false;
        }

        public bool ShouldSerializeBtcDue()
        {
            return false;
        }

        public bool ShouldSerializeTransactions()
        {
            return false;
        }

        public bool ShouldSerializeRate()
        {
            return false;
        }

        public bool ShouldSerializeExRates()
        {
            return false;
        }

        public bool ShouldSerializeExceptionStatus()
        {
            return false;
        }

        public bool ShouldSerializePaymentUrls()
        {
            return false;
        }

        public bool ShouldSerializeRefundable()
        {
            return false;
        }

        public bool ShouldSerializeFlags()
        {
            return false;
        }
    }
}