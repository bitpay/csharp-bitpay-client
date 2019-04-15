using System.Collections.Generic;

namespace BitPayAPI.Models
{
    public class Ledger
    {
        public const string LedgerAud = "AUD";
        public const string LedgerBtc = "BTC";
        public const string LedgerCad = "CAD";
        public const string LedgerEur = "EUR";
        public const string LedgerGbp = "GBP";
        public const string LedgerMxn = "MXN";
        public const string LedgerNdz = "NDZ";
        public const string LedgerUsd = "USD";
        public const string LedgerZar = "ZAR";

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