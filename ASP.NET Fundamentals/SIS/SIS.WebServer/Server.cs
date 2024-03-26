using SIS.WebServer.Routing.Contracts;
using System.Net;
using System.Net.Sockets;

namespace SIS.WebServer
{
    public class Server
    {
        private const string LocalhostIpAddress = "127.0.0.1";

        private readonly int port;

        private readonly TcpListener listener;

        private readonly IServerRoutingTable serverRoutingTable;

        private bool isRunnig;

        public Server(int port, IServerRoutingTable serverRoutingTable)
        {
            this.port = port;
            this.listener = new TcpListener(IPAddress.Parse(LocalhostIpAddress), port);

            this.serverRoutingTable = serverRoutingTable;
        }

        public void Run()
        {
            this.listener.Start();
            this.isRunnig = true;

            Console.WriteLine($"Server started at http://{LocalhostIpAddress}:{this.port}");

            while (this.isRunnig)
            {
                Console.WriteLine("Waiting for client...");

                var client = this.listener.AcceptSocketAsync().GetAwaiter().GetResult();

                this.Listen(client);
            }
        }

        public async Task Listen(Socket client)
        {
            var connectionHandler = new ConnectionHandle(client, this.serverRoutingTable);
            await connectionHandler.ProcessRequest();
        }
    }
}
