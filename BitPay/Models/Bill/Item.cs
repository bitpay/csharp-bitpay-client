using Newtonsoft.Json;

namespace BitPaySDK.Models.Bill
{
    public class Item
    {
   
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "price")]
        public double Price { get; set; }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }

        public bool ShouldSerializeId()
        {
            return false;
        }
    }
}
