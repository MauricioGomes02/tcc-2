using Hal;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.RegularExpressions;
using Tcc2.Api.Interfaces.Services;
using Tcc2.Api.Services.Models;
using Tcc2.Domain.Entities;
using Tcc2.Domain.Pagination;

namespace Tcc2.Api.Services;

public class HateoasGeneratorService : IHateoasGeneratorService
{
    private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

    public HateoasGeneratorService(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
    {
        _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
    }

    public string GenerateForGetOne<T>(T @object, IEnumerable<LinkInfo<T>> linkInfos) where T : class, IEntity
    {
        var resource = GetResourceForOne(@object, linkInfos);
        return resource.ToString();
    }

    public string GenerateForGetMany<T>(
        Paginated<T> @object,
        LinkInfo<T> self,
        string embeddedName,
        IEnumerable<LinkInfo<T>> relatedItems) where T : class, IEntity
    {
        var resource = GetResourceForMany(@object, self, embeddedName, relatedItems);
        return resource.ToString();
    }

    public string GenerateForGetMany<T>(
        IReadOnlyCollection<T> @object,
        LinkInfo<T> self,
        string embeddedName,
        IEnumerable<LinkInfo<T>> relatedItems) where T : class, IEntity
    {
        var resource = GetResourceForMany(@object, self, embeddedName, relatedItems);
        return resource.ToString();
    }

    private Resource GetResourceForOne<T>(T @object, IEnumerable<LinkInfo<T>> linkInfos) where T : class, IEntity
    {
        var resource = new Resource
        {
            State = @object
        };

        var links = GetLinksForOne(@object, linkInfos);

        resource.Links = links;

        return new Resource
        {
            State = @object,
            Links = links
        };
    }

    private LinkCollection GetLinksForOne<T>(T @object, IEnumerable<LinkInfo<T>> linkInfos) where T : class, IEntity
    {
        var collection = new LinkCollection();

        foreach (var linkInfo in linkInfos)
        {
            var href = GetHRefForOne(@object, linkInfo);
            var link = GetLink(linkInfo.Rel, href);

            collection.Add(link);
        }

        return collection;
    }

    private Resource GetResourceForMany<T>(
        Paginated<T> @object,
        LinkInfo<T> self,
        string embeddedName,
        IEnumerable<LinkInfo<T>> linkInfos) where T : class, IEntity
    {
        var resource = new Resource
        {
            State = default,
        };

        var links = GetLinksForMany(@object, self);
        var embeddedLinks = GetEmbeddedLinksForMany(@object.Items, embeddedName, linkInfos);

        resource.Links = links;

        return new Resource
        {
            State = default,
            Links = links,
            EmbeddedResources = embeddedLinks
        };
    }

    private Resource GetResourceForMany<T>(
        IReadOnlyCollection<T> @object,
        LinkInfo<T> self,
        string embeddedName,
        IEnumerable<LinkInfo<T>> linkInfos) where T : class, IEntity
    {
        var resource = new Resource
        {
            State = default,
        };

        var links = GetLinksForMany(self);
        var embeddedLinks = GetEmbeddedLinksForMany(@object, embeddedName, linkInfos);

        resource.Links = links;

        return new Resource
        {
            State = default,
            Links = links,
            EmbeddedResources = embeddedLinks
        };
    }

    private LinkCollection GetLinksForMany<T>(Paginated<T> @object, LinkInfo<T> linkInfo) where T : class, IEntity
    {
        var collection = new LinkCollection();

        var getManyRoute = _actionDescriptorCollectionProvider
               .ActionDescriptors
               .Items
               .First(x => x.DisplayName!.Contains(linkInfo.ControllerName)
                   && x.AttributeRouteInfo!.Name == linkInfo.RouteName);

        var template = getManyRoute.AttributeRouteInfo!.Template!;
        var queryString = linkInfo.QueryString!;

        var matches = Regex.Matches(template, @"\{.*?\}");
        var hrefBase = "/" + template;
        foreach (var match in matches)
        {
            var templateId = match.ToString()!;
            hrefBase = hrefBase.Replace(templateId, linkInfo.GetId(default!, templateId).ToString());
        }

        var href = QueryHelpers.AddQueryString(hrefBase, queryString);
        var selfLink = GetLink("self", href);
        collection.Add(selfLink);

        if (linkInfo.IncludeFind)
        {
            var findlink = GetLink("find", $"{hrefBase}/{{?id}}", true);
            collection.Add(findlink);
        }

        var pageIndex = @object.PageIndex;
        var totalPages = @object.TotalPages;

        // Previous and first
        if (pageIndex > 0)
        {
            if ((pageIndex - 1) <= (totalPages - 1))
            {
                queryString[nameof(pageIndex)] = $"{pageIndex - 1}";
                href = QueryHelpers.AddQueryString(template, queryString);
                var previousLink = GetLink("previous", href);
                collection.Add(previousLink);
            }

            queryString[nameof(pageIndex)] = "0";
            href = QueryHelpers.AddQueryString(template, queryString);
            var firstLink = GetLink("first", href);
            collection.Add(firstLink);
        }

        // Next and last
        if (pageIndex < totalPages - 1)
        {
            if ((pageIndex + 1) >= 0)
            {
                queryString[nameof(pageIndex)] = $"{pageIndex + 1}";
                href = QueryHelpers.AddQueryString(template, queryString);
                var nextLink = GetLink("next", href);
                collection.Add(nextLink);
            }

            queryString[nameof(pageIndex)] = $"{totalPages - 1}";
            href = QueryHelpers.AddQueryString(template, queryString);
            var lastLink = GetLink("last", href);
            collection.Add(lastLink);
        }

        return collection;
    }

    private LinkCollection GetLinksForMany<T>(LinkInfo<T> linkInfo) where T : class, IEntity
    {
        var getManyRoute = _actionDescriptorCollectionProvider
               .ActionDescriptors
               .Items
               .First(x => x.DisplayName!.Contains(linkInfo.ControllerName)
                   && x.AttributeRouteInfo!.Name == linkInfo.RouteName);

        var template = getManyRoute.AttributeRouteInfo!.Template!;

        var matches = Regex.Matches(template, @"\{.*?\}");
        var href = "/" + template;
        foreach (var match in matches)
        {
            var templateId = match.ToString()!;
            href = href.Replace(templateId, linkInfo.GetId(default!, templateId).ToString());
        }

        var collection = new LinkCollection();
        
        var selfLink = GetLink("self", href);
        var findLink = GetLink("find", $"{href}/{{?id}}", true);

        collection.Add(selfLink);
        collection.Add(findLink);

        return collection;
    }

    private EmbeddedResourceCollection GetEmbeddedLinksForMany<T>(
        IReadOnlyCollection<T> objects,
        string embeddedName,
        IEnumerable<LinkInfo<T>> linkInfos) where T : class, IEntity
    {
        var collection = new EmbeddedResourceCollection();
        var resources = new ResourceCollection();

        foreach (var @object in objects)
        {
            var resouce = GetResourceForOne(@object, linkInfos);
            resources.Add(resouce);
        }

        var embeddedResource = new EmbeddedResource
        {
            Name = embeddedName,
            Resources = resources
        };

        collection.Add(embeddedResource);
        return collection;
    }

    private string GetHRefForOne<T>(T @object, LinkInfo<T> linkInfo) where T : class, IEntity
    {
        var getOneRoute = _actionDescriptorCollectionProvider
               .ActionDescriptors
               .Items
               .First(x => x.DisplayName!.Contains(linkInfo.ControllerName)
                   && x.AttributeRouteInfo!.Name == linkInfo.RouteName);

        var template = getOneRoute.AttributeRouteInfo!.Template!;

        var matches = Regex.Matches(template, @"\{.*?\}");
        var href = "/" + template;
        foreach (var match in matches)
        {
            var templateId = match.ToString()!;
            href = href.Replace(templateId, linkInfo.GetId(@object, templateId).ToString());
        }

        return href;
    }

    private static Link GetLink(string rel, string href, bool isTemplate = false)
    {
        var link = new Link(rel);
        var linkItemCollection = new LinkItemCollection();

        var linkItem = new LinkItem(href);

        if (isTemplate)
        {
            linkItem.Templated = true;
        }

        linkItemCollection.Add(linkItem);

        link.Items = linkItemCollection;

        return link;
    }
}
