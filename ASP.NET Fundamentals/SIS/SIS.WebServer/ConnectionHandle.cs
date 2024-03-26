using SIS.HTTP.Common;
using SIS.HTTP.Enums;
using SIS.HTTP.Exceptions;
using SIS.HTTP.Requests;
using SIS.HTTP.Requests.Contracts;
using SIS.HTTP.Responses.Contracts;
using SIS.WebServer.Results;
using SIS.WebServer.Routing.Contracts;
using System.Net.Sockets;
using System.Text;

namespace SIS.WebServer
{
    public class ConnectionHandle
    {
        private Socket client;
        private IServerRoutingTable serverRoutingTable;

        public ConnectionHandle(Socket client, IServerRoutingTable serverRoutingTable)
        {
            CoreValidator.ThrowIfNull(client, nameof(client));
            CoreValidator.ThrowIfNull(serverRoutingTable, nameof(serverRoutingTable));

            this.client = client;
            this.serverRoutingTable = serverRoutingTable;
        }

        public async Task ProcessRequest()
        {
            try
            {
                IHttpRequest httpRequest = await this.ReadRequest();

                if (httpRequest != null)
                {
                    Console.WriteLine($"Processing: {httpRequest.RequestMethod} {httpRequest.Path}...");

                    IHttpResponse httpResponse = this.HandleRequest(httpRequest);

                    this.PrepareResponse(httpResponse);
                }
            }
            catch (BadRequestException e)
            {
                this.PrepareResponse(new TextResult(e.ToString(), HttpResponseStatusCode.BadRequest));
            }
            catch (Exception e) 
            {
                this.PrepareResponse(new TextResult(e.ToString(), HttpResponseStatusCode.InternalServerError));
            }

            this.client.Shutdown(SocketShutdown.Both);
        }

        private async Task<IHttpRequest> ReadRequest()
        {
            StringBuilder result = new StringBuilder();
            ArraySegment<byte> data = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                int numberOfBytesRead = await this.client.ReceiveAsync(data.Array, SocketFlags.None);

                if (numberOfBytesRead == 0)
                {
                    break;
                }

                string bytesAsString = Encoding.UTF8.GetString(data.Array, 0, numberOfBytesRead);

                result.Append(bytesAsString);

                if (numberOfBytesRead < 1023)
                {
                    break;
                }
            }

            if (result.Length == 0)
            {
                return null;
            }

            return new HttpRequest(result.ToString());
        }

        private IHttpResponse HandleRequest(IHttpRequest httpRequest)
        {
            if (!this.serverRoutingTable.Contains(httpRequest.RequestMethod, httpRequest.Path))
            {
                return new TextResult($"Route with method {httpRequest.RequestMethod} and path \"{httpRequest.Path}\" not found.", HttpResponseStatusCode.NotFound);
            }

            return this.serverRoutingTable.Get(httpRequest.RequestMethod, httpRequest.Path).Invoke(httpRequest);
        }

        private void PrepareResponse(IHttpResponse httpResponse)
        {
            byte[] bypeSegments = httpResponse.GetBytes();

            this.client.Send(bypeSegments, SocketFlags.None);
        }
    }
}
