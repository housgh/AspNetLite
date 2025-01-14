using AspNetLite.Http.Models;

namespace AspNetLite.Http.Extensions;

public static class EndpointExtensions
{
    public static (RouteData? Data, Delegate? Endpoint) FromRoute(this Dictionary<(string route, HttpMethod method), Delegate> endpoints, 
        string relativePath, HttpMethod method)
    {
        var routeData = new RouteData();
        var segments = relativePath.Split('?');

        if (segments.Length > 1)
        {
            var queryParameters = segments[1].Split('&');
        
            routeData.QueryParameters = queryParameters
                .Select(p => p.Split('='))
                .ToDictionary(p => p[0], p => p.Length < 2 ? string.Empty : p[1]);
        }
        
        var routeExists = endpoints.TryGetValue((relativePath, method), out var endpoint);
        
        if(routeExists)
            return (routeData, endpoint);
        
        foreach (var endpointRoute in endpoints)
        {
            var routeSegments = endpointRoute.Key.route.Split('/').ToList();
            var relativePathSegments = segments[0].Split('/').ToList();
            if(routeSegments.Count != relativePathSegments.Count)
                continue;
            var isRoute = true;
            var index = 0;
            foreach (var element in routeSegments)
            {
                if (!element.Equals(relativePathSegments[index], StringComparison.OrdinalIgnoreCase))
                {
                    if (!element.StartsWith('{') || !element.EndsWith('}'))
                    {
                        isRoute = false;
                        break;
                    }
                    routeData.RouteParameters.Add(element[1..^1], relativePathSegments[index]);
                }
                index++;
            }
            if(isRoute)
                return (routeData, endpointRoute.Value);
        }

        return (null, null);
    }
}