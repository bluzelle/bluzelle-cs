using System;
using System.Threading.Tasks;
using WebSocketSharp;
using Google.Protobuf;
using System.IO;


namespace bluzelle_cs
{
    public class ConnectionLayer
    {

        private string url;
        private ulong request_id;
        public ConnectionLayer(string url)
        {
            this.url = url;
        }


        public bzn_envelope BuildRequest(bzn_msg msg, string uuid)
        {
            var env = new bzn_envelope()
            {
                DatabaseMsg = msg.Db.ToByteString(),
            };

            return env;
        }

        public async Task<database_response> SendRequestToSocket(byte[] req)
        {
            byte[] response = null;
            bool failed = false;
            Utils utils = new Utils();
            using (var ws = new WebSocket(url))
            {
                ws.OnError += (sender, e) =>
                {
                    failed = true;
                };

                ws.OnMessage += (sender, e) =>
                {
                    response = e.RawData;
                };

                ws.OnClose += (sender, e) =>
                {
                };

                ws.Connect();

                try
                {
                    ws.Send(req);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }

                while (response == null)
                {
                    await Task.Delay(100);
                    if (failed)
                    {
                        return null;
                    }
                }

                bzn_envelope env = new bzn_envelope();
                database_response dbresp = new database_response();

                using (var stream = new MemoryStream(response))
                {
                    env = bzn_envelope.Parser.ParseFrom(stream);
                    dbresp = database_response.Parser.ParseFrom(env.DatabaseResponse);
                    if (dbresp.Redirect != null) {
                        this.url = $"ws://{dbresp.Redirect.LeaderHost}:{dbresp.Redirect.LeaderPort}"; 
                        return await SendRequestToSocket(req);
                    }
                    return dbresp;
                }
                 
            }
        }

        public async Task<database_response> DoRequest(bzn_envelope req, string uuid)
        {
            // request_id++;
            // var req = BuildRequest(msg, request_id, uuid);

            var response = await SendRequestToSocket(req.ToByteArray());
            return response; 
        }
    }
}