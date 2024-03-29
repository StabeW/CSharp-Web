using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

public partial class Program
{
    public static Dictionary<string, int> sessionStore = [];
    public static async Task Main(string[] args)
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 8000);
        tcpListener.Start();

        while (true)
        {
            TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => ProcessClientAsync(tcpClient));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }

    private static async Task ProcessClientAsync(TcpClient tcpClient)
    {
        const string NewLine = "\r\n";
        using NetworkStream networkStream = tcpClient.GetStream();
        byte[] requestBytes = new byte[10000];

        int bytesRead = await networkStream.ReadAsync(requestBytes);
        string request = Encoding.UTF8.GetString(requestBytes, 0, bytesRead);

        var sid = MyRegex().Match(request).Value?.Replace("sid=", string.Empty);

        var newSid = Guid.NewGuid().ToString();

        var count = 0;

        if (sessionStore.ContainsKey(sid))
        {
            sessionStore[sid]++;
            count = sessionStore[sid];
        }
        else
        {
            sid = null;
            sessionStore[newSid] = 1;
            count = 1;
        }

        string responseText =
            @"<form action='/Account/Login' method= 'post'> 
            <input type=data name='data' />
            <input type=text name='username' />
            <input type=password name='password' />
            <input type=submit value='login' />
            </form>" + "<h1>" + count + "</h1>" + 
            "<h1>" + DateTime.UtcNow + "</h1>";
        
        string response = "HTTP/1.0 200 OK" + NewLine +
                          "Server: SoftUniServer/1.0" + NewLine +
                          "Content-Type: text/html" + NewLine +
                        (string.IsNullOrWhiteSpace(sid) ?
                        ("Set-Cookie: sid=" + newSid + NewLine)
                        : string.Empty) +
                          "Set-Cookie: name=StabeW; path=/; Max-Age=3200; HttpOnly; " + NewLine +
                          "Set-Cookie: lang=en; path=/; HttpOnly; Max-Age=3200" + NewLine +
                          "Content-Lenght: " + responseText.Length + NewLine +
                          NewLine +
                          responseText;
        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
        await networkStream.WriteAsync(responseBytes);
        Console.WriteLine(request);
        Console.WriteLine(new string('=', 60));
    }

    [GeneratedRegex(@"sid=[^\n]*\n")]
    private static partial Regex MyRegex();
}