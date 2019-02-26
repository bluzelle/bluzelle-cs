using LunarParser;
using LunarParser.JSON;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSocketSharp;
using Google.Protobuf;
using System.IO;
using System.Text;

namespace bluzelle_cs
{
    public class Bluzelle:ISwarm
    {
        #region Constructors
            private readonly ConnectionLayer _ConnectionLayer;
            private readonly CryptoLayer _CryptoLayer;
            private readonly MetadataLayer _MetadataLayer;
            private readonly ApiLayer _ApiLayer;

        #endregion
        public Bluzelle(string entry, string pemKey, string uuid)
        {
            _ConnectionLayer = new ConnectionLayer(entry);
            _CryptoLayer = new CryptoLayer(pemKey);
            _MetadataLayer = new MetadataLayer(uuid);
            _ApiLayer = new ApiLayer(uuid,_ConnectionLayer, _CryptoLayer, _MetadataLayer);
        }

        #region APILayer
            //DB ops
            public async Task<bool> CreateDB()
            {
                return await _ApiLayer.CreateDB();
            }
            public async Task<bool> DeleteDB()
            {
                return await _ApiLayer.DeleteDB();
            }

            public async Task<bool> HasDB()
            {
                return await _ApiLayer.HasDB();
            }

            // CRUD ops
            public async Task<bool> Create(string Key, string value)
            {
                return await _ApiLayer.Create(Key, value);
            }

            public async Task<string> Read(string Key)
            {
                return await _ApiLayer.Read(Key); 
            }

            public async Task<string> QuickRead(string Key)
            {
                return await _ApiLayer.QuickRead(Key);
            }

            public async Task<bool> Delete(string Key)
            {
                return await _ApiLayer.Delete(Key);
            }

            public async Task<bool> Update(string Key, string value)
            {
                return await _ApiLayer.Update(Key, value);
            }

            // permission ops
            public async Task<string> Writers()
            {
                return await _ApiLayer.Writers();
            }
            public async Task<bool> AddWriters(string writers)
            {
                return await _ApiLayer.AddWriters(writers);
            }

            public async Task<bool> RemoveWriters(string writers)
            {
                return await _ApiLayer.RemoveWriters(writers);
            }

            //admin ops
            public async Task<string> Size()
            {
                return await _ApiLayer.Size();
            }
            public async Task<bool> Has(string Key)
            {
                return await _ApiLayer.Has(Key);
            }
            public async Task<string> Keys()
            {
                return await _ApiLayer.Keys();
            }
        #endregion
    }
}