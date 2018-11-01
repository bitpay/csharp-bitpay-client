using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitPayAPI.Models
{
    /// <summary>
    /// Provides an interface to the BitPay server for the token resource.
    /// </summary>
    public class Token
    {
     
        // API fields
        //

        [JsonProperty(PropertyName = "guid")]
        public string Guid { get; set; }
        public bool ShouldSerializeGuid() { return true; }

        [JsonProperty(PropertyName = "nonce")]
        public long Nonce { get; set; }
        public bool ShouldSerializeNonce() { return Nonce != 0; }

        // Required fields
        //

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public bool ShouldSerializeId() { return !String.IsNullOrEmpty(Id); }

        // Optional fields

        [JsonProperty(PropertyName = "pairingCode")]
        public string PairingCode { get; set; }
        public bool ShouldSerializePairingCode() { return !String.IsNullOrEmpty(PairingCode); }

        [JsonProperty(PropertyName = "facade")]
        public string Facade { get; set; }
        public bool ShouldSerializeFacade() { return !String.IsNullOrEmpty(Facade); }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }
        public bool ShouldSerializeLabel() { return !String.IsNullOrEmpty(Label); }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        public bool ShouldSerializeCount() { return Count != 0; }

        // Response fields
        //

        [JsonProperty(PropertyName = "pairingExpiration")]
        public long PairingExpiration { get; set; }
        public bool ShouldSerializePairingExpiration() { return false; }

        [JsonProperty(PropertyName = "policies")]
        public List<Policy> Policies { get; set; }
        public bool ShouldSerializePolicies() { return false; }

        [JsonProperty(PropertyName = "resource")]
        public string Resource { get; set; }
        public bool ShouldSerializeResource() { return false; }

        [JsonProperty(PropertyName = "token")]
        public string Value { get; set; }
        public bool ShouldSerializeValue() { return false; }

        [JsonProperty(PropertyName = "dateCreated")]
        public long DateCreated { get; set; }
        public bool ShouldSerializeDateCreated() { return false; }
    }
}
