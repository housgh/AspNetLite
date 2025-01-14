using System.Net;
using System.Text;
using System.Text.Json;

namespace AspNetLite.Http.Models;

public class HttpResponse(StreamWriter writer)
{
    public StreamWriter Writer { get; set; } = writer;
    public HeaderCollection Headers { get; set; } = [];
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    public object? Content { get; set; }
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"HTTP/1.1 {GetResponseCode()}").Append(Environment.NewLine);
        foreach (var keyValuePair in Headers)
        {
            sb.Append($"{keyValuePair.Key}: {keyValuePair.Value}")
                .Append(Environment.NewLine);
        }

        sb.Append(Environment.NewLine);
        if (Content is null) return sb.ToString();
        sb.Append(JsonSerializer.Serialize(Content));
        return sb.ToString();
    }

    private string GetResponseCode()
    {
        return $"{(int)StatusCode} {StatusCode}";
    }
}