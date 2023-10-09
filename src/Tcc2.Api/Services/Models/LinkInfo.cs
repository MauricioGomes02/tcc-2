using Microsoft.Extensions.Primitives;

namespace Tcc2.Api.Services.Models;

public class LinkInfo<TResource> where TResource : class
{
    private readonly IDictionary<string, Func<TResource, long>>? _getIds;

    public LinkInfo(
        string controllerName,
        string routeName,
        string rel,
        IDictionary<string, StringValues>? queryString = null,
        IDictionary<string, Func<TResource, long>>? getIds = null,
        bool includeFind = true)
    {
        ControllerName = controllerName;
        RouteName = routeName;
        Rel = rel;
        QueryString = queryString;
        _getIds = getIds;
        IncludeFind = includeFind;
    }

    public string ControllerName { get; private set; }
    public string RouteName { get; private set; }
    public string Rel { get; private set; }
    public IDictionary<string, StringValues>? QueryString { get; private set; }
    public bool IncludeFind { get; private set; }

    public long? GetId(TResource resource, string templateId)
    {
        if (_getIds is not null && _getIds.TryGetValue(templateId, out var getId))
        {
            return getId(resource);
        }

        throw new ArgumentException($"Unable to get id for template {templateId}");
    }
}
