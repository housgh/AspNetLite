using System.Net;
using System.Net.Sockets;
using System.Text;
using AspNetLite.Http.Extensions;
using AspNetLite.Http.Models;

namespace AspNetLite.Http;

public static class ServerBootstrapper
{
    public static async Task RunAsync(this AppServer appServer, int port = 8080)
    {
        var serverCancellationTokenSource = new CancellationTokenSource();
        var serverCancellationToken = serverCancellationTokenSource.Token;
        using var listener = new TcpListener(IPAddress.Any, port);
        listener.Start(10);
        Console.WriteLine($"Listening on port {port}");
        while (!serverCancellationToken.IsCancellationRequested)
        {
            using var tcpClient = await listener.AcceptTcpClientAsync(serverCancellationToken);
            var requestCancellationTokenSource = new CancellationTokenSource();
            var requestCancellationToken = requestCancellationTokenSource.Token;
            var context = new HttpContext(tcpClient);
            
            await appServer.RunPipeline(context, requestCancellationToken);
        }
    }
}