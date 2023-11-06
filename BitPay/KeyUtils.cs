// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BitPay.Exceptions;

using Org.BouncyCastle.Crypto.Digests;
using BigInteger = Org.BouncyCastle.Math.BigInteger;

namespace BitPay
{
    public static class KeyUtils
    {
        private static string? PrivateKeyFile;
        private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        private static readonly char[] HexArray = "0123456789abcdef".ToCharArray();

        private static string? _derivedSin;
        private static readonly BigInteger Base = BigInteger.ValueOf(58);

        public static bool PrivateKeyExists(string? privateKeyFile)
        {
            if (privateKeyFile == null)
            {
                return false;
            }
            
            PrivateKeyFile = privateKeyFile;
            
            return File.Exists(privateKeyFile);
        }

        public static EcKey CreateEcKey()
        {
            //Default constructor uses SecureRandom numbers.
            return new EcKey();
        }
        
        public static EcKey CreateEcKeyFromString(string? privateKey)
        {
            if (privateKey == null)
            { 
                throw new ArgumentNullException(nameof(privateKey));
            }
            
            var pkey = new BigInteger(privateKey);
            var key = new EcKey(pkey);
            return key;
        }
        // Convenience method.
        public static EcKey CreateEcKeyFromHexStringFile(string privKeyFile)
        {
            var privateKey = GetKeyStringFromFile(privKeyFile);
            return CreateEcKeyFromHexString(privateKey);
        }

        public static EcKey LoadEcKey()
        {
            // using (var fs = File.OpenRead(PrivateKeyFile))
            // {
            //     var b = new byte[1024];
            //     await fs.ReadAsync(b, 0, b.Length);
            //     var key = EcKey.FromAsn1(b);
            //     return key;
            // }
                
            if (PrivateKeyFile == null)
            { 
                throw new ArgumentNullException(nameof(PrivateKeyFile));
            }
            
            byte[] file = File.ReadAllBytes(PrivateKeyFile);
            var key = EcKey.FromAsn1(file);
            return key;
        }

        public static async Task SaveEcKey(EcKey ecKey)
        {
            if (ecKey == null)
            {
                throw new ArgumentNullException(nameof(ecKey));
            }

            var bytes = ecKey.ToAsn1();
            if (!string.IsNullOrEmpty(Path.GetDirectoryName(PrivateKeyFile)) && !Directory.Exists(Path.GetDirectoryName(PrivateKeyFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PrivateKeyFile)!);
            }
            using (var fs = new FileStream(PrivateKeyFile!, FileMode.Create, FileAccess.Write))
            {
#pragma warning disable CA1835
                await fs.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
#pragma warning restore CA1835

            }
        }

        public static string DeriveSin(EcKey ecKey)
        {
            if (ecKey == null)
            {
                throw new ArgumentNullException(nameof(ecKey));
            }

            if (_derivedSin != null) return _derivedSin;
            // Get sha256 hash and then the RIPEMD-160 hash of the public key (this call gets the result in one step).
            var pubKey = ecKey.GetPublicKey();
            var output = new byte[20];
            using (var sha256Managed = SHA256.Create())
            {
                var hash = sha256Managed.ComputeHash(pubKey);
                var ripeMd160Digest = new RipeMD160Digest();
                ripeMd160Digest.BlockUpdate(hash, 0, hash.Length);
                ripeMd160Digest.DoFinal(output, 0);
            }

            var pubKeyHash = output;

            // Convert binary pubKeyHash, SINtype and version to Hex
            var version = "0F";
            var siNtype = "02";
            var pubKeyHashHex = BytesToHex(pubKeyHash);

            // Concatenate all three elements
            var preSin = version + siNtype + pubKeyHashHex;

            // Convert the hex string back to binary and double sha256 hash it leaving in binary both times
            var preSiNbyte = HexToBytes(preSin);
            var hash2Bytes = DoubleDigest(preSiNbyte);

            // Convert back to hex and take first four bytes
            var hashString = BytesToHex(hash2Bytes);
            var first4Bytes = hashString.Substring(0, 8);

            // Append first four bytes to fully appended SIN string
            var unencoded = preSin + first4Bytes;
            var unencodedBytes = new BigInteger(unencoded, 16).ToByteArray();
            var encoded = Encode(unencodedBytes);

            _derivedSin = encoded;

            return encoded;
        }

        private static string Encode(byte[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            // TODO: This could be a lot more efficient.
            var bi = new BigInteger(1, input);
            var s = new StringBuilder();
            while (bi.CompareTo(Base) >= 0)
            {
                var mod = bi.Mod(Base);
                s.Insert(0, new[] {Alphabet[mod.IntValue]});
                bi = bi.Subtract(mod).Divide(Base);
            }

            s.Insert(0, new[] {Alphabet[bi.IntValue]});
            // Convert leading zeros too.
            foreach (var anInput in input)
                if (anInput == 0)
                    s.Insert(0, new[] {Alphabet[0]});
                else
                    break;

            return s.ToString();
        }

        /// <summary>
        ///     See <see cref="DoubleDigest(byte[], int, int)" />.
        /// </summary>
        private static byte[] DoubleDigest(byte[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return DoubleDigest(input, 0, input.Length);
        }

        /// <summary>
        ///     Calculates the SHA-256 hash of the given byte range, and then hashes the resulting hash again. This is
        ///     standard procedure in BitCoin. The resulting hash is in big endian form.
        /// </summary>
        private static byte[] DoubleDigest(byte[] input, int offset, int length)
        {
            using (var algorithm = SHA256.Create())
            {
                var first = algorithm.ComputeHash(input, offset, length);
                return algorithm.ComputeHash(first);
            }
        }

        /// <summary>
        ///     Signs the input string with the provided key
        /// </summary>
        /// <param name="ecKey">The key object to sign with</param>
        /// <param name="input">The string to be signed</param>
        /// <returns>The signature</returns>
        /// <throws>BitPayGenericException BitPayGenericException class</throws>
        public static string Sign(EcKey? ecKey, string input)
        {
            if (ecKey == null)
            {
                BitPayExceptionProvider.ThrowMissingParameterException();
                throw new InvalidOperationException();
            }

            string result = null!;

            try
            {
                var hash = Sha256Hash(input);
                var hashBytes = HexToBytes(hash);
                var signature = ecKey.Sign(hashBytes);
                result = BytesToHex(signature);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowGenericExceptionWithMessage(e.Message);
            }

            return result;
        }
        
        /// <summary>
        ///     Bytes to Hex
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>string</returns>
        /// <throws>BitPayGenericException BitPayGenericException class</throws>
        public static string BytesToHex(byte[]? bytes)
        {
            if (bytes == null)
            {
                BitPayExceptionProvider.ThrowGenericExceptionWithMessage("Missing bytes");
                throw new InvalidOperationException();
            }

            var hexChars = new char[bytes.Length * 2];
            for (var j = 0; j < bytes.Length; j++)
            {
                var v = bytes[j] & 0xFF;
                hexChars[j * 2] = HexArray[(int) ((uint) v >> 4)];
                hexChars[j * 2 + 1] = HexArray[v & 0x0F];
            }

            return new string(hexChars);
        }
        
        private static EcKey CreateEcKeyFromHexString(string privateKey)
        {
            var pkey = new BigInteger(privateKey, 16);
            var key = new EcKey(pkey);
            return key;
        }

        private static string Sha256Hash(string value)
        {
            var sb = new StringBuilder();
            using (var hash = SHA256.Create())
            {
                var enc = Encoding.UTF8;
                var result = hash.ComputeHash(enc.GetBytes(value));

                foreach (var b in result)
#pragma warning disable CA1305
                    sb.Append(b.ToString("x2"));
#pragma warning restore CA1305
            }

            return sb.ToString();
        }

        private static int GetHexVal(char hex)
        {
            var val = (int) hex;
            return val - (val < 58 ? 48 : val < 97 ? 55 : 87);
        }

        private static bool IsValidHexDigit(char chr)
        {
            return '0' <= chr && chr <= '9' || 'a' <= chr && chr <= 'f' || 'A' <= chr && chr <= 'F';
        }

        private static byte[] HexToBytes(string hex)
        {
            if (hex == null)
                throw new ArgumentNullException(nameof(hex));
            if (hex.Length % 2 == 1)
                throw new FormatException("The binary key cannot have an odd number of digits");

            if (hex.Length == 0)
                return new byte[0];

            var arr = new byte[hex.Length >> 1];

            for (var i = 0; i < hex.Length >> 1; ++i)
            {
                var highNibble = hex[i << 1];
                var lowNibble = hex[(i << 1) + 1];

                if (!IsValidHexDigit(highNibble) || !IsValidHexDigit(lowNibble))
                    throw new FormatException("The binary key contains invalid chars.");

                arr[i] = (byte) ((GetHexVal(highNibble) << 4) + GetHexVal(lowNibble));
            }

            return arr;
        }
        
        private static string GetKeyStringFromFile(string filename)
        {
            using (var sr = new StreamReader(filename))
            {
                var line = sr.ReadToEnd();
                sr.Close();
                return line;
            }
        }
    }
}