using System.Net;
using AspNetLite.Http.Models;

namespace AspNetLite.Http.Extensions;

public static class AppServerControllerExtensions
{
    public static void UseControllers(this AppServer appServer)
    {
        appServer.UseMiddleware(async (context, next) => 
            await HandleRequest(appServer, context, next));
    }

    private static async Task HandleRequest(AppServer appServer, HttpContext context, Func<HttpContext, Task> next)
    {
        try
        {
            var method = context.Request.Method;
            var route = context.Request.Route;

            var response = context.Response;
        
            var endpointData = appServer.Endpoints.FromRoute(route, method);

            if (endpointData.Endpoint is null || endpointData.Data is null)
            {
                await ReturnStatusCode(response, HttpStatusCode.NotFound);
                return;
            }
        
            context.Request.RouteData = endpointData.Data;

            var arguments = GetEndpointArguments(context, endpointData.Endpoint, endpointData.Data);
            var responseData= endpointData.Endpoint.DynamicInvoke(arguments);

            if (responseData is null)
            {
                await ReturnStatusCode(response, HttpStatusCode.NoContent);
                return;
            }
        
            await ReturnStatusCode(response, HttpStatusCode.OK, responseData);
        
            await next(context);
        }
        catch (Exception ex)
        {
            await ReturnStatusCode(context.Response, HttpStatusCode.InternalServerError, ex.ToString(), "text/plain");
        }
    }

    private static object?[] GetEndpointArguments(HttpContext context, Delegate endpoint, RouteData routeData)
    {
        var parameters = endpoint.Method.GetParameters();

        var arguments = new List<object?>();
        foreach (var parameter in parameters)
        {
            if (parameter.ParameterType == typeof(HttpContext))
            {
                arguments.Add(context);
            }

            if (!string.IsNullOrWhiteSpace(parameter.Name) && 
                routeData.RouteParameters.TryGetValue(parameter.Name, out var routeParameter))
            {
                arguments.Add(Convert.ChangeType(routeParameter, parameter.ParameterType));
                continue;
            }
            
            if (!string.IsNullOrWhiteSpace(parameter.Name) && 
                routeData.QueryParameters.TryGetValue(parameter.Name, out var queryParameter))
            {
                arguments.Add(Convert.ChangeType(queryParameter, parameter.ParameterType));
            }
        }

        return arguments.ToArray();
    }

    private static async Task ReturnStatusCode(HttpResponse response, HttpStatusCode statusCode, object? content = null, string? contentType = null)
    {
        var stream = response.Writer;
        response.StatusCode = statusCode;
        response.Content = content;
        response.Headers.ContentType = contentType ?? "application/json";
        await stream.WriteAsync(response.ToString());
        await stream.FlushAsync();
    }
}