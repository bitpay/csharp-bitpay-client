using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using System.IO;
using System.Numerics;

namespace BitPayAPI
{
    public class KeyUtils
    {
        private static char[] hexArray = "0123456789abcdef".ToCharArray();

	    public KeyUtils() {

	    }

	    public static ECKey loadKeys(String privateKey, String publicKey) {
//J             BigInteger privKey = new BigInteger(privateKey, 16);
//C#            BigInteger privKey = BigInteger.Parse(privateKey, NumberStyles.HexNumber); 

//J            ECKey key = new ECKey(privKey, null, true);
            ECKey key = new ECKey(getBytes(privateKey), null, true);

		    return key;
	    }

	    public static String readKeyFromFile(String filename) {
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

	    public static String signString(ECKey key, String input) {
		    Console.WriteLine("Signing string: " + input);
		    byte[] data = getBytes(input);
            //byte[] data = Utils.formatMessageForSigning(input);
//            Sha256Hash hash = Sha256Hash.create(data);
//            ECDSASignature sig = key.sign(hash, null);
//            byte[] bytes = sig.encodeToDER();

            return bytesToHex(key.signData(data));
	    }

	    public static String bytesToHex(byte[] bytes) {
	        char[] hexChars = new char[bytes.Length * 2];
	        for ( int j = 0; j < bytes.Length; j++ ) {
	            int v = bytes[j] & 0xFF;
                hexChars[j * 2] = hexArray[(int)((uint)v >> 4)];
	            hexChars[j * 2 + 1] = hexArray[v & 0x0F];
	        }
	        return new String(hexChars);
	    }

        private static byte[] getBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string getString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

    }
}
