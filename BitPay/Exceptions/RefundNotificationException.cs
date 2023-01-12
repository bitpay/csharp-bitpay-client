using System;

namespace BitPay.Exceptions
{
    public class RefundNotificationException : RefundException
    {
        private const string BitPayCode = "BITPAY-REFUND-NOTIFICATION";
        private const string BitPayMessage = "Failed to send refund notification";

        public RefundNotificationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundNotificationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}