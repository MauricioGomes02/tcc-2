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
        var days = activity.ActivityDay.Select(x => x.Day.Id);

        var criteria = new Criteria<Person, Person>(
            storedPerson => storedPerson.Activities != null && storedPerson.Id != activity.PersonId
                && storedPerson.Activities.Any(
                    // Day
                    storedActivity => storedActivity.ActivityDay.Any(
                        activityDay => days.Contains(activityDay.Day.Id))
                    // Address
                    && (storedActivity.Address.PostalCode == activity.Address.PostalCode 
                        && storedActivity.Address.Number == activity.Address.Number)
                    // Schedules
                    && ((storedActivity.Start >= activity.Start && storedActivity.Start < activity.End)
                        || (storedActivity.Start <= activity.Start && storedActivity.End > activity.End)
                    )),
            x => x,
            pageIndex,
            pageSize);

        return _personRepository.GetPaginatedAsync(criteria, cancellationToken);
    }
}
