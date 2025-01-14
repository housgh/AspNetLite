using System.Net.Sockets;

namespace AspNetLite.Http.Models;

public class HttpContext
{
    public HttpRequest Request { get; set; }
    public HttpResponse Response { get; set; }
    internal HttpContext(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();
        Request = HttpRequest.FromStream(stream);
        Response = new HttpResponse(new StreamWriter(stream));
    }
}