using System;
using System.Collections.Generic;

namespace BitPayAPI
{
    /// <summary>
    /// Provides BitPay invoice parameter handling.
    /// </summary>
    public class InvoiceParams
    {
	    private string posData;
	    private string notificationURL;
	    private string transactionSpeed;
	    private bool   fullNotifications;
	    private string notificationEmail;
	    private string redirectURL;
	    private string orderId;
	    private string itemDesc;
	    private string itemCode;
	    private bool   physical;

	    private string buyerName;
	    private string buyerAddress1;
	    private string buyerAddress2;
	    private string buyerCity;
	    private string buyerState;
	    private string buyerZip;
	    private string buyerCountry;
	    private string buyerEmail;
	    private string buyerPhone;

        /// <summary>
        /// Constructor.
        /// </summary>
	    public InvoiceParams() 
        {
		    this.physical = false;
		    this.fullNotifications = false;
	    }

        /// <summary>
        /// A pass through variable provided by the merchant and designed to be used by the merchant to correlate
        /// the invoice with an order or other object in their system.
        /// 
	    /// This pass through variable can be a JSON-encoded string, for example
	    ///    posData:'{"ref":711454,"affiliate":"spring112"}'
        /// </summary>
        /// <returns>POS data.</returns>
	    public string getPosData()
        {
		    return posData;
	    }


        /// <summary>
        /// A pass through variable provided by the merchant and designed to be used by the merchant to correlate
        /// the invoice with an order or other object in their system.
        /// 
        /// This pass through variable can be a JSON-encoded string, for example
        ///    posData:'{"ref":711454,"affiliate":"spring112"}'
        /// </summary>
        /// <param name="posData">The data to pass through the BitPay server.</param>
        public void setPosData(string posData)
        {
		    this.posData = posData;
	    }

        /// <summary>
        /// A URL to send status update messages to your server (this must be an https URL, unencrypted http
        /// URLs or any other type of URL is not supported).
        /// 
        /// Bitpay.com will send a POST request with a JSON encoding of the invoice to this URL when the invoice
        /// status changes.
        /// </summary>
        /// <returns>The notifiction URL.</returns>
	    public string getNotificationURL()
        {
		    return notificationURL;
	    }

        /// <summary>
        /// A URL to send status update messages to your server (this must be an https URL, unencrypted http
        /// URLs or any other type of URL is not supported).
        /// 
        /// Bitpay.com will send a POST request with a JSON encoding of the invoice to this URL when the invoice
        /// status changes.
        /// </summary>
        /// <param name="notificationURL">The notificationURL.</param>
        public void setNotificationURL(string notificationURL)
        {
		    this.notificationURL = notificationURL;
	    }

        /// <summary>
        /// The transaction speed preference of an invoice determines when an invoice is confirmed.  For the
        /// high speed setting, it will confirmed as soon as full payment is received on the bitcoin network
        /// (note, the invoice will go from a status of new to confirmed, bypassing the paid status).  For
        /// the medium speed setting, the invoice is confirmed after the payment transaction(s) have been
        /// confrimed by 1 block on the bitcoin network.  For the low speed setting, 6 blocks on the bitcoin
        /// network are required.  Invoices are considered complete after 6 blocks on the bitcoin network,
        /// therefore an invoice will go from a paid status directly to a complete status if the transaction
        /// speed is set to low.
        ///
        /// default value: set in your https://bitpay.com/order-settings
        /// "high" : An invoice is considered to be "confirmed" immediately upon receipt of payment.
        /// "medium" : An invoice is considered to be "confirmed" after 1 block confirmation (~10 minutes).
        /// "low" : An invoice is considered to be "confirmed" after 6 block confirmations (~1 hour).
        /// 
        /// NOTE: Orders are posted to your Account Summary after 6 block confirmations regardless of this
        /// setting.
        /// </summary>
        /// <returns>The transaction speed; "high", "medium", or "low".</returns>
	    public string getTransactionSpeed() 
        {
		    return transactionSpeed;
	    }

        /// <summary>
        /// The transaction speed preference of an invoice determines when an invoice is confirmed.  For the
        /// high speed setting, it will confirmed as soon as full payment is received on the bitcoin network
        /// (note, the invoice will go from a status of new to confirmed, bypassing the paid status).  For
        /// the medium speed setting, the invoice is confirmed after the payment transaction(s) have been
        /// confrimed by 1 block on the bitcoin network.  For the low speed setting, 6 blocks on the bitcoin
        /// network are required.  Invoices are considered complete after 6 blocks on the bitcoin network,
        /// therefore an invoice will go from a paid status directly to a complete status if the transaction
        /// speed is set to low.
        ///
        /// default value: set in your https://bitpay.com/order-settings
        /// "high" : An invoice is considered to be "confirmed" immediately upon receipt of payment.
        /// "medium" : An invoice is considered to be "confirmed" after 1 block confirmation (~10 minutes).
        /// "low" : An invoice is considered to be "confirmed" after 6 block confirmations (~1 hour).
        /// 
        /// NOTE: Orders are posted to your Account Summary after 6 block confirmations regardless of this
        /// setting.
        /// </summary>
        /// <param name="transactionSpeed">Set to "high", "medium", or "low".</param>
	    public void setTransactionSpeed(String transactionSpeed)
        {
		    this.transactionSpeed = transactionSpeed;
	    }

        /// <summary>
        /// Default: false
        /// true: Notifications will be sent on every status change.
        /// false: Notifications are only sent when an invoice is confirmed (according the the transactionSpeed
        /// setting).
        /// </summary>
        /// <returns>Returns true if full notifications are on, otherwise false.</returns>
	    public bool isFullNotifications() 
        {
		    return fullNotifications;
	    }

        /// <summary>
        /// Default: false
        /// true: Notifications will be sent on every status change.
        /// false: Notifications are only sent when an invoice is confirmed (according the the transactionSpeed
        /// setting).
        /// </summary>
        /// <param name="fullNotifications">Set true for full notifications, otherwise set to false.</param>
	    public void setFullNotifications(bool fullNotifications) {
		    this.fullNotifications = fullNotifications;
	    }

        /// <summary>
        /// Bitpay.com will send an email to this email address when the invoice status changes.
        /// </summary>
        /// <returns>The notification email address.</returns>
	    public string getNotificationEmail() {
		    return notificationEmail;
	    }

        /// <summary>
        /// Bitpay.com will send an email to this email address when the invoice status changes.
        /// </summary>
        /// <param name="notificationEmail">The notification email address to set.</param>
        public void setNotificationEmail(string notificationEmail) 
        {
		    this.notificationEmail = notificationEmail;
	    }

        /// <summary>
        /// This is the URL for a return link that is displayed on the receipt, to return the shopper back to
        /// your website after a successful purchase. This could be a page specific to the order, or to their
        /// account.
        /// </summary>
        /// <returns>The redirect URL.</returns>
	    public string getRedirectURL()
        {
		    return redirectURL;
	    }

        /// <summary>
        /// This is the URL for a return link that is displayed on the receipt, to return the shopper back to
        /// your website after a successful purchase. This could be a page specific to the order, or to their
        /// account.
        /// </summary>
        /// <param name="redirectURL">The redirect URL to set.</param>
	    public void setRedirectURL(string redirectURL) 
        {
		    this.redirectURL = redirectURL;
	    }

        /// <summary>
        /// Used to display your public order number to the buyer on the BitPay invoice. In the merchant Account 
        /// Summary page, this value is used to identify the ledger entry.
        /// </summary>
        /// <returns>The order Id.</returns>
	    public string getOrderId()
        {
		    return orderId;
	    }

        /// <summary>
        /// Used to display your public order number to the buyer on the BitPay invoice. In the merchant Account 
        /// Summary page, this value is used to identify the ledger entry.
        /// </summary>
        /// <param name="orderId">The order Id to set.</param>
	    public void setOrderId(string orderId) 
        {
		    this.orderId = orderId;
	    }

        /// <summary>
        /// Used to display an item description to the buyer.
        /// </summary>
        /// <returns>The item description</returns>
	    public string getItemDesc() 
        {
		    return itemDesc;
	    }

        /// <summary>
        /// Used to display an item description to the buyer.
        /// </summary>
        /// <param name="itemDesc">A description of the item.</param>
	    public void setItemDesc(string itemDesc)
        {
		    this.itemDesc = itemDesc;
	    }

        /// <summary>
        /// Used to display an item SKU code or part number to the buyer.
        /// </summary>
        /// <returns>The item code.</returns>
	    public string getItemCode() 
        {
		    return itemCode;
	    }

        /// <summary>
        /// Used to display an item SKU code or part number to the buyer.
        /// </summary>
        /// <param name="itemCode">The item code to set.</param>
	    public void setItemCode(string itemCode) 
        {
		    this.itemCode = itemCode;
	    }

        /// <summary>
        /// default value: false
        /// true : Indicates a physical item will be shipped (or picked up)
        /// false : Indicates that nothing is to be shipped for this order
        /// </summary>
        /// <returns>True if the item is a physical item, false otherwise.</returns>
	    public bool isPhysical()
        {
		    return physical;
	    }

        /// <summary>
        /// default value: false
        /// true : Indicates a physical item will be shipped (or picked up)
        /// false : Indicates that nothing is to be shipped for this order
        /// </summary>
        /// <param name="physical">Set true is item is physical, false otherwise.</param>
	    public void setPhysical(bool physical)
        {
		    this.physical = physical;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <returns>The buyer name.</returns>
	    public string getBuyerName()
        {
		    return buyerName;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <param name="buyerName">The buyer name to set.</param>
	    public void setBuyerName(string buyerName)
        {
		    this.buyerName = buyerName;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <returns>The buyer address, line 1.</returns>
        public string getBuyerAddress1()
        {
            return buyerAddress1;
        }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <param name="buyerAddress1">The buyer address, line 1 to set.</param>
        public void setBuyerAddress1(string buyerAddress1)
        {
            this.buyerAddress1 = buyerAddress1;
        }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <returns>The buyer address, line 2.</returns>
	    public string getBuyerAddress2() 
        {
		    return buyerAddress2;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <param name="buyerAddress1">The buyer address, line 1 to set.</param>
	    public void setBuyerAddress2(string buyerAddress2)
        {
		    this.buyerAddress2 = buyerAddress2;
	    }


        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <returns>The buyer city.</returns>
	    public string getBuyerCity() 
        {
		    return buyerCity;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <param name="buyerCity">The buyer city to set.</param>
	    public void setBuyerCity(string buyerCity)
        {
		    this.buyerCity = buyerCity;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <returns>The buyer state.</returns>
	    public string getBuyerState() 
        {
		    return buyerState;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <param name="buyerState">The buyer state to set.</param>
	    public void setBuyerState(string buyerState) 
        {
		    this.buyerState = buyerState;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <returns>The buyer zip (postal) code.</returns>
	    public string getBuyerZip() 
        {
		    return buyerZip;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <param name="buyerZip">The buyer zip (postal) code to set.</param>
	    public void setBuyerZip(string buyerZip) 
        {
		    this.buyerZip = buyerZip;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <returns>The buyer country.</returns>
        public string getBuyerCountry()
        {
		    return buyerCountry;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <param name="buyerCountry">The buyer country to set.</param>
	    public void setBuyerCountry(string buyerCountry) 
        {
		    this.buyerCountry = buyerCountry;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <returns>The buyer email address.</returns>
	    public string getBuyerEmail() {
		    return buyerEmail;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <param name="buyerEmail">The buyer email address to set.</param>
	    public void setBuyerEmail(string buyerEmail) {
		    this.buyerEmail = buyerEmail;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <returns>The buyer phone number.</returns>
	    public string getBuyerPhone() 
        {
		    return buyerPhone;
	    }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
        /// <param name="buyerPhone">The buyer phone number to set.</param>
	    public void setBuyerPhone(string buyerPhone) 
        {
		    this.buyerPhone = buyerPhone;
	    }

        /// <summary>
        /// Get the entire set of invoice parameters
        /// </summary>
        /// <returns>A list of key value pairs.</returns>
	    public List<KeyValuePair<string, string>> getKeyValuePairs()
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(new KeyValuePair<string, string>("physical", this.physical.ToString().ToLower()));
            parameters.Add(new KeyValuePair<string, string>("fullNotifications", this.fullNotifications.ToString().ToLower()));
		
		    if (this.notificationURL != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("notificationURL", this.notificationURL));
		    }

            if (this.transactionSpeed != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("transactionSpeed", this.transactionSpeed));
		    }
		    
            if (this.posData != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("posData", this.posData));
		    }
		    
            if (this.notificationEmail != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("notificationEmail", this.notificationEmail));
		    }
		    
            if (this.redirectURL != null)
            {
                parameters.Add(new KeyValuePair<string, string>("redirectURL", this.redirectURL));
		    }
		    
            if (this.orderId != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("orderID", this.orderId));
		    }
		    
            if (this.itemDesc != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("itemDesc", this.itemDesc));
		    }
		    
            if (this.itemCode != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("itemCode", this.itemCode));
		    }
		    
            if (this.buyerName != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("buyerName", this.buyerName));
		    }
		    
            if (this.buyerAddress1 != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("buyerAddress1", this.buyerAddress1));
		    }
		    
            if (this.buyerAddress2 != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("buyerAddress2", this.buyerAddress2));
		    }
		    
            if (this.buyerCity != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("buyerCity", this.buyerCity));
		    }
		    
            if (this.buyerState != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("buyerState", this.buyerState));
		    }
		    
            if (this.buyerZip != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("buyerZip", this.buyerZip));
		    }
		    
            if (this.buyerCountry != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("buyerCountry", this.buyerCountry));
		    }
		    
            if (this.buyerEmail != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("buyerEmail", this.buyerEmail));
		    }
		    
            if (this.buyerPhone != null) 
            {
                parameters.Add(new KeyValuePair<string, string>("buyerPhone", this.buyerPhone));
		    }
		    
            return parameters;
	    }
    }
}
