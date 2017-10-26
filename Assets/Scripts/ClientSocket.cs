using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Assets.Scripts
{
    public class ClientSocket : UnityEngine.MonoBehaviour
    {
        public void connectToServer()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpClient client = new TcpClient();
            client.Connect(ip, 8585);

            Console.WriteLine("Conectado ao server");

            NetworkStream ns = client.GetStream();

            Thread thread = new Thread(o => ReceiveData((TcpClient)o));
            thread.Start(client);

            string s;
            while (true)
            {
                byte[] buffer = Encoding.ASCII.GetBytes("");
                ns.Write(buffer, 0, buffer.Length);
            }

            client.Client.Shutdown(SocketShutdown.Send);
            thread.Join();
            ns.Close();
            client.Close();

        }

        static void ReceiveData(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            byte[] receivedBytes = new byte[1024];
            int byte_count;

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                Console.Write(Encoding.ASCII.GetString(receivedBytes, 0, byte_count));
            }
        }
    }
}
