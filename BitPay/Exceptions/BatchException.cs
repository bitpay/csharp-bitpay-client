using System;

namespace BitPaySDK.Exceptions
{
    public class BatchException : BitPayException
    {
        private const string BitPayCode = "BITPAY-BATCH";
        private const string BitPayMessage = "Error when processing the batch";

        public BatchException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BatchException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}