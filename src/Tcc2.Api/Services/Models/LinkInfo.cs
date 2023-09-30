using Microsoft.Extensions.Primitives;

namespace Tcc2.Api.Services.Models;

public class LinkInfo<TResource> where TResource : class
{
    private readonly Func<TResource, long>? _getId;

    public LinkInfo(
        string controllerName,
        string routeName,
        string rel,
        IDictionary<string, StringValues>? queryString = null,
        Func<TResource, long>? getId = null)
    {
        ControllerName = controllerName;
        RouteName = routeName;
        Rel = rel;
        QueryString = queryString;
        _getId = getId;
    }

    public string ControllerName { get; private set; }
    public string RouteName { get; private set; }
    public string Rel { get; private set; }
    public IDictionary<string, StringValues>? QueryString { get; private set; }

    public long? GetId(TResource resource)
    {
        return _getId is null ? null : _getId(resource);
    }
}
