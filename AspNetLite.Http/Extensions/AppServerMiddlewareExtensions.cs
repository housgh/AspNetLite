using AspNetLite.Http.Models;

namespace AspNetLite.Http.Extensions;

public static class AppServerMiddlewareExtensions
{
    public static void UseMiddleware(this AppServer appServer, Func<HttpContext, Func<HttpContext, Task>, Task> action)
    {
        appServer.Middlewares.Add(action);
    }

    internal static async Task RunPipeline(this AppServer appServer, HttpContext context,
        CancellationToken cancellationToken)
    {
        var middlewares = appServer.Middlewares
            .Reverse();

        var next = new Func<HttpContext, Task>(_ => Task.CompletedTask);
        foreach (var middleware in middlewares)
        {
            if (cancellationToken.IsCancellationRequested) return;
            var middlewareWithNext = new Func<HttpContext, Func<HttpContext ,Task>, Task>(async (ctx, nxt) => await middleware(ctx, nxt));
            var nextMiddleware = next;
            next = ctx =>  middlewareWithNext(ctx, nextMiddleware);
        }

        await next(context);
    }
}