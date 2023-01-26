namespace BitPay
{
    public class ConfigFilePath
    {
        private readonly string _value;

        public ConfigFilePath(string value)
        {
            _value = value;
        }

        public string Value()
        {
            return _value;
        }
    }
}