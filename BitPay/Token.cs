using System;
using System.Collections.Generic;

namespace BitPayAPI
{
    /// <summary>
    /// Provides an interface to the BitPay server for the token resource.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The facade.
        /// </summary>
        public string facade { get; set; }

        /// <summary>
        /// The token used to reference an existing object.
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// Update this instance with a JSON object from the BitPay server.
        /// </summary>
        /// <param name="obj">A decoded JSON object.</param>
        public Token updateWithObject(dynamic obj)
        {
            Dictionary<string, object>.KeyCollection kc = obj.GetDynamicMemberNames();

            if (kc.Count > 1)
            {
                Console.Out.WriteLine("Size of Token object is unexpected.  Expected one entry, got " + kc.Count + " entries.");
            }

            foreach (string key in kc)
            {
                this.facade = key;
                this.token = obj[key];
            }
            return this;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
	    public Token()
        {

	    }
    }
}
