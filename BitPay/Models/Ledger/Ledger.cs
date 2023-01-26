using System.Collections.Generic;

namespace BitPay.Models.Ledger
{
    public class Ledger
    {

        public List<LedgerEntry> Entries;
        public string currency { get; set; }
        public double balance { get; set; }

        /// <summary>
        ///     Creates a Ledger object.
        /// </summary>
        public Ledger(List<LedgerEntry> entries)
        {
            Entries = entries;
        }
    }
}