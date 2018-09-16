using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Diagnostics;
using System.IO;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;

namespace BitPayAPI
{
    public class EcKey {

        private static readonly ECDomainParameters _ecParams;
        private static readonly SecureRandom _secureRandom;

        public Byte[] privKey { get; private set; }
		public Byte[] pubKey { get; private set; }
		public Boolean isCompressed { get; private set; }

        static EcKey() {
            // All clients must agree on the curve to use by agreement. BitCoin uses secp256k1.
            var @params = SecNamedCurves.GetByName("secp256k1");
            _ecParams = new ECDomainParameters(@params.Curve, @params.G, @params.N, @params.H);
            _secureRandom = new SecureRandom();
        }

        public EcKey() {
            var generator = new ECKeyPairGenerator();
            var keygenParams = new ECKeyGenerationParameters(_ecParams, _secureRandom);
            generator.Init(keygenParams);
            var keypair = generator.GenerateKeyPair();
            var privParams = (ECPrivateKeyParameters)keypair.Private;
            var pubParams = (ECPublicKeyParameters)keypair.Public;
            privKey = privParams.D.ToByteArray();
            // The public key is an encoded point on the elliptic curve. It has no meaning independent of the curve.
            pubKey = pubParams.Q.GetEncoded();
        }

		public EcKey(Byte[] privKey, Byte[] pubKey = null, Boolean compressed = false)
		{
			this.privKey = privKey;
			if (pubKey != null)
			{
				this.pubKey = pubKey;
				this.isCompressed = pubKey.Length <= 33;
			}
			else
			{
				calcPubKey(compressed);
			}
		}

		public void compress(bool comp)
		{
			if (isCompressed == comp) return;
			ECPoint point = _ecParams.Curve.DecodePoint(pubKey);
			if (comp)
				pubKey = compressPoint(point).GetEncoded();
			else
				pubKey = decompressPoint(point).GetEncoded();
			isCompressed = comp;
		}

		public Boolean verifySignature(Byte[] data, Byte[] sig)
		{
			ECDsaSigner signer = new ECDsaSigner();
			signer.Init(false, new ECPublicKeyParameters(_ecParams.Curve.DecodePoint(pubKey), _ecParams));
			using (Asn1InputStream asn1stream = new Asn1InputStream(sig))
			{
				Asn1Sequence seq = (Asn1Sequence)asn1stream.ReadObject();
				return signer.VerifySignature(data, ((DerInteger)seq[0]).PositiveValue, ((DerInteger)seq[1]).PositiveValue);
			}
		}

		public Byte[] signData(Byte[] data)
		{
			if (privKey == null)
				throw new InvalidOperationException();
			ECDsaSigner signer = new ECDsaSigner();
			signer.Init(true, new ECPrivateKeyParameters(new BigInteger(1, privKey), _ecParams));
			BigInteger[] sig = signer.GenerateSignature(data);
			using (MemoryStream ms = new MemoryStream())
			using (Asn1OutputStream asn1stream = new Asn1OutputStream(ms))
			{
				DerSequenceGenerator seq = new DerSequenceGenerator(asn1stream);
				seq.AddObject(new DerInteger(sig[0]));
				seq.AddObject(new DerInteger(sig[1]));
				seq.Close();
                return ms.ToArray();
            }
		}

		private void calcPubKey(bool comp) {

			ECPoint point = _ecParams.G.Multiply(new BigInteger(1, privKey));
			this.pubKey = point.GetEncoded();
			compress(comp);
		}

		private ECPoint compressPoint(ECPoint point)
		{
			return new FpPoint(_ecParams.Curve, point.X, point.Y, true);
		}

		private ECPoint decompressPoint(ECPoint point)
		{
			return new FpPoint(_ecParams.Curve, point.X, point.Y, false);
		}

        /// <summary>
        /// Construct an ECKey from an ASN.1 encoded private key. These are produced by OpenSSL and stored by the BitCoin
        /// reference implementation in its wallet.
        /// </summary>
        public static EcKey FromAsn1(byte[] asn1PrivKey)
        {
            return new EcKey(ExtractPrivateKeyFromAsn1(asn1PrivKey).ToByteArray());
        }

        /// <summary>
        /// Output this ECKey as an ASN.1 encoded private key, as understood by OpenSSL or used by the BitCoin reference
        /// implementation in its wallet storage format.
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
                    seq.AddObject(new DerOctetString(privKey));
                    seq.AddObject(new DerTaggedObject(0, SecNamedCurves.GetByName("secp256k1").ToAsn1Object()));
                    seq.AddObject(new DerTaggedObject(1, new DerBitString(pubKey)));
                    seq.Close();
                }
                return baos.ToArray();
            }
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
                var seq = (DerSequence)decoder.ReadObject();
                Debug.Assert(seq.Count == 4, "Input does not appear to be an ASN.1 OpenSSL EC private key");
                Debug.Assert(((DerInteger)seq[0]).Value.Equals(BigInteger.One), "Input is of wrong version");
                key = (DerOctetString)seq[1];
            }
            return new BigInteger(1, key.GetOctets());
        }

        /// <summary>
        /// Calculates an ECDSA signature in DER format for the given input hash. Note that the input is expected to be
        /// 32 bytes long.
        /// </summary>
        public byte[] Sign(byte[] input)
        {
            var signer = new ECDsaSigner();
            var privKey = new ECPrivateKeyParameters(new BigInteger(this.privKey), _ecParams);
            signer.Init(true, privKey);
            var sigs = signer.GenerateSignature(input);
            // What we get back from the signer are the two components of a signature, r and s. To get a flat byte stream
            // of the type used by BitCoin we have to encode them using DER encoding, which is just a way to pack the two
            // components into a structure.
            using (var bos = new MemoryStream())
            {
                var seq = new DerSequenceGenerator(bos);
                seq.AddObject(new DerInteger(sigs[0]));
                seq.AddObject(new DerInteger(sigs[1]));
                seq.Close();
                return bos.ToArray();
            }
        }
    }
}
