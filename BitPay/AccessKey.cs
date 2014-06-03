using System;

namespace BitPayAPI
{
    /// <summary>
    /// Provides an interface to the BitPay server for the access key resource.
    /// </summary>
    public class AccessKey
    {
        /// <summary>
        /// <summary>
        /// The facade.
        /// </summary>
        public string facade { get; set; }

        /// The SIN.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The user friendly label set by the requestor.
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// The approval status for this access key; true if this access key has been approved.
        /// </summary>
        public bool approved { get; set; }

        /// <summary>
        /// The API resource token for this object.
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// Update this instance with a JSON object from the BitPay server.
        /// </summary>
        /// <param name="obj">A decoded JSON object.</param>
        public AccessKey updateWithObject(dynamic obj)
        {
            this.id = (string)obj.id;
            this.label = (string)obj.label;
            this.approved = Convert.ToBoolean(obj.approved);
            this.token = (string)obj.token;
            return this;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="facade">The facade to which this access key applies.</param>
        public AccessKey(string facade)
        {
            this.facade = facade;
        }
    }
}
