using System;

namespace BitPayAPI
{
    /// <summary>
    /// Provides an interface to the BitPay server for the access key resource.
    /// </summary>
    public class AccessKey
    {
        /// <summary>
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
        /// Constructor.  Initializes the access key object.
        /// </summary>
        /// <param name="obj">A decoded JSON object.</param>
	    public AccessKey(dynamic obj)
        {
            this.id = (string)obj.data.id;
            this.label = (string)obj.data.label;
            this.approved = Convert.ToBoolean(obj.data.approved);
            this.token = (string)obj.data.token;
	    }
    }
}
