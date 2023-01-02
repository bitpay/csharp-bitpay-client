using System;

namespace BitPay.Utils
{
    public class GuidGenerator
    {
        public string Execute() {
            Guid guid = Guid.NewGuid();

            return guid.ToString();
        }
    }
}