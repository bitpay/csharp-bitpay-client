using System;

namespace BitPay.Utils
{
    public class UuidGenerator : IGuidGenerator
    {
        public string Execute()
        {
            Guid guid = Guid.NewGuid();
            
            return guid.ToString();
        }
    }
}