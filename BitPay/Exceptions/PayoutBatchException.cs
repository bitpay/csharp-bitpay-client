using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutBatchException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the payout batch";
        private readonly string _bitpayCode = "BITPAY-PAYOUT-BATCH-GENERIC";

        public PayoutBatchException() : base(BitPayMessage)
        {
            BitpayCode = _bitpayCode;
        }

        public PayoutBatchException(Exception ex) : base(BitPayMessage, ex)
        {
            BitpayCode = _bitpayCode;
        }

        public PayoutBatchException(string bitpayCode, string message) : base(bitpayCode, message)
        {
        }

        public PayoutBatchException(string bitpayCode, string message, Exception cause) : base(bitpayCode, message, cause)
        {
        }
    }
}
