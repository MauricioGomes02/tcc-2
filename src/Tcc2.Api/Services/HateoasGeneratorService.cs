using Hal;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using Tcc2.Api.Interfaces.Services;
using Tcc2.Api.Services.Models;
using Tcc2.Application.Models;
using Tcc2.Domain.Pagination;

namespace Tcc2.Api.Services;

public class HateoasGeneratorService : IHateoasGeneratorService
{
    private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

    public HateoasGeneratorService(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
    {
        _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
    }

    public string GenerateForGetOne<T>(T @object, IEnumerable<LinkInfo<T>> linkInfos) where T : class, IIdentity
    {
        var resource = GetResourceForOne(@object, linkInfos);
        return resource.ToString();
    }

    public string GenerateForGetMany<T>(
        Paginated<T> @object,
        LinkInfo<T> self,
        string embeddedName,
        IEnumerable<LinkInfo<T>> relatedItems) where T : class, IIdentity
    {
        var resource = GetResourceForMany(@object, self, embeddedName, relatedItems);
        return resource.ToString();
    }

    private LinkCollection GetPaginationLinkCollection(
        Paginated<IIdentity> @object,
        HttpRequest request)
    {
        var links = new LinkCollection();

        //// Self link
        //var path = request.Path;
        //var queryString = request.QueryString;
        //var self = GetLink("self", hrefs: path + queryString);
        //links.Add(self);

        //var totalPages = @object.TotalPages;
        //var pageIndex = @object.PageIndex;
        //var pageSize = @object.PageSize;

        //// Previous and First link
        //if (pageIndex > 0)
        //{
        //    var index = (pageIndex - 1);
        //    var query = GetQueryString(index, pageSize);
        //    var previous = GetLink("previous", hrefs: path + query);
        //    links.Add(previous);

        //    index = 0;
        //    query = GetQueryString(index, pageSize);
        //    var first = GetLink("first", hrefs: path + query);
        //    links.Add(first);
        //}

        //// Next link
        //if (pageIndex < (totalPages - 1))
        //{
        //    var index = (pageIndex + 1);
        //    var query = GetQueryString(index, pageSize);
        //    var next = GetLink("next", hrefs: path + query);
        //    links.Add(next);

        //    index = (totalPages - 1);
        //    query = GetQueryString(index, pageSize);
        //    var last = GetLink("last", hrefs: path + query);
        //    links.Add(last);
        //}

        //var getOnePersonRoute = _actionDescriptorCollectionProvider
        //    .ActionDescriptors
        //    .Items
        //    .First(x => x.DisplayName!.Contains(nameof(PersonController))
        //        && x.AttributeRouteInfo!.Name == "GetOnePerson");

        //var template = getOnePersonRoute.AttributeRouteInfo!.Template!;

        //var find = GetLink("find", true, template.Replace("/{id}", "{?id}"));
        //links.Add(find);

        return links;
    }

    //private EmbeddedResourceCollection GetPaginationEmbeddedLink(
    //    string name,
    //    IReadOnlyCollection<Identity> @objects)
    //{
    //    var embeddedLink = new EmbeddedResourceCollection();
    //    var resource = new EmbeddedResource()
    //    {
    //        Name = name,
    //        Resources = GetPaginationResourceCollection(@objects)
    //    };
    //    embeddedLink.Add(resource);

    //    return embeddedLink;
    //}

    //private ResourceCollection GetPaginationResourceCollection(IReadOnlyCollection<Identity> @objects)
    //{
    //    var resourceCollection = new ResourceCollection();
    //    foreach (var @object in @objects)
    //    {
    //        var getOnePersonRoute = _actionDescriptorCollectionProvider
    //        .ActionDescriptors
    //        .Items
    //        .First(x => x.DisplayName!.Contains(nameof(PersonController))
    //            && x.AttributeRouteInfo!.Name == "GetOnePerson");

    //        var template = getOnePersonRoute.AttributeRouteInfo!.Template!;
    //        var links = new LinkCollection();
    //        var self = GetLink("self", hrefs: template.Replace("{id}", @object.Id.ToString()));
    //        links.Add(self);

    //        var resource = new Resource()
    //        {
    //            State = @object,
    //            Links = links
    //        };
    //        resourceCollection.Add(resource);
    //    }

    //    return resourceCollection;
    //}

    private static string GetPaginationQueryString(int pageIndex, int pageSize)
    {
        return $"pageIndex={pageIndex}&pageSize={pageSize}";
    }

    private Resource GetResourceForOne<T>(T @object, IEnumerable<LinkInfo<T>> linkInfos) where T : class, IIdentity
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

    private LinkCollection GetLinksForOne<T>(T @object, IEnumerable<LinkInfo<T>> linkInfos) where T : class, IIdentity
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
        IEnumerable<LinkInfo<T>> linkInfos) where T : class, IIdentity
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

    private LinkCollection GetLinksForMany<T>(Paginated<T> @object, LinkInfo<T> linkInfo) where T : class, IIdentity
    {
        var collection = new LinkCollection();

        var getManyRoute = _actionDescriptorCollectionProvider
               .ActionDescriptors
               .Items
               .First(x => x.DisplayName!.Contains(linkInfo.ControllerName)
                   && x.AttributeRouteInfo!.Name == linkInfo.RouteName);

        var template = $"/{getManyRoute.AttributeRouteInfo!.Template!}";
        var queryString = linkInfo.QueryString!;

        var href = QueryHelpers.AddQueryString(template, queryString);
        var selfLink = GetLink("self", href);
        collection.Add(selfLink);

        var findlink = GetLink("find", $"{template}/{{?id}}", true);
        collection.Add(findlink);

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

    private EmbeddedResourceCollection GetEmbeddedLinksForMany<T>(
        IReadOnlyCollection<T> objects,
        string embeddedName,
        IEnumerable<LinkInfo<T>> linkInfos) where T : class, IIdentity
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

    private string GetHRefForOne<T>(T @object, LinkInfo<T> linkInfo) where T : class, IIdentity
    {
        var getOneRoute = _actionDescriptorCollectionProvider
               .ActionDescriptors
               .Items
               .First(x => x.DisplayName!.Contains(linkInfo.ControllerName)
                   && x.AttributeRouteInfo!.Name == linkInfo.RouteName);

        var template = getOneRoute.AttributeRouteInfo!.Template!;
        var href = "/" + template.Replace("{id}", linkInfo.GetId(@object)!.ToString());
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
