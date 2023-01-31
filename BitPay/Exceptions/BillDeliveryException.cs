using System;

namespace BitPay.Exceptions
{
    public class BillDeliveryException : BillException
    {
        private const string BitPayCode = "BITPAY-BILL-Delivery";
        private const string BitPayMessage = "Failed to deliver bill";

        public BillDeliveryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BillDeliveryException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}