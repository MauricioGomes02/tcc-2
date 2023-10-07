using Tcc2.Domain.Entities;
using Tcc2.Domain.Entities.ValueObjects;
using Tcc2.Domain.Pagination;

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
    Task<Paginated<Person>> GetNearbyPeopleAsync(
        long id,
        double radius,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken);
}
