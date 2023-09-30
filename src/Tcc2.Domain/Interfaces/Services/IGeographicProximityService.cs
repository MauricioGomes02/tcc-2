using Tcc2.Domain.Entities;
using Tcc2.Domain.Entities.ValueObjects;
using Tcc2.Domain.Pagination;

namespace Tcc2.Domain.Interfaces.Services;
public interface IGeographicProximityService
{
    public Task<Paginated<Person>> GetAsync(
        Address originAddress,
        double radiusInKilometers,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken);
}
