namespace googleHW.Attributes;

internal class HttpPOST: Attribute
{
    public string MethodURI;

    public HttpPOST(string methodUri)
    {
        MethodURI = methodUri;
    }
}