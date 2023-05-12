// Copyright (c) 2019 BitPay.
// All rights reserved.

using System.Collections.Generic;

using BitPay.Converters;
using BitPay.Exceptions;

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class Invoice
    {
        private string _currency = "";
        private dynamic? _exchangeRates;
        private dynamic? _refundAddresses;
        
        // Creates a minimal invoice request object.
        public Invoice(decimal price, string currency)
        {
            Price = price;
            Currency = currency;
        }

        // API fields
        //

        [JsonProperty(PropertyName = "guid")] public string? ResourceGuid { get; set; }

        [JsonProperty(PropertyName = "token")] public string? Token { get; set; }

        // Required fields
        //

        [JsonProperty(PropertyName = "price")] public decimal Price { get; set; }

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
        public string? OrderId { get; set; }

        [JsonProperty(PropertyName = "itemDesc")]
        public string? ItemDesc { get; set; }

        [JsonProperty(PropertyName = "itemCode")]
        public string? ItemCode { get; set; }

        [JsonProperty(PropertyName = "posData")]
        public string? PosData { get; set; }

        [JsonProperty(PropertyName = "notificationURL")]
        public string? NotificationUrl { get; set; }

        [JsonProperty(PropertyName = "transactionSpeed")]
        public string? TransactionSpeed { get; set; }

        [JsonProperty(PropertyName = "fullNotifications")]
        public bool FullNotifications { get; set; }

        [JsonProperty(PropertyName = "autoRedirect")]
        public bool AutoRedirect { get; set; }

        [JsonProperty(PropertyName = "nonPayProPaymentReceived")]
        public bool NonPayProPaymentReceived { get; set; }

        [JsonProperty(PropertyName = "jsonPayProRequired")]
        public bool JsonPayProRequired { get; set; }

        [JsonProperty(PropertyName = "extendedNotifications")]
        public bool ExtendedNotifications { get; set; }

        [JsonProperty(PropertyName = "notificationEmail")]
        public string? NotificationEmail { get; set; }

        [JsonProperty(PropertyName = "redirectURL")]
        public string? RedirectUrl { get; set; }

        [JsonProperty(PropertyName = "physical")]
        public bool Physical { get; set; }

        [JsonProperty(PropertyName = "paymentCurrencies")]
        public List<string>? PaymentCurrencies { get; set; }

        [JsonProperty(PropertyName = "paymentString")]
        public string? PaymentString { get; set; }

        [JsonProperty(PropertyName = "verificationLink")]
        public string? VerificationLink { get; set; }

        [JsonProperty(PropertyName = "acceptanceWindow")]
        public long AcceptanceWindow { get; set; }

        [JsonProperty(PropertyName = "forcedBuyerSelectedTransactionCurrency")]
        public string? ForcedBuyerSelectedTransactionCurrency { get; set; }

        [JsonProperty(PropertyName = "forcedBuyerSelectedWallet")]
        public string? ForcedBuyerSelectedWallet { get; set; }

        [JsonProperty(PropertyName = "buyerEmail")]
        public string? BuyerEmail { get; set; }

        // Buyer data
        //

        [JsonProperty(PropertyName = "buyer")] public Buyer? Buyer { get; set; }

        // Response fields
        //

        [JsonProperty(PropertyName = "merchantName")]
        public string? MerchantName { get; set; }

        [JsonProperty(PropertyName = "selectedWallet")]
        public string? SelectedWallet { get; set; }

        [JsonProperty(PropertyName = "bitpayIdRequired")]
        public bool BitpayIdRequired { get; set; }

        [JsonProperty(PropertyName = "forceCancel")]
        public bool ForceCancel { get; set; }

        [JsonProperty(PropertyName = "isCancelled")]
        public bool IsCancelled { get; set; }

        public string? Id { get; set; }

        public string? Url { get; set; }

        public string? Status { get; set; }

        public bool LowFeeDetected { get; set; }

        public long InvoiceTime { get; set; }

        public long ExpirationTime { get; set; }

        public long CurrentTime { get; set; }

        public int TargetConfirmations { get; set; }

        public int UnderpaidAmount { get; set; }

        public int OverpaidAmount { get; set; }

        public List<InvoiceTransaction>? Transactions { get; set; }

        public List<ItemizedDetails>? ItemizedDetails { get; set; }

        public string? ExceptionStatus { get; set; }

        public dynamic? RefundAddresses
        {
            get => _refundAddresses;
            set => _refundAddresses = JsonConvert.DeserializeObject(value?.ToString(Formatting.None));
        }

        public bool RefundAddressRequestPending { get; set; }

        public string? BuyerProvidedEmail { get; set; }

        public InvoiceBuyerProvidedInfo? BuyerProvidedInfo { get; set; }

        public UniversalCodes? UniversalCodes { get; set; }
        
        [JsonConverter(typeof(SupportedTransactionCurrenciesConverter))]
        public SupportedTransactionCurrencies? SupportedTransactionCurrencies { get; set; }

        public Shopper? Shopper { get; set; }

        public MinerFees? MinerFees { get; set; }

        public string? TransactionCurrency { get; set; }

        public string? BillId { get; set; }

        public List<RefundInfo>? RefundInfo { get; set; }

        public decimal AmountPaid { get; set; }

        public string? DisplayAmountPaid { get; set; }

        public string? CloseURL { get; set; }

        public dynamic? ExchangeRates
        {
            get => _exchangeRates;
            set => _exchangeRates = JsonConvert.DeserializeObject(value?.ToString(Formatting.None));
        }

        public bool ShouldSerializeOrderId()
        {
            return !string.IsNullOrEmpty(OrderId);
        }

        public bool ShouldSerializePaymentString()
        {
            return !string.IsNullOrEmpty(PaymentString);
        }

        public bool ShouldSerializeVerificationLink()
        {
            return !string.IsNullOrEmpty(VerificationLink);
        }

        public bool ShouldSerializeSelectedWallet()
        {
            return !string.IsNullOrEmpty(SelectedWallet);
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

        public bool ShouldSerializeAutoRedirect()
        {
            return AutoRedirect;
        }

        public bool ShouldSerializeForceCancel()
        {
            return ForceCancel;
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

        public bool ShouldSerializeIsCancelled()
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

        public bool ShouldSerializeUnderpaidAmount()
        {
            return false;
        }

        public bool ShouldSerializeOverpaidAmount()
        {
            return false;
        }

        public bool ShouldSerializeTransactions()
        {
            return false;
        }

        public bool ShouldSerializeItemizedDetails()
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

        public bool ShouldSerializeNonPayProPaymentReceived()
        {
            return false;
        }

        public bool ShouldSerializeJsonPayProRequired()
        {
            return false;
        }

        public bool ShouldSerializeRefundAddressRequestPending()
        {
            return false;
        }

        public bool ShouldSerializeBitpayIdRequired()
        {
            return false;
        }

        public bool ShouldSerializeMerchantName()
        {
            return !string.IsNullOrEmpty(MerchantName);
        }

        public bool ShouldSerializeForcedBuyerSelectedTransactionCurrency()
        {
            return !string.IsNullOrEmpty(ForcedBuyerSelectedTransactionCurrency);
        }

        public bool ShouldSerializeForcedBuyerSelectedWallet()
        {
            return !string.IsNullOrEmpty(ForcedBuyerSelectedWallet);
        }

        public bool ShouldSerializeBuyerEmail()
        {
            return !string.IsNullOrEmpty(BuyerEmail);
        }

        public bool ShouldSerializeBuyerProvidedEmail()
        {
            return false;
        }

        public bool ShouldSerializeBuyerProvidedInfo()
        {
            return false;
        }

        public bool ShouldSerializeDisplayAmountPaid()
        {
            return !string.IsNullOrEmpty(DisplayAmountPaid);
        }

        public bool ShouldSerializeCloseURL()
        {
            return !string.IsNullOrEmpty(CloseURL);
        }
    }
}