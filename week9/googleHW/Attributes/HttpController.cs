namespace googleHW.Attributes;

internal class HttpController: Attribute
{
    public string ControllerName;

    public HttpController(string controllerName)
    {
        ControllerName = controllerName;
    }
}