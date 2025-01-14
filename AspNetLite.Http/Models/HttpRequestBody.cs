using System.Text.Json;

namespace AspNetLite.Http.Models;

public class HttpRequestBody
{
    private readonly string _body;

    internal HttpRequestBody(string body)
    {
        _body = body;
    }
    
    public string ReadAsString() => _body;

    public TBody? ReadAsJson<TBody>()
    {
        return JsonSerializer.Deserialize<TBody>(_body);
    }
}