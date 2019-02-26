using System;
using System.IO;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using LunarParser;
using LunarParser.JSON;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using ProtoBuf;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1.Sec;
using Google.Protobuf;

// using System.Security.Cryptography;
using System.ComponentModel;
using System.Linq;

using bluzelle_cs;

namespace bluzelle_cs
{
    public class Utils
    {
        public Utils()
        {
            // constructor
        }

        public byte[] SignData(byte[] data, ECPrivateKeyParameters privKey)
        {
            try
            {
                ISigner signer = SignerUtilities.GetSigner("SHA-512withECDSA");

                signer.Init(true, privKey);
                signer.BlockUpdate(data, 0, data.Length);
                
                var signature = signer.GenerateSignature();
                return signature;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Signing Failed: " + exc.ToString());
                return null;
            }
        }

    }
}