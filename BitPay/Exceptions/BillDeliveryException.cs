using System;

namespace BitPaySDK.Exceptions
{
    public class BillDeliveryException : BitPayException
    {
        private const string BitPayCode = "BITPAY-BILL-Delivery";
        private const string BitPayMessage = "Failed to deliver bill";

        public BillDeliveryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BillDeliveryException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}