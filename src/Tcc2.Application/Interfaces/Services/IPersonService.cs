using Tcc2.Application.Models.Inputs;
using Tcc2.Application.Models.Outputs;
using Tcc2.Domain.Pagination;

namespace Tcc2.Application.Interfaces.Services;

public interface IPersonService
{
    Task<PersonCompleteOutput> AddAsync(PersonInput person, CancellationToken cancellationToken);
    Task<Paginated<PersonSimpleOutput>> GetAsync(int pageIndex, int pageSize, CancellationToken cancellationToken);
    Task<PersonCompleteOutput> GetAsync(long id, CancellationToken cancellationToken);
    Task<AddressCompleteOutput> GetAddressAsync(long id, CancellationToken cancellationToken);
    Task<Paginated<PersonSimpleOutput>> GetNearbyPeopleAsync(
        long id,
        double radiusInKilometers,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken);
}
