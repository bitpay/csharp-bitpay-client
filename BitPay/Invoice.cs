using System;

namespace BitPayAPI
{
    /// <summary>
    /// Provides an interface to the BitPay server to create an invoice for payment.
    /// </summary>
    public class Invoice
    {
        /// <summary>
        /// The unique id of the invoice assigned by bitpay.com.
        /// </summary>
        /// 
        public string id { get; set; }

        /// <summary>
        /// An https URL where the invoice can be viewed.
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// The current invoice status.
        /// 
        /// "new" An invoice starts in this state.  When in this state and only in this state, payments to the
        /// associated bitcoin address are credited to the invoice.  If an invoice has received a partial
        /// payment, it will still reflect a status of new to the merchant (from a merchant system perspective, 
        /// an invoice is either paid or not paid, partial payments and over payments are handled by bitpay.com
        /// by either refunding the customer or applying the funds to a new invoice.
        /// 
        /// "paid" As soon as full payment (or over payment) is received, an invoice goes into the paid status.
        /// 
        /// "confirmed" The transaction speed preference of an invoice determines when an invoice is confirmed.
        /// For the high speed setting, it will confirmed as soon as full payment is received on the bitcoin
        /// network (note, the invoice will go from a status of new to confirmed, bypassing the paid status).
        /// For the medium speed setting, the invoice is confirmed after the payment transaction(s) have been
        /// confrimed by 1 block on the bitcoin network.  For the low speed setting, 6 blocks on the bitcoin
        /// network are required.  Invoices are considered complete after 6 blocks on the bitcoin network, 
        /// therefore an invoice will go from a paid status directly to a complete status if the transaction
        /// speed is set to low.
        /// 
        /// "complete" When an invoice is complete, it means that BitPay.com has credited the merchant’s account
        /// for the invoice.  Currently, 6 confirmation blocks on the bitcoin network are required for an invoice
        /// to be complete.  Note, in the future (for qualified payers), invoices may move to a complete status 
        /// immediately upon payment, in which case the invoice will move directly from a new status to a 
        /// complete status.
        /// 
        /// "expired" An expired invoice is one where payment was not received and the 15 minute payment window
        /// has elapsed.
        /// 
        /// "invalid" An invoice is considered invalid when it was paid, but payment was not confirmed within 1
        /// hour after receipt.  It is possible that some transactions on the bitcoin network can take longer 
        /// than 1 hour to be included in a block.  In such circumstances, once payment is confirmed, BitPay.com
        /// will make arrangements with the merchant regarding the funds (which can either be credited to the
        /// merchant account on another invoice, or returned to the buyer).
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// The amount of bitcoins being requested for payment of this invoice (same as the price if the
        /// merchant set the price in BTC).
        /// </summary>
        public double btcPrice { get; set; }

        /// <summary>
        /// The price set by the merchant (in terms of the provided currency).
        /// </summary>
        public double price { get; set; }

        /// <summary>
        /// The 3 letter currency code in which the invoice was priced.
        /// </summary>
        public string currency { get; set; }
	
        /// <summary>
        /// Constructor.  Initializes the invoice object.
        /// </summary>
        /// <param name="obj">A decoded JSON object.</param>
	    public Invoice(dynamic obj)
        {
            this.id = (string)obj.id;
            this.url = (string)obj.url;
            this.status = (string)obj.status;
            this.btcPrice = Convert.ToDouble(obj.btcPrice);
            this.price = Convert.ToDouble(obj.price);
            this.currency = (string)obj.currency;
	    }

    }
}
