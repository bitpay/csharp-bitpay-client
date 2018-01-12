using BitCoinSharp;
using Org.BouncyCastle.Math;
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace BitPayAPI
{
    public class KeyUtils
    {
        private static char[] hexArray = "0123456789abcdef".ToCharArray();
        private static String PRIV_KEY_FILENAME = "bitpay_private.key";

	    public KeyUtils() {}

        public static bool privateKeyExists()
        {
            return File.Exists(PRIV_KEY_FILENAME);
        }

        public static EcKey createEcKey()
        {
            //Default constructor uses SecureRandom numbers.
            return new EcKey();
        }

        public static EcKey createEcKeyFromHexString(String privateKey)
        {
            BigInteger pkey = new BigInteger(privateKey, 16);
            EcKey key = new EcKey(pkey);
            return key;
        }

        // Convenience method.
        public static EcKey createEcKeyFromHexStringFile(String privKeyFile)
        {
            String privateKey = getKeyStringFromFile(privKeyFile);
            return createEcKeyFromHexString(privateKey);
        }

        public static EcKey loadEcKey()
        {
            using (FileStream fs = File.OpenRead(PRIV_KEY_FILENAME))
            {
                byte[] b = new byte[1024];
                fs.Read(b, 0, b.Length);
                EcKey key = EcKey.FromAsn1(b);
                return key;
            }
	    }

        public static String getKeyStringFromFile(String filename)
        {
            StreamReader sr;
            try
            {
                sr = new StreamReader(filename);
                String line = sr.ReadToEnd();
                sr.Close();
                return line;
            }
            catch (IOException e)
            {
                Console.Write(e.Message);
            }
            return "";
        }

        public static void saveEcKey(EcKey ecKey)
        {
		    byte[] bytes = ecKey.ToAsn1();
            FileStream fs = new FileStream(PRIV_KEY_FILENAME, FileMode.Create, FileAccess.Write);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }

        public static String deriveSIN(EcKey ecKey)
        {
            // Get sha256 hash and then the RIPEMD-160 hash of the public key (this call gets the result in one step).
            byte[] pubKeyHash = ecKey.PubKeyHash; 

            // Convert binary pubKeyHash, SINtype and version to Hex
            String version = "0F";
            String SINtype = "02";
            String pubKeyHashHex = bytesToHex(pubKeyHash);

            // Concatenate all three elements
            String preSIN = version + SINtype + pubKeyHashHex;

            // Convert the hex string back to binary and double sha256 hash it leaving in binary both times
            byte[] preSINbyte = hexToBytes(preSIN);
            byte[] hash2Bytes = Utils.DoubleDigest(preSINbyte);

            // Convert back to hex and take first four bytes
            String hashString = bytesToHex(hash2Bytes);
            String first4Bytes = hashString.Substring(0, 8);

            // Append first four bytes to fully appended SIN string
            String unencoded = preSIN + first4Bytes;
            byte[] unencodedBytes = new BigInteger(unencoded, 16).ToByteArray();
            String encoded = Base58.Encode(unencodedBytes);

            return encoded;
        }

 	    public static String sign(EcKey ecKey, String input) 
        {
            String hash = sha256Hash(input);
            return bytesToHex(ecKey.Sign(hexToBytes(hash)));
	    }

        private static String sha256Hash(String value)
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
        private static bool isValidHexDigit(char chr)
        {
            return ('0' <= chr && chr <= '9') || ('a' <= chr && chr <= 'f') || ('A' <= chr && chr <= 'F');
        }

        public static byte[] hexToBytes(string hex)
        {
            if (hex == null)
                throw new ArgumentNullException("hex");
            if (hex.Length % 2 == 1)
                throw new FormatException("The binary key cannot have an odd number of digits");

            if (hex == string.Empty)
                return new byte[0];

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                char highNibble = hex[i << 1];
                char lowNibble = hex[(i << 1) + 1];

                if (!isValidHexDigit(highNibble) || !isValidHexDigit(lowNibble))
                    throw new FormatException("The binary key contains invalid chars.");

                arr[i] = (byte)((getHexVal(highNibble) << 4) + (getHexVal(lowNibble)));
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

        static byte[] getBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
