using Tcc2.Domain.Entities;
using Tcc2.Domain.Interfaces.Infrastructure.Repositories;
using Tcc2.Domain.Interfaces.Services;
using Tcc2.Domain.Pagination;

namespace Tcc2.Domain.Services;

public class FunctionalProximityService : IFunctionalProximityService
{
    private readonly IPersonRepository _personRepository;

    public FunctionalProximityService(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public Task<Paginated<Person>> GetAsync(
        Activity activity,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var criteria = new Criteria<Person, Person>(
            x => x.Activities != null
                && x.Activities.Any(y => y.Date == activity.Date),
            x => x,
            pageIndex,
            pageSize);

        return _personRepository.GetAsync(criteria, cancellationToken);
    }
}
