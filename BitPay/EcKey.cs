/*
 * Copyright 2011 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace BitPaySDK
{
    /// <summary>
    ///     Represents an elliptic curve keypair that we own and can use for signing transactions. Currently,
    ///     Bouncy Castle is used. In future this may become an interface with multiple implementations using different crypto
    ///     libraries. The class also provides a static method that can verify a signature with just the public key.
    /// </summary>
    [Serializable]
    public class EcKey
    {
        private static readonly ECDomainParameters EcParams;

        private static readonly SecureRandom SecureRandom;

        private readonly BigInteger _privateKey;

        [NonSerialized] private byte[] _pubKeyHash;

        private string _publicKeyHexBytes;

        static EcKey()
        {
            // All clients must agree on the curve to use by agreement. BitCoin uses secp256k1.
            var ecParameters = SecNamedCurves.GetByName("secp256k1");
            EcParams = new ECDomainParameters(ecParameters.Curve, ecParameters.G, ecParameters.N, ecParameters.H);
            SecureRandom = new SecureRandom();
        }

        /// <summary>
        ///     Generates an entirely new keypair.
        /// </summary>
        public EcKey()
        {
            var generator = new ECKeyPairGenerator();
            var keygenParams = new ECKeyGenerationParameters(EcParams, SecureRandom);
            generator.Init(keygenParams);
            var keypair = generator.GenerateKeyPair();
            var privateKeyParams = (ECPrivateKeyParameters) keypair.Private;
            var pubParams = (ECPublicKeyParameters) keypair.Public;
            _privateKey = privateKeyParams.D;
            // The public key is an encoded point on the elliptic curve. It has no meaning independent of the curve.
            PublicKey = pubParams.Q.GetEncoded();
        }

        /// <summary>
        ///     Creates an ECKey given only the private key. This works because EC public keys are derivable from their
        ///     private keys by doing a multiply with the generator value.
        /// </summary>
        public EcKey(BigInteger privateKey)
        {
            _privateKey = privateKey;
            PublicKey = PublicKeyFromPrivate(privateKey);
        }

        /// <summary>
        ///     Gets the hash160 form of the public key (as seen in addresses).
        /// </summary>
        public byte[] PubKeyHash => _pubKeyHash ?? (_pubKeyHash = Sha256Hash160(PublicKey));

        /// <summary>
        ///     Gets the raw public key value. This appears in transaction scriptSigs. Note that this is <b>not</b> the same
        ///     as the pubKeyHash/address.
        /// </summary>
        public byte[] PublicKey { get; }

        public string PublicKeyHexBytes
        {
            get
            {
                if (_publicKeyHexBytes == null) _publicKeyHexBytes = KeyUtils.BytesToHex(PublicKey);

                return _publicKeyHexBytes;
            }
        }

        /// <summary>
        ///     Construct an ECKey from an ASN.1 encoded private key. These are produced by OpenSSL and stored by the BitCoin
        ///     reference implementation in its wallet.
        /// </summary>
        public static EcKey FromAsn1(byte[] asn1PrivKey)
        {
            return new EcKey(ExtractPrivateKeyFromAsn1(asn1PrivKey));
        }

        /// <summary>
        ///     Output this ECKey as an ASN.1 encoded private key, as understood by OpenSSL or used by the BitCoin reference
        ///     implementation in its wallet storage format.
        /// </summary>
        public byte[] ToAsn1()
        {
            using (var baos = new MemoryStream(400))
            {
                using (var encoder = new Asn1OutputStream(baos))
                {
                    // ASN1_SEQUENCE(EC_PRIVATEKEY) = {
                    //   ASN1_SIMPLE(EC_PRIVATEKEY, version, LONG),
                    //   ASN1_SIMPLE(EC_PRIVATEKEY, privateKey, ASN1_OCTET_STRING),
                    //   ASN1_EXP_OPT(EC_PRIVATEKEY, parameters, ECPKPARAMETERS, 0),
                    //   ASN1_EXP_OPT(EC_PRIVATEKEY, publicKey, ASN1_BIT_STRING, 1)
                    // } ASN1_SEQUENCE_END(EC_PRIVATEKEY)
                    var seq = new DerSequenceGenerator(encoder);
                    seq.AddObject(new DerInteger(1)); // version
                    seq.AddObject(new DerOctetString(_privateKey.ToByteArray()));
                    seq.AddObject(new DerTaggedObject(0, SecNamedCurves.GetByName("secp256k1").ToAsn1Object()));
                    seq.AddObject(new DerTaggedObject(1, new DerBitString(PublicKey)));
                    seq.Close();
                }

                return baos.ToArray();
            }
        }

        /// <summary>
        ///     Derive the public key by doing a point multiply of G * priv.
        /// </summary>
        private static byte[] PublicKeyFromPrivate(BigInteger privKey)
        {
            return EcParams.G.Multiply(privKey).GetEncoded();
        }

        /// <summary>
        ///     Gets the hash160 form of the input array
        /// </summary>
        /// <param name="input">The array to hash</param>
        /// <returns>The hash160 hash</returns>
        public static byte[] Sha256Hash160(byte[] input)
        {
            var sha256 = new SHA256Managed().ComputeHash(input);
            var digest = new RipeMD160Digest();
            digest.BlockUpdate(sha256, 0, sha256.Length);
            var @out = new byte[20];
            digest.DoFinal(@out, 0);
            return @out;
        }

        public static string BytesToHexString(byte[] bytes)
        {
            var buf = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                var s = b.ToString("x");
                if (s.Length < 2)
                    buf.Append('0');
                buf.Append(s);
            }

            return buf.ToString();
        }

        public override string ToString()
        {
            var b = new StringBuilder();
            b.Append("pub:").Append(BytesToHexString(PublicKey));
            // maybe we don't want to show the private key wherever we call this method from...
            //b.Append(" priv:").Append(BytesToHexString(_privateKey.ToByteArray()));
            return b.ToString();
        }

        protected virtual BigInteger CalculateE(BigInteger n, byte[] message)
        {
            var messageBitLength = message.Length * 8;
            var trunc = new BigInteger(1, message);

            if (n.BitLength < messageBitLength) trunc = trunc.ShiftRight(messageBitLength - n.BitLength);

            return trunc;
        }

        /// <summary>
        ///     Calculates an ECDSA signature in DER format for the given input hash. Note that the input is expected to be
        ///     32 bytes long.
        /// </summary>
        public byte[] Sign(byte[] input)
        {
            var ecDsaSigner = new ECDsaSigner();
            var privateKeyParameters = new ECPrivateKeyParameters(_privateKey, EcParams);
            ecDsaSigner.Init(true, privateKeyParameters);
            var signature = ecDsaSigner.GenerateSignature(input);
            using (var memoryStream = new MemoryStream())
            {
                var sequenceGenerator = new DerSequenceGenerator(memoryStream);
                sequenceGenerator.AddObject(new DerInteger(signature[0]));
                sequenceGenerator.AddObject(new DerInteger(signature[1]));
                sequenceGenerator.Close();
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        ///     Verifies the given ASN.1 encoded ECDSA signature against a hash using the public key.
        /// </summary>
        /// <param name="data">Hash of the data to verify.</param>
        /// <param name="signature">ASN.1 encoded signature.</param>
        /// <param name="pub">The public key bytes to use.</param>
        public static bool Verify(byte[] data, byte[] signature, byte[] pub)
        {
            var signer = new ECDsaSigner();
            var @params = new ECPublicKeyParameters(EcParams.Curve.DecodePoint(pub), EcParams);
            signer.Init(false, @params);
            DerInteger r;
            DerInteger s;
            using (var decoder = new Asn1InputStream(signature))
            {
                var seq = (DerSequence) decoder.ReadObject();
                r = (DerInteger) seq[0];
                s = (DerInteger) seq[1];
            }

            return signer.VerifySignature(data, r.Value, s.Value);
        }

        /// <summary>
        ///     Verifies the given ASN.1 encoded ECDSA signature against a hash using the public key.
        /// </summary>
        /// <param name="data">Hash of the data to verify.</param>
        /// <param name="signature">ASN.1 encoded signature.</param>
        public bool Verify(byte[] data, byte[] signature)
        {
            return Verify(data, signature, PublicKey);
        }

        private static BigInteger ExtractPrivateKeyFromAsn1(byte[] asn1PrivKey)
        {
            // To understand this code, see the definition of the ASN.1 format for EC private keys in the OpenSSL source
            // code in ec_asn1.c:
            //
            // ASN1_SEQUENCE(EC_PRIVATEKEY) = {
            //   ASN1_SIMPLE(EC_PRIVATEKEY, version, LONG),
            //   ASN1_SIMPLE(EC_PRIVATEKEY, privateKey, ASN1_OCTET_STRING),
            //   ASN1_EXP_OPT(EC_PRIVATEKEY, parameters, ECPKPARAMETERS, 0),
            //   ASN1_EXP_OPT(EC_PRIVATEKEY, publicKey, ASN1_BIT_STRING, 1)
            // } ASN1_SEQUENCE_END(EC_PRIVATEKEY)
            //
            DerOctetString key;
            using (var decoder = new Asn1InputStream(asn1PrivKey))
            {
                var seq = (DerSequence) decoder.ReadObject();
                Debug.Assert(seq.Count == 4, "Input does not appear to be an ASN.1 OpenSSL EC private key");
                Debug.Assert(((DerInteger) seq[0]).Value.Equals(BigInteger.One), "Input is of wrong version");
                key = (DerOctetString) seq[1];
            }

            return new BigInteger(1, key.GetOctets());
        }
    }
}