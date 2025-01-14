using System.Text.Json;
using AspNetLite.Http;
using AspNetLite.Http.Extensions;
using AspNetLite.Http.Models;

var server = new AppServer();

server.MapGet("{name}/Hello/{anotherName}", (string name, string anotherName) => 
    new { Message = $"Hello, {name}, {anotherName}" });

server.MapPost("", (HttpContext context) =>
{
    Console.WriteLine(JsonSerializer.Serialize(context.Request.RequestBody.ReadAsJson<object>()));
    return new { yourData = context.Request.RequestBody.ReadAsJson<object>() };
});

server.UseMiddleware(async (context, next) =>
{
    Console.WriteLine($"Hello world from middleware 2 for request: {context.Request.Route}");
    await next(context);
});

server.UseControllers();

await server.RunAsync();