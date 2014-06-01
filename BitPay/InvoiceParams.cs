using System;
using System.Collections.Generic;

namespace BitPayAPI
{
    /// <summary>
    /// Provides BitPay invoice parameter handling.
    /// </summary>
    public class InvoiceParams
    {
        /// <summary>
        /// A pass through variable provided by the merchant and designed to be used by the merchant to correlate
        /// the invoice with an order or other object in their system.
        /// 
        /// This pass through variable can be a JSON-encoded string, for example
        ///    posData:'{"ref":711454,"affiliate":"spring112"}'
        /// </summary>
	    public string posData { get; set; }

        /// <summary>
        /// A URL to send status update messages to your server (this must be an https URL, unencrypted http
        /// URLs or any other type of URL is not supported).
        /// 
        /// Bitpay.com will send a POST request with a JSON encoding of the invoice to this URL when the invoice
        /// status changes.
        /// </summary>
	    public string notificationURL { get; set; }

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
	    public string transactionSpeed { get; set; }

        /// <summary>
        /// Default: false
        /// true: Notifications will be sent on every status change.
        /// false: Notifications are only sent when an invoice is confirmed (according the the transactionSpeed
        /// setting).
        /// </summary>
	    public bool   fullNotifications { get; set; }

        /// <summary>
        /// Bitpay.com will send an email to this email address when the invoice status changes.
        /// </summary>
	    public string notificationEmail { get; set; }

        /// <summary>
        /// This is the URL for a return link that is displayed on the receipt, to return the shopper back to
        /// your website after a successful purchase. This could be a page specific to the order, or to their
        /// account.
        /// </summary>
	    public string redirectURL { get; set; }

        /// <summary>
        /// Used to display your public order number to the buyer on the BitPay invoice. In the merchant Account 
        /// Summary page, this value is used to identify the ledger entry.
        /// </summary>
	    public string orderId { get; set; }

        /// <summary>
        /// Used to display an item description to the buyer.
        /// </summary>
	    public string itemDesc { get; set; }

        /// <summary>
        /// Used to display an item SKU code or part number to the buyer.
        /// </summary>
	    public string itemCode { get; set; }

        /// <summary>
        /// default value: false
        /// true : Indicates a physical item will be shipped (or picked up)
        /// false : Indicates that nothing is to be shipped for this order
        /// </summary>
	    public bool   physical { get; set; }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
	    public string buyerName { get; set; }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
	    public string buyerAddress1 { get; set; }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
	    public string buyerAddress2 { get; set; }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
	    public string buyerCity { get; set; }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
	    public string buyerState { get; set; }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
	    public string buyerZip { get; set; }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
	    public string buyerCountry { get; set; }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
	    public string buyerEmail { get; set; }

        /// <summary>
        /// Used for display purposes only and will be shown on the invoice if provided.
        /// </summary>
	    public string buyerPhone { get; set; }


        /// <summary>
        /// Get the entire set of invoice parameters in a dictionary.
        /// </summary>
        /// <returns>A list of key value pairs.</returns>
        public Dictionary<string, string> getDictionary()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("physical", this.physical.ToString().ToLower());
            parameters.Add("fullNotifications", this.fullNotifications.ToString().ToLower());
		
		    if (this.notificationURL != null) 
            {
                parameters.Add("notificationURL", this.notificationURL);
		    }

            if (this.transactionSpeed != null) 
            {
                parameters.Add("transactionSpeed", this.transactionSpeed);
		    }
		    
            if (this.posData != null) 
            {
                parameters.Add("posData", this.posData);
		    }
		    
            if (this.notificationEmail != null) 
            {
                parameters.Add("notificationEmail", this.notificationEmail);
		    }
		    
            if (this.redirectURL != null)
            {
                parameters.Add("redirectURL", this.redirectURL);
		    }
		    
            if (this.orderId != null) 
            {
                parameters.Add("orderID", this.orderId);
		    }
		    
            if (this.itemDesc != null) 
            {
                parameters.Add("itemDesc", this.itemDesc);
		    }
		    
            if (this.itemCode != null) 
            {
                parameters.Add("itemCode", this.itemCode);
		    }
		    
            if (this.buyerName != null) 
            {
                parameters.Add("buyerName", this.buyerName);
		    }
		    
            if (this.buyerAddress1 != null) 
            {
                parameters.Add("buyerAddress1", this.buyerAddress1);
		    }
		    
            if (this.buyerAddress2 != null) 
            {
                parameters.Add("buyerAddress2", this.buyerAddress2);
		    }
		    
            if (this.buyerCity != null) 
            {
                parameters.Add("buyerCity", this.buyerCity);
		    }
		    
            if (this.buyerState != null) 
            {
                parameters.Add("buyerState", this.buyerState);
		    }
		    
            if (this.buyerZip != null) 
            {
                parameters.Add("buyerZip", this.buyerZip);
		    }
		    
            if (this.buyerCountry != null) 
            {
                parameters.Add("buyerCountry", this.buyerCountry);
		    }
		    
            if (this.buyerEmail != null) 
            {
                parameters.Add("buyerEmail", this.buyerEmail);
		    }
		    
            if (this.buyerPhone != null) 
            {
                parameters.Add("buyerPhone", this.buyerPhone);
		    }
		    
            return parameters;
	    }
    }
}
