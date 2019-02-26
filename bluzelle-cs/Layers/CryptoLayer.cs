using System;
using System.Text;
using System.ComponentModel;
using System.Linq;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1;

using Google.Protobuf;
using bluzelle_cs;



namespace bluzelle_cs
{
    public class CryptoLayer
    {
        private string private_pem;
        private static readonly Org.BouncyCastle.Asn1.X9.X9ECParameters curve = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1");
        private static readonly Org.BouncyCastle.Crypto.Parameters.ECDomainParameters domain = new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
        private Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters privateKeyParameters;
        private Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters pubKeyParameters;
        public CryptoLayer(string privatePem)
        {
            this.private_pem = privatePem;
        }

        private string DeterministicSerialize(String[] obj)
        {

            string finalBin = "";

            for(int i = 0; i < obj.Length; i++)
            {
                finalBin += (obj[i].ToString().Length + "|" + obj[i].ToString());                   
            }

            return finalBin;
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        
        public string GetPublicKey(byte[] privKey)
        {
            var privHex = BitConverter.ToString(privKey).Replace("-","");
            privHex = privHex.Substring(104, (privHex.Length - 104));
            privKey = StringToByteArray(privHex);

            Org.BouncyCastle.Math.BigInteger d = new Org.BouncyCastle.Math.BigInteger(privKey);
            this.privateKeyParameters = new Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters(d, domain);
            Org.BouncyCastle.Math.EC.ECPoint q = domain.G.Multiply(d);
            this.pubKeyParameters = new Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters(q, domain);

            //strip first byte
            var pubBytes = this.pubKeyParameters.Q.GetEncoded();
            var pubHex = BitConverter.ToString(pubBytes).Replace("-","");
            var strippedPubBytes = StringToByteArray(pubHex.Substring(2,(pubHex.Length - 2)));

            return "MFYwEAYHKoZIzj0CAQYFK4EEAAoDQgAE" + Convert.ToBase64String(strippedPubBytes);
        }

        public bool VerifySignature(ECPublicKeyParameters pubKey, byte[] signature, string msg)
        {
            try
            {
                // Console.WriteLine(msg);
                byte[] msgBytes = Encoding.ASCII.GetBytes(msg);
                byte[] sigBytes = signature;

                var pubBytes = this.pubKeyParameters.Q.GetEncoded();
                var pubHex = BitConverter.ToString(pubBytes).Replace("-","");
                // Console.WriteLine(msg);
                // Console.WriteLine(pubHex);

                ISigner signer = SignerUtilities.GetSigner("SHA-512withECDSA");
                signer.Init(false, pubKey);
                signer.BlockUpdate(msgBytes, 0, msgBytes.Length);
                return signer.VerifySignature(sigBytes);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Verification failed with the error: " + exc.ToString());
                return false;
            }
        }
        public bzn_envelope SignEnvelope(bzn_envelope env)
        {
            DateTime upper = DateTime.UtcNow;
            DateTime lower = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            ulong timestamp = (ulong)(upper - lower).TotalMilliseconds;

            // determine public key from provided key
            var sender = GetPublicKey(Convert.FromBase64String(this.private_pem));
            var payloadCase = 0;

            switch(env.PayloadCase.ToString())
            {
                case "DatabaseMsg":
                {
                    payloadCase = 10;
                    break;
                }
                case "PbftInternalRequest":
                {
                    payloadCase = 11;
                    break;
                }
                case "DatabaseResponse":
                {
                    payloadCase = 12;
                    break;
                }
                case "Json":
                {
                    payloadCase = 13;
                    break;
                }
                case "Audit":
                {
                    payloadCase = 14;
                    break;
                }
                case "Pbft":
                {
                    payloadCase = 15;
                    break;
                }
                case "PbftMembership":
                {
                    payloadCase = 16;
                    break;
                }
                case "StatusRequest":
                {
                    payloadCase = 17;
                    break;
                }
                case "StatusResponse":
                {
                    payloadCase = 18;
                    break;
                }
            }

            String[] binForTheWin = new String[]
            {
                sender,
                payloadCase.ToString(),
                env.DatabaseMsg.ToStringUtf8(),
                timestamp.ToString()
            };

            // data for serializing
            env.Sender =  sender;
            var signer = new Utils();
            var hexHash = BitConverter.ToString(Encoding.ASCII.GetBytes(DeterministicSerialize(binForTheWin))).Replace("-","");
            env.Signature  = ByteString.CopyFrom(signer.SignData(StringToByteArray(hexHash), this.privateKeyParameters));
            env.Timestamp = (ulong)timestamp;

            // Console.WriteLine(DeterministicSerialize(binForTheWin));

            if(VerifySignature(this.pubKeyParameters,
            env.Signature.ToByteArray(),
            DeterministicSerialize(binForTheWin)))
            {
                // var sigHex = BitConverter.ToString(env.ToByteArray()).Replace("-","");
                // Console.WriteLine(sigHex);
                return env;
            }
            else
            {
                Console.WriteLine("Error in signature.");
                return null;
            }
            
        }
    }
}