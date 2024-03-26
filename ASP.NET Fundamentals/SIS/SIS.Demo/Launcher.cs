using SIS.Demo;
using SIS.HTTP.Enums;
using SIS.WebServer;
using SIS.WebServer.Routing;
using SIS.WebServer.Routing.Contracts;

public class Launcher
{
    public static void Main(string[] args)
    {
        IServerRoutingTable serverRoutingTable = new ServerRoutingTable();

        serverRoutingTable.Add(HttpRequestMethod.Get,
            path:"/",
            func:request => new HomeController().Index(request));

        Server server = new Server(port:8000, serverRoutingTable);

        server.Run();
    }
}