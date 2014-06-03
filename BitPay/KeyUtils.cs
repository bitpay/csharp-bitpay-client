using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using System.IO;
using System.Numerics;

using System.Security.Cryptography;

namespace BitPayAPI
{
    public class KeyUtils
    {

        private static char[] hexArray = "0123456789abcdef".ToCharArray();       

	    public KeyUtils() 
        {

	    }

	    public static ECKey loadKeys(String privateKey)
        {
            ECKey key = new ECKey(hexToBytes(privateKey), null, true);
		    return key;
	    }

	    public static String readKeyFromFile(String filename)
        {
		    StreamReader sr;
	        try {
	    	    sr = new StreamReader(filename);
                String line = sr.ReadToEnd();
	            sr.Close();
	            return line;
	        } catch (IOException e) {
                Console.Write(e.Message);
		    }
	        return "";
	    }

	    public static String signString(ECKey key, String input) 
        {
            String hash = sha256(input);
            return bytesToHex(key.signData(hexToBytes(hash)));
	    }

        private static String sha256(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }

        private static int getHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public static byte[] hexToBytes(string hex)
        {
            if (hex.Length % 2 == 1)
            {
                throw new Exception("The binary key cannot have an odd number of digits");
            }
            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((getHexVal(hex[i << 1]) << 4) + (getHexVal(hex[(i << 1) + 1])));
            }
            return arr;
        }

        public static String bytesToHex(byte[] bytes)
        {
	        char[] hexChars = new char[bytes.Length * 2];
	        for ( int j = 0; j < bytes.Length; j++ ) {
	            int v = bytes[j] & 0xFF;
                hexChars[j * 2] = hexArray[(int)((uint)v >> 4)];
	            hexChars[j * 2 + 1] = hexArray[v & 0x0F];
	        }
	        return new String(hexChars);
	    }
    }
}
