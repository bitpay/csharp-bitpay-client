// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Bill
{
    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string? Description { get; set; }

        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }
        
        public Item(decimal price, int quantity)
        {
            Price = price;
            Quantity = quantity;
        }

        public bool ShouldSerializeId()
        {
            return false;
        }
    }
}
