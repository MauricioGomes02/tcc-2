using Tcc2.Application.Models.Inputs;
using Tcc2.Application.Models.Outputs;
using Tcc2.Domain.Models.Pagination;

namespace Tcc2.Application.Interfaces.Services;

public interface IPersonService
{
    Task<PersonCompleteOutput> AddAsync(PersonInput person, CancellationToken cancellationToken);
    Task<Paginated<PersonSimpleOutput>> GetAsync(int pageIndex, int pageSize, CancellationToken cancellationToken);
    Task<PersonCompleteOutput> GetAsync(long id, CancellationToken cancellationToken);
    Task<AddressCompleteOutput> GetAddressAsync(long id, CancellationToken cancellationToken);
    Task<ActivityCompleteOutput> AddActivityAsync(
        long id,
        ActivityInput? activity,
        CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ActivitySimpleOutput>> GetActivitiesAsync(long id, CancellationToken cancellationToken);
    Task<ActivityCompleteOutput> GetActivityAsync(long id, long activityId, CancellationToken cancellationToken);
    Task<Paginated<PersonSimpleOutput>> GetGeographicallyNearbyPeopleAsync(
        long id,
        double radius,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken);
    Task<Paginated<PersonSimpleOutput>> GetFunctionallyNearbyPeopleAsync(
        long id, 
        long activityId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken);
}
