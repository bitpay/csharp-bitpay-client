namespace BitPayAPI.Models.Invoice
{
    public static class Status
    {
        public const string New = "new";
        public const string Funded = "funded";
        public const string Processing = "processing";
        public const string Complete = "complete";
        public const string Failed = "failed";
        public const string Cancelled = "cancelled";
    }
}