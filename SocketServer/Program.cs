using System.Net.Sockets;
using System.Net;

namespace SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(60, 10);
            Console.SetBufferSize(60, 10);

            Socket listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(Dns.GetHostEntry("localhost").AddressList[0], 11000));
            listener.Listen(10);
            Console.WriteLine("Waiting for a connection on " + listener.LocalEndPoint);
            while (true) new Server(listener.Accept());
        }
    }
}