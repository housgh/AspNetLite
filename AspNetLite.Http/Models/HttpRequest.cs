using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace AspNetLite.Http.Models;

public class HttpRequest
{
    public StreamReader Reader { get; set; }
    public HeaderCollection Headers { get; set; } = [];
    public HttpMethod Method { get; set; } = HttpMethod.Get;
    public string Route { get; set; } = "/";
    public string HttpVersion { get; set; } = "1.1";
    public RouteData RouteData { get; set; } = new();
    public HttpRequestBody RequestBody { get; set; } = new(string.Empty);

    private HttpRequest(StreamReader reader)
    {
        Reader = reader;
    }

    internal static HttpRequest FromStream(NetworkStream stream)
    {
        var buffer = new byte[1_024];
        var bytes = stream.Read(buffer, 0, buffer.Length);
        var requestString = Encoding.UTF8.GetString(buffer.Take(bytes).ToArray());
        Console.WriteLine(requestString);
        if (bytes == 0 || string.IsNullOrWhiteSpace(requestString))
            throw new InvalidOperationException("Could not read request stream.");
        var request = new HttpRequest(new StreamReader(stream));
        request.Parse(requestString);
        return request;
    }

    private void Parse(string requestString)
    {
        var lines = requestString.Split(Environment.NewLine);
        if(lines.Length <= 0)
            throw new InvalidOperationException("Could not read request stream.");
        ParseRequestInformation(lines[0]);
        var headers = new List<string>();
        var index = 1;
        while (!string.IsNullOrWhiteSpace(lines[index]))
        {
            headers.Add(lines[index]);
            index++;
        }
        ParseHeaders(headers);
        
        if(Method == HttpMethod.Get) return;
        
        if (Headers.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
        {
            ParseBodyAsJson(lines.Skip(index + 1));
        }

        if (Headers.ContentType.Contains("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
        {
            ParseBodyAsUrlEncoded(lines.Skip(index + 1));
        }
    }

    private void ParseBodyAsUrlEncoded(IEnumerable<string> lines)
    {
        var dictionary = new Dictionary<string, string>();
        foreach (var line in lines)
        {
            var keyValuePair = line.Split("=");
            if(keyValuePair.Length != 2)
                throw new InvalidOperationException("Could not parse request body.");
            dictionary.Add(keyValuePair[0], keyValuePair[1]);
        }

        var body = JsonSerializer.Serialize(dictionary);
        RequestBody = new HttpRequestBody(body);
    }

    private void ParseBodyAsJson(IEnumerable<string> skip)
    {
        var body = string.Join(string.Empty , skip);
        RequestBody = new HttpRequestBody(body);
    }

    private void ParseRequestInformation(string information)
    {
        var segments = information.Split(" ");
        if(segments.Length < 3)
            throw new InvalidOperationException("Could not read request stream.");
        var httpMethodString = segments[0];
        Method = new HttpMethod(httpMethodString);
        Route = segments[1].StartsWith('/') ? segments[1] : $"/{segments[1]}";
        var versionStringSegments = segments[2].Split('/');
        if(versionStringSegments.Length < 2)
            throw new InvalidOperationException("Invalid or unsupported HTTP version.");
        HttpVersion = versionStringSegments[1].Trim();
    }

    private void ParseHeaders(IEnumerable<string> headersList)
    {
        var dictionary = headersList.Where(headerString => !string.IsNullOrWhiteSpace(headerString))
            .Select(headerString =>
        {
            var keyValuePair = headerString.Split(':', 2);
            if(keyValuePair.Length < 2)
                throw new InvalidOperationException("Invalid or unsupported HTTP header.");
            return new KeyValuePair<string, string>(keyValuePair[0].Trim(), keyValuePair[1].Trim());
        }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        Headers = new HeaderCollection(dictionary);
    }
}