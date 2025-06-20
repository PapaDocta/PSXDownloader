using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PSXDLL
{
    public abstract class Client : IDisposable
    {
        private readonly DestroyDelegate? _destroyer;
        private readonly byte[] _buffer;
        private readonly byte[] _remoteBuffer;
        private Socket? _clientSocket;
        private Socket? _destinationSocket;

        protected Client()
        {
            _buffer = new byte[256 * 1024]; //0x1000
            _remoteBuffer = new byte[128 * 1024]; //0x400
            ClientSocket = null;
            _destroyer = null;
        }

        protected Client(Socket clientSocket, DestroyDelegate destroyer)
        {
            _buffer = new byte[256 * 1024]; //0x1000
            _remoteBuffer = new byte[128 * 1024]; //0x400
            ClientSocket = clientSocket;
            _destroyer = destroyer;
        }

        public byte[] Buffer => _buffer;

        public Socket? ClientSocket
        {
            get => _clientSocket;
            set
            {
                if (_clientSocket != null)
                {
                    _clientSocket.Close();
                }
                _clientSocket = value;
            }
        }

        public Socket? DestinationSocket
        {
            get => _destinationSocket;
            set
            {
                if (_destinationSocket != null)
                {
                    _destinationSocket.Close();
                }
                _destinationSocket = value;
            }
        }

        public byte[] RemoteBuffer => _remoteBuffer;

        public void Dispose()
        {
            try
            {
                if (ClientSocket != null)
                {
                    ClientSocket.Shutdown(SocketShutdown.Both);
                }
            }
            catch
            {
            }
            try
            {
                if (DestinationSocket != null)
                {
                    DestinationSocket.Shutdown(SocketShutdown.Both);
                }
            }
            catch
            {
            }
            if (ClientSocket != null)
            {
                ClientSocket.Close();
            }
            if (DestinationSocket != null)
            {
                DestinationSocket.Close();
            }
            ClientSocket = null;
            DestinationSocket = null;
            _destroyer?.Invoke(this);
        }

        private static async Task RelayAsync(Socket source, Socket destination, byte[] buffer)
        {
            while (true)
            {
                int size;
                try
                {
                    size = await source.ReceiveAsync(buffer, SocketFlags.None);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "RelayReceive");
                    break;
                }

                if (size <= 0)
                {
                    break;
                }

                try
                {
                    await destination.SendAsync(buffer.AsMemory(0, size), SocketFlags.None);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "RelaySend");
                    break;
                }
            }
        }

        public async Task StartRelayAsync()
        {
            if (ClientSocket == null || DestinationSocket == null)
            {
                Dispose();
                return;
            }

            Task t1 = RelayAsync(ClientSocket, DestinationSocket, Buffer);
            Task t2 = RelayAsync(DestinationSocket, ClientSocket, RemoteBuffer);
            await Task.WhenAny(t1, t2);
            Dispose();
        }

        public abstract void StartHandshake();

        public void StartRelay()
        {
            _ = StartRelayAsync();
        }

        public override string ToString()
        {
            try
            {
                return ($"connecting： {((IPEndPoint)DestinationSocket!.RemoteEndPoint!).Address}");
            }
            catch
            {
                return "Connection established successfully";
            }
        }
    }
}
