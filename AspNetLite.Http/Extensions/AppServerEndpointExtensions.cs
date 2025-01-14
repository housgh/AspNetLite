namespace AspNetLite.Http.Extensions;

public static class AppServerEndpointExtensions
{
    public static void MapGet(this AppServer appServer, string route, Delegate endpoint)
    {
        var added = appServer.Endpoints.TryAdd(($"/{route}", HttpMethod.Get), endpoint);
        if(!added)
            throw new InvalidOperationException($"Route {route} already exists (GET).");
    }
    
    public static void MapPost(this AppServer appServer, string route, Delegate endpoint)
    {
        var added = appServer.Endpoints.TryAdd(($"/{route}", HttpMethod.Post), endpoint);
        if(!added)
            throw new InvalidOperationException($"Route {route} already exists (POST).");
    }
    
    public static void MapPut(this AppServer appServer, string route, Delegate endpoint)
    {
        var added = appServer.Endpoints.TryAdd(($"/{route}", HttpMethod.Put), endpoint);
        if(!added)
            throw new InvalidOperationException($"Route {route} already exists (PUT).");
    }
    
    public static void MapPatch(this AppServer appServer, string route, Delegate endpoint)
    {
        var added = appServer.Endpoints.TryAdd(($"/{route}", HttpMethod.Patch), endpoint);
        if(!added)
            throw new InvalidOperationException($"Route {route} already exists (PATCH).");
    }
    
    public static void MapDelete(this AppServer appServer, string route, Delegate endpoint)
    {
        var added = appServer.Endpoints.TryAdd(($"/{route}", HttpMethod.Delete), endpoint);
        if(!added)
            throw new InvalidOperationException($"Route {route} already exists. (DELETE)");
    }
}