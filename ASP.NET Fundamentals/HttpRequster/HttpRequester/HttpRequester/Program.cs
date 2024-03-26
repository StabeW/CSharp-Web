using System.Net.Sockets;
using System.Net;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 8000);
        tcpListener.Start();
        while (true)
        {
            TcpClient tcpClient = tcpListener.AcceptTcpClient();

            ProcessClient(tcpClient);
        }
    }

    private static void ProcessClient(TcpClient tcpClient)
    {
        const string NewLine = "\r\n";
        using NetworkStream networkStream = tcpClient.GetStream();
        byte[] requestBytes = new byte[10000];

        int bytesRead = networkStream.Read(requestBytes, 0, requestBytes.Length);
        string request = Encoding.UTF8.GetString(requestBytes, 0, bytesRead);

        string responseText = @"<form action='/Account/Login' method= 'post'> 
<input type=data name='data' />
<input type=text name='username' />
<input type=password name='password' />
<input type=submit value='login' />
</form>" + "<h1>" + DateTime.UtcNow + "</h1>";
        Thread.Sleep(1000);
        string response = "HTTP/1.0 200 OK" + NewLine +
                          "Server: SoftUniServer/1.0" + NewLine +
                          "Content-Type: text/html" + NewLine +
                          // "Location: https://google.com" + NewLine +
                          // "Content-Disposition: attachment; filename=stabew.html" + NewLine +
                          "Content-Lenght: " + responseText.Length + NewLine +
                          NewLine +
                          responseText;
        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
        networkStream.Write(responseBytes);
        Console.WriteLine(request);
        Console.WriteLine(new string('=', 60));
    }
}