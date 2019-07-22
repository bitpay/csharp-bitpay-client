using System.Collections.Generic;

namespace BitPaySDK.Models.Ledger
{
    public class Ledger
    {

        public List<LedgerEntry> Entries;

        /// <summary>
        ///     Creates a Ledger object.
        /// </summary>
        public Ledger(List<LedgerEntry> entries)
        {
            Entries = entries;
        }
    }
}