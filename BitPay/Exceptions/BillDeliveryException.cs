// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class BillDeliveryException : BillException
    {
        private new const string BitPayCode = "BITPAY-BILL-Delivery";
        private const string BitPayMessage = "Failed to deliver bill";

        public BillDeliveryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BillDeliveryException(Exception ex, string apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected BillDeliveryException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}