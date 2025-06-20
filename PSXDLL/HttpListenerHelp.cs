using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PSXDLL
{
    public sealed class HttpListenerHelp : Listener
    {
        public UpdataUrlLog? UpdataUrlLog;

        public HttpListenerHelp(int port)
            : this(IPAddress.Any, port)
        {
        }

        public HttpListenerHelp(IPAddress address, int port)
            : base(port, address)
        {
        }

        public HttpListenerHelp(IPAddress address, int port, UpdataUrlLog updataurlLog)
            : base(port, address)
        {
            UpdataUrlLog = updataurlLog;
        }

        public override string ConstructString => $"Host:{Address};Port:{Port}";

        public override void OnAccept(Socket clientSocket)
        {
            try
            {
                HttpClient client = new(clientSocket, RemoveClient, UpdataUrlLog!);
                AddClient(client);
                client.StartHandshake();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "OnAccept");
            }
        }

        public override string ToString()
        {
            return ($"HTTP service on {Address}:{Port}");
        }
    }
}
