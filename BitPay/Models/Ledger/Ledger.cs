using System.Collections.Generic;

namespace BitPayAPI.Models.Ledger
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