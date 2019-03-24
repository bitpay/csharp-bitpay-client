using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitPayAPI.Models
{
	public enum LEDGER_ITEM_TYPE { Invoice=1000, Fee=1001, Payment=1002, Other=1003,FeeRefund=1004, Deposit=1005, Exchange=1006, ExchangeFee=1007, PlanCharge=1008,
		PlanCredit =1009, AccountSettlement= 1017,InvoiceFee = 1023, BitcoinDeposit = 1024, };
	public class LedgerEntry
    {
		public LEDGER_ITEM_TYPE type => (LEDGER_ITEM_TYPE)Code;
		public LedgerEntry() {}

        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

		public decimal TrueAmount {
			get => Amount / Scale; set => Amount = value * Scale;
		}

        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "scale")]
        public decimal Scale { get; set; }

        [JsonProperty(PropertyName = "txType")]
        public string TxType { get; set; }

        [JsonProperty(PropertyName = "sale")]
        public string Sale { get; set; }

        [JsonProperty(PropertyName = "buyer")]
        public Buyer Buyer { get; set; }

        [JsonProperty(PropertyName = "invoiceId")]
        public string InvoiceId { get; set; }

		
		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }

		[JsonProperty(PropertyName = "sourceType")]
        public string SourceType { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<string, string> CustomerData { get; set; }

        [JsonProperty(PropertyName = "invoiceAmount")]
        public double InvoiceAmount { get; set; }

        [JsonProperty(PropertyName = "invoiceCurrency")]
        public string InvoiceCurrency { get; set; }

        [JsonProperty(PropertyName = "transactionCurrency")]
        public string TransactionCurrency { get; set; }
    }
}
