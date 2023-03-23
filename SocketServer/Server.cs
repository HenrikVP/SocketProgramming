using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
    internal class Server
    {
        static List<Player> players = new();

        public Server(Socket socket)
        {
            Thread ctThread = new(() => ClientThread(socket));
            ctThread.Start();
        }

        void ClientThread(Socket socket)
        {
            while (true)
            {
                string? msg = null;
                while (msg == null || !msg.Contains("<EOM>"))
                {
                    byte[] dataIn = new byte[1024];
                    int bytesRecieved = socket.Receive(dataIn);
                    msg += Encoding.ASCII.GetString(dataIn, 0, bytesRecieved);
                }
                Console.WriteLine($"{socket.RemoteEndPoint} : {msg}");

                byte[] dataOut = Encoding.ASCII.GetBytes(Game(msg) + "|<EOM>");
                socket.Send(dataOut);

                foreach (var p in players) p.Choice = 0;
            }
        }

        string Game(string msg)
        {
            string[] arr = msg.Split('|');
            Player player = new() { Guid = arr[0], Name = arr[1], Choice = int.Parse(arr[2]) };

            bool newPlayer = true;
            foreach (var p in players)
                if (p != null && player.Guid == p.Guid)
                {
                    newPlayer = false;
                    p.Choice = player.Choice;
                }
            if (newPlayer) players.Add(player);

            bool allPlayersMadeChoice = false;
            while (!allPlayersMadeChoice || players.Count < 2)
            {
                allPlayersMadeChoice = true;
                foreach (var p in players)
                    if (p.Choice == 0) allPlayersMadeChoice = false;
            }

            string serverMsg = "";
            if (allPlayersMadeChoice && players.Count > 1) serverMsg = CheckResult();

            return serverMsg;
        }

        string CheckResult()
        {
            int[] choices = new int[4];
            foreach (var p in players) choices[p.Choice]++;

            if (choices[1] > 0 && choices[2] == 0 && choices[3] > 0) return "Rock win!";
            else if (choices[1] > 0 && choices[2] > 0 && choices[3] == 0) return "Paper win!";
            else if (choices[1] == 0 && choices[2] > 0 && choices[3] > 0) return "Scissor win!";
            else return "It was a draw!";
        }

        class Player
        {
            public string? Guid { get; set; }
            public string? Name { get; set; }
            public int Choice { get; set; }
        }
    }
}