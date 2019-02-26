using System;
using System.Collections.Generic;
using Org.BouncyCastle.Math;

namespace bluzelle_cs
{
    public class MetadataLayer
    {
        public delegate void OnOutgoingMsg(object x);
        private OnOutgoingMsg onOutgoingMsg;
        private string uuid;

        private SortedDictionary<object,bool> nonceMap;

        public MetadataLayer(string uuid)
        {
            this.uuid = uuid;

            this.nonceMap = new SortedDictionary<object, bool>();
        }

        private ulong GenerateNonce()
        {
            var rand = new Random();
            var min = 1;
            var max = Int32.MaxValue;

            Int64 result = rand.Next((Int32)(min >> 32), (Int32)(max >> 32));
            result = (result << 32);
            result = result | (Int64)rand.Next((Int32)min, (Int32)max);
            return (ulong)result;
        }

        public bzn_msg SetNonceHeader(bzn_msg msg)
        {
            var uuidInsert = this.uuid; 
            // var nonceInsert = GenerateNonce();
            var nonce = "127";
            decimal d  = Decimal.Parse(nonce);
            ulong nonceFormatted = (ulong)(d);
            var nonceInsert = nonceFormatted;
            msg.Db.Header = new database_header() 
            { 
                DbUuid = uuidInsert, 
                Nonce = nonceInsert 
            };
            Console.WriteLine(nonceInsert);
            this.nonceMap.Add(nonceInsert, true);

            return msg;
        }

        public void RemoveNonceHeader(database_response res) 
        {
            if(res.Header.DbUuid == this.uuid)
            {
                var nonce = res.Header.Nonce;

                try
                {
                    if(this.nonceMap[nonce])
                    {
                        this.nonceMap.Remove(nonce);
                    }
                    else
                    {
                        Console.WriteLine("Metadata layer: nonce doesn't belong to map.  Was it terminated too early?");
                    }
                }
                catch(KeyNotFoundException)
                {
                    Console.WriteLine("Metadata layer: nonce doesn't belong to map.  Was it terminated too early?");
                }
            }
        }

    }
}