using System.Runtime.CompilerServices;
using Tcc2.Domain.Entities;
using Tcc2.Domain.Entities.ValueObjects;
using Tcc2.Domain.Exceptions;
using Tcc2.Domain.Interfaces.Infrastructure.Repositories;
using Tcc2.Domain.Interfaces.Infrastructure.Services;
using Tcc2.Domain.Interfaces.Services;
using Tcc2.Domain.Models.Pagination;
using Tcc2.Domain.Models.Validation;

namespace Tcc2.Domain.Services;

public class PersonService : IPersonService
{
    private readonly IGeographicCoordinateService _geographicCoordinateService;
    private readonly IGeographicProximityService _geographicProximityService;
    private readonly IFunctionalProximityService _functionProximityService;
    private readonly IPersonRepository _repository;

    public PersonService(
        IGeographicCoordinateService geographicCoordinateService,
        IGeographicProximityService geographicProximityService,
        IFunctionalProximityService functionProximityService,
        IPersonRepository repository)
    {
        _geographicCoordinateService = geographicCoordinateService;
        _geographicProximityService = geographicProximityService;
        _functionProximityService = functionProximityService;
        _repository = repository;
    }

    public async Task<Person> AddAsync(Person person, CancellationToken cancellationToken)
    {
        person.Validate();

        var geographicCoordinate = await _geographicCoordinateService
            .GetAsync(person.Address, cancellationToken)
            .ConfigureAwait(false);

        if (geographicCoordinate is null)
        {
            throw new ValueObjectNotFoundException(
                "Unable to obtain the geographic coordinates of the address provided");
        }

        person.Address.AddGeographicCoordinate(geographicCoordinate);

        var soredPerson = await _repository.AddAsync(person, cancellationToken).ConfigureAwait(false);
        await _repository.SaveAsync(cancellationToken).ConfigureAwait(false);
        return soredPerson;
    }

    public Task<Paginated<Person>> GetAsync(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        var validationResult = new ValidationResult();
        var modelName = nameof(PersonService);

        if (pageIndex < 0)
        {
            validationResult.Add(
                nameof(pageIndex),
                modelName,
                "Cannot be less than zero");
        }

        if (pageSize <= 0)
        {
            validationResult.Add(
                nameof(pageSize),
                modelName,
                "Cannot be less than or equal to zero");
        }

        if (!validationResult.IsValid)
        {
            throw new ValidationException("Invalid input", validationResult);
        }

        var criteria = new Criteria<Person, Person>(
            x => true,
            x => x,
            pageIndex,
            pageSize);

        return _repository.GetPaginatedAsync(criteria, cancellationToken);
    }

    public async Task<Person> GetAsync(long id, CancellationToken cancellationToken)
    {
        var validationResult = new ValidationResult();
        validationResult.AddRange(GetIdInvalidFields(id));

        if (!validationResult.IsValid)
        {
            throw new ValidationException("Invalid input", validationResult);
        }

        var criteria = new Criteria<Person, Person>(x => x.Id == id, x => x);

        var person = (await _repository
            .GetAsync(criteria, cancellationToken)
            .ConfigureAwait(false)).SingleOrDefault();

        if (person is null)
        {
            throw new EntityNotFoundException($"No person entity found for id {id}");
        }

        return person;
    }

    public async Task<CompositeAddress> GetAddressAsync(long id, CancellationToken cancellationToken)
    {
        var validationResult = new ValidationResult();
        validationResult.AddRange(GetIdInvalidFields(id));

        if (!validationResult.IsValid)
        {
            throw new ValidationException("Invalid input", validationResult);
        }

        var criteria = new Criteria<Person, CompositeAddress>(
            x => x.Id == id,
            x => x.Address);

        var address = (await _repository
            .GetAsync(criteria, cancellationToken)
            .ConfigureAwait(false)).SingleOrDefault();

        if (address is null)
        {
            throw new EntityNotFoundException($"No person entity found for id {id}");
        }

        return address;
    }

    public async Task<Activity> AddActivityAsync(long id, Activity activity, CancellationToken cancellationToken)
    {
        activity.Validate();

        var storedDomainPerson = await GetAsync(id, cancellationToken).ConfigureAwait(false);
        storedDomainPerson.Activities.Add(activity);
        _ = await _repository
            .UpdateAsync(storedDomainPerson, cancellationToken)
            .ConfigureAwait(false);

        await _repository.SaveAsync(cancellationToken).ConfigureAwait(false);
        return activity;
    }

    public async Task<IReadOnlyCollection<Activity>> GetActivitiesAsync(long id, CancellationToken cancellationToken)
    {
        var person = await GetAsync(id, cancellationToken).ConfigureAwait(false);
        return person.Activities.ToList();
    }

    public async Task<Activity> GetActivityAsync(long id, long activityId, CancellationToken cancellationToken)
    {
        var validationResult = new ValidationResult();
        validationResult.AddRange(GetIdInvalidFields(id));
        validationResult.AddRange(GetIdInvalidFields(activityId));

        if (!validationResult.IsValid)
        {
            throw new ValidationException("Invalid input", validationResult);
        }

        var person = await GetAsync(id, cancellationToken).ConfigureAwait(false);
        var activities = person.Activities;
        var storedActivity = activities.SingleOrDefault(x => x.Id == activityId);

        if (storedActivity is null)
        {
            throw new EntityNotFoundException($"No activity entity found for id {activityId} and person id {id}");
        }

        return storedActivity;
    }

    public async Task<Paginated<Person>> GetGeographicallyNearbyPeopleAsync(
        long id,
        double radius,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var validationResult = new ValidationResult();
        validationResult.AddRange(GetIdInvalidFields(id));
        validationResult.AddRange(GetPaginationInvalidFields(pageIndex, pageSize));

        if (radius <= 0)
        {
            validationResult.Add(
                nameof(radius),
                nameof(PersonService),
                "Cannot be less than or equal to zero");
        }

        if (!validationResult.IsValid)
        {
            throw new ValidationException("Invalid input", validationResult);
        }

        var address = await GetAddressAsync(id, cancellationToken).ConfigureAwait(false);

        var nearbyPeople = await _geographicProximityService
            .GetAsync(address, radius, pageIndex, pageSize, cancellationToken)
            .ConfigureAwait(false);

        return nearbyPeople;
    }

    public async Task<Paginated<Person>> GetFunctionallyNearbyPeopleAsync(
        long id,
        long activityId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var activity = await GetActivityAsync(id, activityId, cancellationToken).ConfigureAwait(false);
        var nearbyPeople = await _functionProximityService
            .GetAsync(activity, pageIndex, pageSize, cancellationToken)
            .ConfigureAwait(false);

        return nearbyPeople;
    }

    private static IEnumerable<InvalidField> GetIdInvalidFields(
        long id,
        [CallerArgumentExpression(nameof(id))] string parameterName = "")
    {
        var validationResult = new ValidationResult();
        var modelName = nameof(PersonService);

        if (id <= 0)
        {
            validationResult.Add(
                parameterName,
                modelName,
                "Cannot be less than or equal to zero");
        }

        return validationResult.InvalidFields;
    }

    private static IEnumerable<InvalidField> GetPaginationInvalidFields(int pageIndex, int pageSize)
    {
        var validationResult = new ValidationResult();
        var modelName = nameof(PersonService);

        if (pageIndex < 0)
        {
            validationResult.Add(
                nameof(pageIndex),
                modelName,
                "Cannot be less than zero");
        }

        if (pageSize <= 0)
        {
            validationResult.Add(
                nameof(pageSize),
                modelName,
                "Cannot be less than or equal to zero");
        }

        return validationResult.InvalidFields;
    }
}
