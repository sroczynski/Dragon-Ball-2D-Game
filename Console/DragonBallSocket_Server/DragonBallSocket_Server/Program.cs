using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DragonBallSocket_Server
{
    class Program
    {
        static readonly object _lock = new object();
        static readonly Dictionary<int, TcpClient> jogadores = new Dictionary<int, TcpClient>();

        static void Main(string[] args)
        {
            int count = 1;

            TcpListener ServerSocket = new TcpListener(IPAddress.Any, 8585);
            ServerSocket.Start();

            while (true)
            {
                TcpClient jogador = ServerSocket.AcceptTcpClient();
                lock (_lock) jogadores.Add(count, jogador);

                Console.WriteLine(string.Format("Jogador {0} Conectado", count));

                Thread t = new Thread(HandleClient);
                t.Start(count);

                count++;
            }
        }

        public static void HandleClient(object o)
        {
            int id = (int)o;
            TcpClient client;

            lock (_lock)
                client = jogadores[id];

            while (true)
            {
                NetworkStream stream = client.GetStream();

                byte[] buffer = new byte[1024];
                int byte_count = stream.Read(buffer, 0, buffer.Length);

                if (byte_count == 0)
                {
                    break;
                }

                string data = Encoding.ASCII.GetString(buffer, 0, byte_count);

                broadcast(data);

                Console.WriteLine(data);
            }

            lock (_lock)
                jogadores.Remove(id);

            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        public static void broadcast(string data)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data + Environment.NewLine);

            lock (_lock)
            {
                foreach (TcpClient c in jogadores.Values)
                {
                    NetworkStream stream = c.GetStream();

                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                }
            }
        }
    }
}
