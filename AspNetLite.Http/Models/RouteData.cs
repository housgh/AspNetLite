namespace AspNetLite.Http.Models;

public class RouteData
{
    public Dictionary<string, string> QueryParameters { get; set; } = [];
    public Dictionary<string, string?> RouteParameters { get; set; } = [];
}