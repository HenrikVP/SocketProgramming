using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SocketClient
{
    internal class Client
    {
        Socket socket;

        public Client()
        {
            Console.Write("Server IP: ");
            IPAddress? ip = IPAddress.TryParse(Console.ReadLine(), out ip) ? ip : Dns.GetHostEntry("localhost").AddressList[0];
            socket = new(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ip, 11000);
            Console.WriteLine("Connected to: " + socket.RemoteEndPoint);
            
            Game();
            
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        void Game()
        {
            Guid guid = Guid.NewGuid();
            Console.Write("Input name: ");
            string? name = Console.ReadLine();
            int choice;
            do
            {
                Console.WriteLine("SELECT\t1 Rock\t2 Paper\t3 Scissor");
                choice = int.TryParse(Console.ReadLine(), out choice) ? choice : 0;
                ContactServer($"{guid}|{name}|{choice}|<EOM>");
            }
            while (choice != 0);
        }

        void ContactServer(string msg)
        {
            socket.Send(Encoding.ASCII.GetBytes(msg));

            byte[] buffer = new byte[1024];
            int bytesRecieved = socket.Receive(buffer);
            Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, bytesRecieved));
        }
    }
}