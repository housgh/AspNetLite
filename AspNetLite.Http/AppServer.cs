using AspNetLite.Http.Models;

namespace AspNetLite.Http;

public class AppServer
{
    internal readonly Dictionary<(string route, HttpMethod method), Delegate> Endpoints = [];
    internal readonly ICollection<Func<HttpContext, Func<HttpContext, Task>, Task>> Middlewares = [];
}