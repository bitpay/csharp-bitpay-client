// Copyright (c) 2019 BitPay.
// All rights reserved.

using System.Collections.Generic;

using BitPay.Exceptions;

using Newtonsoft.Json;

namespace BitPay.Models.Bill
{
    public class Bill
    {
        private string _currency = "";

        // Creates a minimal bill request object.
        public Bill(string currency, string email, List<Item> items, string? number)
        {
            Number = number;
            Currency = currency;
            Email = email;
            Items = items;
        }

        // API fields
        //

        [JsonProperty(PropertyName = "token")]
        public string? Token { get; set; }

        // Required fields
        //

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
        
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "items")]
        public List<Item> Items { get; set; }

        // Optional fields
        //

        [JsonProperty(PropertyName = "number")]
        public string? Number { get; set; }
        
        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "address1")]
        public string? Address1 { get; set; }

        [JsonProperty(PropertyName = "address2")]
        public string? Address2 { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string? City { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string? State { get; set; }

        [JsonProperty(PropertyName = "zip")]
        public string? Zip { get; set; }
        
        [JsonProperty(PropertyName = "country")]
        public string? Country { get; set; }

        [JsonProperty(PropertyName = "cc")]
        public List<string>? Cc { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string? Phone { get; set; }

        [JsonProperty(PropertyName = "dueDate")]
        public string? DueDate { get; set; }

        [JsonProperty(PropertyName = "passProcessingFee")]
        public bool PassProcessingFee { get; set; }

        // Response fields
        //

        [JsonProperty(PropertyName = "status")]
        public string? Status { get; set; }
        
        [JsonProperty(PropertyName = "url")]
        public string? Url { get; set; }
        
        [JsonProperty(PropertyName = "createdDate")]
        public string? CreatedDate { get; set; }
        
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        
        [JsonProperty(PropertyName = "merchant")]
        public string? Merchant { get; set; }
        
        
        public bool ShouldSerializeItems()
        {
            return (Items != null && Items.Count > 0);
        }
        
        public bool ShouldSerializeNumber()
        {
            return !string.IsNullOrEmpty(Number);
        }

        public bool ShouldSerializeName()
        {
            return !string.IsNullOrEmpty(Name);
        }

        public bool ShouldSerializeAddress1()
        {
            return !string.IsNullOrEmpty(Address1);
        }

        public bool ShouldSerializeAddress2()
        {
            return !string.IsNullOrEmpty(Address2);
        }

        public bool ShouldSerializeCity()
        {
            return !string.IsNullOrEmpty(City);
        }

        public bool ShouldSerializeState()
        {
            return !string.IsNullOrEmpty(State);
        }

        public bool ShouldSerializeZip()
        {
            return !string.IsNullOrEmpty(Zip);
        }

        public bool ShouldSerializeCountry()
        {
            return !string.IsNullOrEmpty(Country);
        }

        public bool ShouldSerializeCc()
        {
            return (Cc != null);
        }

        public bool ShouldSerializePhone()
        {
            return !string.IsNullOrEmpty(Phone);
        }

        public bool ShouldSerializeDueDate()
        {
            return !string.IsNullOrEmpty(DueDate);
        }

        public bool ShouldSerializePassProcessingFee()
        {
            return PassProcessingFee;
        }

        public bool ShouldSerializeStatus()
        {
            return false;
        }

        public bool ShouldSerializeUrl()
        {
            return false;
        }

        public bool ShouldSerializeCreatedDate()
        {
            return false;
        }

        public bool ShouldSerializeId()
        {
            return false;
        }

        public bool ShouldSerializeMerchant()
        {
            return false;
        }
    }
}