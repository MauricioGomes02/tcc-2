using Tcc2.Domain.Entities;
using Tcc2.Domain.Entities.ValueObjects;
using Tcc2.Domain.Models.Pagination;

namespace Tcc2.Domain.Interfaces.Services;
public interface IPersonService
{
    Task<Person> AddAsync(Person person, CancellationToken cancellationToken);
    Task<Paginated<Person>> GetAsync(int pageIndex, int pageSize, CancellationToken cancellationToken);
    Task<Person> GetAsync(long id, CancellationToken cancellationToken);
    Task<CompositeAddress> GetAddressAsync(long id, CancellationToken cancellationToken);
    Task<Activity> AddActivityAsync(long id, Activity activity, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Activity>> GetActivitiesAsync(long id, CancellationToken cancellationToken);
    Task<Activity> GetActivityAsync(long id, long activityId, CancellationToken cancellationToken);
    Task<Paginated<Person>> GetGeographicallyNearbyPeopleAsync(
        long id,
        double radius,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken);
    Task<Paginated<Person>> GetFunctionallyNearbyPeopleAsync(
        long id,
        long activityId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken);
}
