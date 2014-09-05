using Newtonsoft.Json;
using System;
using System.Web.Script.Serialization;

namespace BitPayAPI
{
    public class Invoice
    {
        /// <summary>
        /// Creates an uninitialized invoice request object.
        /// </summary>
        public Invoice() {}

        // Creates a minimal inovice request object.
        public Invoice(double price, String currency)
        {
            Price = price;
            Currency = currency;
        }

        // API fields
        //

        [JsonProperty(PropertyName = "guid")]
        public string Guid { get; set; }

        [JsonProperty(PropertyName = "nonce")]
        public long Nonce { get; set; }
        public bool ShouldSerializeNonce() { return Nonce != 0; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        // Required fields
        //

        [JsonProperty(PropertyName = "price")]
        public double Price { get; set; }

        String _currency = "";
        [JsonProperty(PropertyName = "currency")]
        public string Currency
        {
            get { return _currency; }
            set
            {
                if (value.Length > 3)
                {
                    throw new ArgumentException("Must be a valid currency code");
                }
                _currency = value;
            }
        }

        // Optional fields
        //

        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }
        public bool ShouldSerializeOrderId() { return !String.IsNullOrEmpty(OrderId); }

        [JsonProperty(PropertyName = "itemDesc")]
        public string ItemDesc { get; set; }
        public bool ShouldSerializeItemDesc() { return !String.IsNullOrEmpty(ItemDesc); }

        [JsonProperty(PropertyName = "itemCode")]
        public string ItemCode { get; set; }
        public bool ShouldSerializeItemCode() { return !String.IsNullOrEmpty(ItemCode); }

        [JsonProperty(PropertyName = "posData")]
        public string PosData { get; set; }
        public bool ShouldSerializePosData() { return !String.IsNullOrEmpty(PosData); }

        [JsonProperty(PropertyName = "notificationURL")]
        public string NotificationURL { get; set; }
        public bool ShouldSerializeNotificationURL() { return !String.IsNullOrEmpty(NotificationURL); }

        [JsonProperty(PropertyName = "transactionSpeed")]
        public string TransactionSpeed { get; set; }
        public bool ShouldSerializeTransactionSpeed() { return !String.IsNullOrEmpty(TransactionSpeed); }

        [JsonProperty(PropertyName = "fullNotifications")]
        public bool FullNotifications { get; set; }
        public bool ShouldSerializeFullNotifications() { return FullNotifications; }

        [JsonProperty(PropertyName = "notificationEmail")]
        public string NotificationEmail { get; set; }
        public bool ShouldSerializeNotificationEmail() { return !String.IsNullOrEmpty(NotificationEmail); }

        [JsonProperty(PropertyName = "redirectURL")]
        public string RedirectURL { get; set; }
        public bool ShouldSerializeRedirectURL() { return !String.IsNullOrEmpty(RedirectURL); }

        [JsonProperty(PropertyName = "physical")]
        public bool Physical { get; set; }
        public bool ShouldSerializePhysical() { return Physical; }

        [JsonProperty(PropertyName = "buyerName")]
        public string BuyerName { get; set; }
        public bool ShouldSerializeBuyerName() { return !String.IsNullOrEmpty(BuyerName); }

        [JsonProperty(PropertyName = "buyerAddess1")]
        public string BuyerAddress1 { get; set; }
        public bool ShouldSerializeBuyerAddress1() { return !String.IsNullOrEmpty(BuyerAddress1); }

        [JsonProperty(PropertyName = "buyerAddess2")]
        public string BuyerAddress2 { get; set; }
        public bool ShouldSerializeBuyerAddress2() { return !String.IsNullOrEmpty(BuyerAddress2); }

        [JsonProperty(PropertyName = "buyerCity")]
        public string BuyerCity { get; set; }
        public bool ShouldSerializeBuyerCity() { return !String.IsNullOrEmpty(BuyerCity); }

        [JsonProperty(PropertyName = "buyerState")]
        public string BuyerState { get; set; }
        public bool ShouldSerializeBuyerState() { return !String.IsNullOrEmpty(BuyerState); }

        [JsonProperty(PropertyName = "buyerZip")]
        public string BuyerZip { get; set; }
        public bool ShouldSerializeBuyerZip() { return !String.IsNullOrEmpty(BuyerZip); }

        [JsonProperty(PropertyName = "buyerCountry")]
        public string BuyerCountry { get; set; }
        public bool ShouldSerializeBuyerCountry() { return !String.IsNullOrEmpty(BuyerCountry); }

        [JsonProperty(PropertyName = "buyerEmail")]
        public string BuyerEmail { get; set; }
        public bool ShouldSerializeBuyerEmail() { return !String.IsNullOrEmpty(BuyerEmail); }

        [JsonProperty(PropertyName = "buyerPhone")]
        public string BuyerPhone { get; set; }
        public bool ShouldSerializeBuyerPhone() { return !String.IsNullOrEmpty(BuyerPhone); }

        // Response fields
        //

        public string Id { get; set; }
        public bool ShouldSerializeId() { return false; }

        public string Url { get; set; }
        public bool ShouldSerializeUrl() { return false; }

        public string Status { get; set; }
        public bool ShouldSerializeStatus() { return false; }

        public double BtcPrice { get; set; }
        public bool ShouldSerializeBtcPrice() { return false; }

        public long InvoiceTime { get; set; }
        public bool ShouldSerializeInvoiceTime() { return false; }

        public long ExpirationTime { get; set; }
        public bool ShouldSerializeExpirationTime() { return false; }
        
        public long CurrentTime { get; set; }
        public bool ShouldSerializeCurrentTime() { return false; }

        public double BtcPaid { get; set; }
        public bool ShouldSerializeBtcPaid() { return false; }

        public decimal Rate { get; set; }
        public bool ShouldSerializeRate() { return false; }

        public string ExceptionStatus { get; set; }
        public bool ShouldSerializeExceptionStatus() { return false; }

        public InvoicePaymentUrls paymentUrls { get; set; }
        public bool ShouldSerializeExceptionInvoicePaymentUrls() { return false; }
    }
}
