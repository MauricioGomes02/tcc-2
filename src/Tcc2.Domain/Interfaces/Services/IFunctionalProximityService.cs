using Tcc2.Domain.Entities;
using Tcc2.Domain.Pagination;

namespace Tcc2.Domain.Interfaces.Services;

public interface IFunctionalProximityService
{
    Task<Paginated<Person>> GetAsync(
        Activity activity,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken);
}
