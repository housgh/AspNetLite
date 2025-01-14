namespace AspNetLite.Http.Models;

public class HeaderCollection : Dictionary<string, string>
{
    internal HeaderCollection() { }
    internal HeaderCollection(Dictionary<string, string> headers)
    {
        foreach (var keyValuePair in headers)
        {
            Add(keyValuePair.Key, keyValuePair.Value);
        }
    }

    public string Authorization
    {
        get => this["Authorization"];
        set => this["Authorization"] = value;
    }

    public string ContentType
    {
        get => this["Content-Type"];
        set => this["Content-Type"] = value;
    }
}