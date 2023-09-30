using Tcc2.Domain.Entities;
using Tcc2.Domain.Pagination;

namespace Tcc2.Domain.Interfaces.Services;
public interface IPersonService
{
    Task<Person> AddAsync(Person person, CancellationToken cancellationToken);
    Task<Paginated<Person>> GetAsync(int pageIndex, int pageSize, CancellationToken cancellationToken);
    Task<Person> GetAsync(long id, CancellationToken cancellationToken);
    Task<Paginated<Person>> GetNearbyPeopleAsync(
        long id,
        double radiusInKilometers,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken);
}
