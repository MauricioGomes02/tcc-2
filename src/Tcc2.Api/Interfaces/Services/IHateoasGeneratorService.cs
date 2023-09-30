using Tcc2.Api.Services.Models;
using Tcc2.Application.Models;
using Tcc2.Domain.Pagination;

namespace Tcc2.Api.Interfaces.Services;

public interface IHateoasGeneratorService
{
    string GenerateForGetOne<T>(T @object, IEnumerable<LinkInfo<T>> linkInfos) where T : class, IIdentity;
    string GenerateForGetMany<T>(
        Paginated<T> @object,
        LinkInfo<T> self,
        string embeddedName,
        IEnumerable<LinkInfo<T>> relatedItems) where T : class, IIdentity;
}
