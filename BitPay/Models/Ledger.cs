using System;
using System.Collections.Generic;

namespace BitPayAPI.Models
{
    public class Ledger
    {
        public const String LedgerAud = "AUD";
        public const String LedgerBtc = "BTC";
        public const String LedgerCad = "CAD";
        public const String LedgerEur = "EUR";
        public const String LedgerGbp = "GBP";
        public const String LedgerMxn = "MXN";
        public const String LedgerNdz = "NDZ";
        public const String LedgerUsd = "USD";
        public const String LedgerZar = "ZAR";

        public List<LedgerEntry> Entries;

        /// <summary>
        /// Creates a Ledger object.
        /// </summary>
        public Ledger(List<LedgerEntry> entries)
        {
            Entries = entries;
        }
    }
}
