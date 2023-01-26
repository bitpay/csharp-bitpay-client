namespace BitPay
{
    public class PosToken
    {
        private readonly string _value;

        public PosToken(string value)
        {
            _value = value;
        }

        public string Value()
        {
            return _value;
        }
    }
}