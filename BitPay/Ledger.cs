using System;
using System.Collections.Generic;

namespace BitPayAPI
{
    public class Ledger
    {
        public const String LEDGER_AUD = "AUD";
        public const String LEDGER_BTC = "BTC";
        public const String LEDGER_CAD = "CAD";
        public const String LEDGER_EUR = "EUR";
        public const String LEDGER_GBP = "GBP";
        public const String LEDGER_MXN = "MXN";
        public const String LEDGER_NDZ = "NDZ";
        public const String LEDGER_USD = "USD";
        public const String LEDGER_ZAR = "ZAR";

        public List<LedgerEntry> Entries = null;

        /// <summary>
        /// Creates a Ledger object.
        /// </summary>
        public Ledger(List<LedgerEntry> entries)
        {
            Entries = entries;
        }
    }
}
