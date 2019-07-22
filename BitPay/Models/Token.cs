using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitPaySDK.Models
{
    /// <summary>
    ///     Provides an interface to the BitPay server for the token resource.
    /// </summary>
    public class Token
    {
        // API fields
        //

        [JsonProperty(PropertyName = "guid")] public string Guid { get; set; }

        [JsonProperty(PropertyName = "nonce")] public long Nonce { get; set; }

        // Required fields
        //

        [JsonProperty(PropertyName = "id")] public string Id { get; set; }

        // Optional fields

        [JsonProperty(PropertyName = "pairingCode")]
        public string PairingCode { get; set; }

        [JsonProperty(PropertyName = "facade")]
        public string Facade { get; set; }

        [JsonProperty(PropertyName = "label")] public string Label { get; set; }

        [JsonProperty(PropertyName = "count")] public int Count { get; set; }

        // Response fields
        //

        [JsonProperty(PropertyName = "pairingExpiration")]
        public long PairingExpiration { get; set; }

        [JsonProperty(PropertyName = "policies")]
        public List<Policy> Policies { get; set; }

        [JsonProperty(PropertyName = "resource")]
        public string Resource { get; set; }

        [JsonProperty(PropertyName = "token")] public string Value { get; set; }

        [JsonProperty(PropertyName = "dateCreated")]
        public long DateCreated { get; set; }

        public bool ShouldSerializeGuid()
        {
            return true;
        }

        public bool ShouldSerializeNonce()
        {
            return Nonce != 0;
        }

        public bool ShouldSerializeId()
        {
            return !string.IsNullOrEmpty(Id);
        }

        public bool ShouldSerializePairingCode()
        {
            return !string.IsNullOrEmpty(PairingCode);
        }

        public bool ShouldSerializeFacade()
        {
            return !string.IsNullOrEmpty(Facade);
        }

        public bool ShouldSerializeLabel()
        {
            return !string.IsNullOrEmpty(Label);
        }

        public bool ShouldSerializeCount()
        {
            return Count != 0;
        }

        public bool ShouldSerializePairingExpiration()
        {
            return false;
        }

        public bool ShouldSerializePolicies()
        {
            return false;
        }

        public bool ShouldSerializeResource()
        {
            return false;
        }

        public bool ShouldSerializeValue()
        {
            return false;
        }

        public bool ShouldSerializeDateCreated()
        {
            return false;
        }
    }
}