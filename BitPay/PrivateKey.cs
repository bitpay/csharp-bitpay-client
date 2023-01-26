namespace BitPay
{
    public class PrivateKey
    {
        private readonly string _value;

        public PrivateKey(string value)
        {
            _value = value;
        }

        public string Value()
        {
            return _value;
        }
    }
}