using Tcc2.Api.Services.Models;
using Tcc2.Domain.Entities;
using Tcc2.Domain.Models.Pagination;

namespace Tcc2.Api.Interfaces.Services;

public interface IHateoasGeneratorService
{
    string GenerateForGetOne<T>(T @object, IEnumerable<LinkInfo<T>> linkInfos) where T : class, IEntity;
    string GenerateForGetMany<T>(
        Paginated<T> @object,
        LinkInfo<T> self,
        string embeddedName,
        IEnumerable<LinkInfo<T>> relatedItems) where T : class, IEntity;
    string GenerateForGetMany<T>(
        IReadOnlyCollection<T> @object,
        LinkInfo<T> self,
        string embeddedName,
        IEnumerable<LinkInfo<T>> relatedItems) where T : class, IEntity;
}
