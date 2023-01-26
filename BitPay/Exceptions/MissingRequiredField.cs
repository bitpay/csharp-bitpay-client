namespace BitPay.Exceptions
{
    public class MissingRequiredField : BitPayException
    {
        private const string BitPayCode = "BITPAY-GENERIC";
        private const string BitPayMessage = "Missing required field";
        
        public MissingRequiredField(string fieldName) : base(BitPayCode, BitPayMessage + " " + fieldName)
        {
        }
    }
}