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
    public class ApiLayer : ISwarm
    {
        private ConnectionLayer connection;
        private CryptoLayer encrypt;
        private MetadataLayer metadata;
        private string dbUuid;
        private ulong request_id;
        public ApiLayer(string uuid, ConnectionLayer builtConnection, CryptoLayer crypt, MetadataLayer ml)
        {
            this.dbUuid = uuid;
            this.connection = builtConnection;
            this.encrypt = crypt;
            this.metadata = ml;
        }

        //DB ops
        public async Task<bool> CreateDB()
        {
            request_id++;
            var msg = new bzn_msg() 
            { 
                Db = new database_msg() 
                { 
                    CreateDb = new database_request()
                } 
            };
            msg = this.metadata.SetNonceHeader(msg);
            Console.WriteLine(msg.Db.Header.DbUuid);
            Console.WriteLine(msg.Db.Header.Nonce);
            var env = this.connection.BuildRequest(msg, this.dbUuid);
            env = this.encrypt.SignEnvelope(env);
            var response = await this.connection.DoRequest(env, this.dbUuid);
            this.metadata.RemoveNonceHeader(response);
            return response != null || String.IsNullOrEmpty(response.Error.Message);
        }
        public async Task<bool> DeleteDB()
        {
            request_id++;
            var msg = new bzn_msg() 
            { 
                Db = new database_msg() 
                { 
                    DeleteDb = new database_request()
                } 
            };
            var env = this.connection.BuildRequest(msg, this.dbUuid);
            env = this.encrypt.SignEnvelope(env);
            var response = await this.connection.DoRequest(env, this.dbUuid);
            return response != null || String.IsNullOrEmpty(response.Error.Message);
        }

        public async Task<bool> HasDB()
        {
            request_id++;
            var msg = new bzn_msg() 
            { 
                Db = new database_msg() 
                { 
                    HasDb = new database_has_db()
                } 
            };
            var env = this.connection.BuildRequest(msg, this.dbUuid);
            env = this.encrypt.SignEnvelope(env);
            var response = await this.connection.DoRequest(env, this.dbUuid);
            
            try
            {
                return response.HasDb.Has;
            }
            catch
            {
                return false;
            }

        }

        // CRUD ops
        public async Task<bool> Create(string Key, string value)
        {
            request_id++;
            var msg = new bzn_msg() 
            { 
                Db = new database_msg() 
                { 
                    Create = new database_create() 
                    { 
                        Key = Key, 
                        Value = ByteString.CopyFrom(value, Encoding.UTF8), 
                    } 
                } 
            };
            var env = this.connection.BuildRequest(msg, this.dbUuid);
            env = this.encrypt.SignEnvelope(env);
            var response = await this.connection.DoRequest(env, this.dbUuid);
            return response != null || String.IsNullOrEmpty(response.Error.Message);
        }

        public async Task<string> Read(string Key)
        {
            request_id++;
            var msg = new bzn_msg() 
            { 
                Db = new database_msg() 
                { 
                    Read = new database_read() 
                    { 
                        Key = Key 
                    } 
                } 
            };

            var env = this.connection.BuildRequest(msg, this.dbUuid);
            env = this.encrypt.SignEnvelope(env);
            var response = await this.connection.DoRequest(env, this.dbUuid);

            if (response != null) 
            {
                try
                {
                    return Encoding.UTF8.GetString(response.Read.Value.ToByteArray());
                }
                catch
                {
                    return response.Error.Message;
                }
            }
            else
            {
                return null;
            } 
        }

        public async Task<string> QuickRead(string Key)
        {
            request_id++;
            var msg = new bzn_msg() 
            { 
                Db = new database_msg() 
                { 
                    QuickRead = new database_read()
                    {
                        Key = Key
                    }
                } 
            };

            var env = this.connection.BuildRequest(msg, this.dbUuid);
            env = this.encrypt.SignEnvelope(env);
            var response = await this.connection.DoRequest(env, this.dbUuid);

            if (response != null) 
            {
                try
                {
                    return Encoding.UTF8.GetString(response.Read.Value.ToByteArray());
                }
                catch
                {
                    return response.Error.Message;
                }
            }
            else
            {
                return null;
            } 
        }

        public async Task<bool> Delete(string Key)
        {
            request_id++;
            var msg = new bzn_msg() 
            { 
                Db = new database_msg() 
                { 
                    Delete = new database_delete() 
                    { 
                        Key = Key 
                    } 
                } 
            };

            var env = this.connection.BuildRequest(msg, this.dbUuid);
            env = this.encrypt.SignEnvelope(env);
            var response = await this.connection.DoRequest(env, this.dbUuid);
            
            return response != null || String.IsNullOrEmpty(response.Error.Message);
        }

        public async Task<bool> Update(string Key, string value)
        {
            request_id++;
            var msg = new bzn_msg() 
            { 
                Db = new database_msg() 
                { 
                    Update = new database_update() 
                    { 
                        Key = Key, 
                        Value = ByteString.CopyFrom(value, Encoding.ASCII) 
                    } 
                } 
            };
            var env = this.connection.BuildRequest(msg, this.dbUuid);
            env = this.encrypt.SignEnvelope(env);
            var response = await this.connection.DoRequest(env, this.dbUuid);
            return response != null || String.IsNullOrEmpty(response.Error.Message);
        }

        // permission ops
        public async Task<string> Writers()
        {
            request_id++;
            var msg = new bzn_msg() 
            { 
                Db = new database_msg() 
                { 
                    Writers = new database_request()
                } 
            };
            var env = this.connection.BuildRequest(msg, this.dbUuid);
            env = this.encrypt.SignEnvelope(env);
            var response = await this.connection.DoRequest(env, this.dbUuid);
            
            if(response != null)
            {
                try
                {
                    return "\nOwner: " + response.Writers.Owner + " \nWriters: " + response.Writers.Writers;
                }
                catch
                {
                    return response.Error.Message;
                }
            }
            else
            {
                return "Something went wrong.";
            }
        }
        public async Task<bool> AddWriters(string writers)
        {
            request_id++;
            var msg = new bzn_msg() 
            { 
                Db = new database_msg() 
                { 
                    AddWriters = new database_writers(){}
                } 
            };

            msg.Db.AddWriters.Writers.Add(writers);

            var env = this.connection.BuildRequest(msg, this.dbUuid);
            env = this.encrypt.SignEnvelope(env);
            var response = await this.connection.DoRequest(env, this.dbUuid);
            return response != null || String.IsNullOrEmpty(response.Error.Message);
        }

        public async Task<bool> RemoveWriters(string writers)
        {
            request_id++;
            var msg = new bzn_msg() 
            { 
                Db = new database_msg() 
                { 
                    RemoveWriters = new database_writers(){}
                } 
            };

            msg.Db.RemoveWriters.Writers.Add(writers);

            var env = this.connection.BuildRequest(msg, this.dbUuid);
            env = this.encrypt.SignEnvelope(env);
            var response = await this.connection.DoRequest(env, this.dbUuid);
            return response != null || String.IsNullOrEmpty(response.Error.Message);
        }

        //admin ops
        public async Task<string> Size()
        {
            request_id++;
            var msg = new bzn_msg() 
            { 
                Db = new database_msg() 
                { 
                    Size = new database_request(){}
                } 
            };
            
            var env = this.connection.BuildRequest(msg, this.dbUuid);
            env = this.encrypt.SignEnvelope(env);
            var response = await this.connection.DoRequest(env, this.dbUuid);

            if (response != null) 
            {
                try
                {
                    return "Size of database in bytes: " + response.Size.Bytes.ToString();
                }
                catch
                {
                    return response.Error.Message;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> Has(string Key)
        {
            request_id++;
            var msg = new bzn_msg() 
            { 
                Db = new database_msg() 
                { 
                    Has = new database_has()
                    {
                        Key = Key
                    }
                } 
            };
            var env = this.connection.BuildRequest(msg, this.dbUuid);
            env = this.encrypt.SignEnvelope(env);
            var response = await this.connection.DoRequest(env, this.dbUuid);
            return response != null || String.IsNullOrEmpty(response.Error.Message);
        }

        public async Task<string> Keys()
        {
            request_id++;
            var msg = new bzn_msg() 
            { 
                Db = new database_msg() 
                { 
                    Keys = new database_request()
                } 
            };
            var env = this.connection.BuildRequest(msg, this.dbUuid);
            env = this.encrypt.SignEnvelope(env);
            var response = await this.connection.DoRequest(env, this.dbUuid);
            
            if(response != null)
            {
                try
                {
                    return "\nOwner: " + response.Keys.Keys;
                }
                catch
                {
                    return response.Error.Message;
                }
            }
            else
            {
                return "Something went wrong.";
            }
        }
    }
}