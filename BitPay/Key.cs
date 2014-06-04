using System;

namespace BitPayAPI
{
    /// <summary>
    /// Provides an interface to the BitPay server for the access key resource.
    /// </summary>
    public class Key
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="obj">A JSON object.</param>
        public Key(dynamic obj)
        {
            this.id = (string)obj.id;
            this.label = (string)obj.label;
            this.approved = Convert.ToBoolean(obj.approved);
            this.token = (string)obj.token;
        }

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
        /// The API facade associated with this object.
        /// </summary>
        public string facade { get; set; }

        /// <summary>
        /// The API resource token for this object.
        /// </summary>
        public string token { get; set; }

    }
}
