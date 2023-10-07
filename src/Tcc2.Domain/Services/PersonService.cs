﻿using Tcc2.Domain.Entities;
using Tcc2.Domain.Entities.ValueObjects;
using Tcc2.Domain.Exceptions;
using Tcc2.Domain.Interfaces.Infrastructure.Repositories;
using Tcc2.Domain.Interfaces.Infrastructure.Services;
using Tcc2.Domain.Interfaces.Services;
using Tcc2.Domain.Pagination;

namespace Tcc2.Domain.Services;

public class PersonService : IPersonService
{
    private readonly IGeographicCoordinateService _geographicCoordinateService;
    private readonly IGeographicProximityService _geographicProximityService;
    private readonly IPersonRepository _repository;

    public PersonService(
        IGeographicCoordinateService geographicCoordinateService,
        IGeographicProximityService geographicProximityService,
        IPersonRepository repository)
    {
        _geographicCoordinateService = geographicCoordinateService;
        _geographicProximityService = geographicProximityService;
        _repository = repository;
    }

    public async Task<Person> AddAsync(Person person, CancellationToken cancellationToken)
    {
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
        var criteria = new Criteria<Person, Person>(
            x => true,
            x => x,
            pageIndex,
            pageSize);

        return _repository.GetPaginatedAsync(criteria, cancellationToken);
    }

    public async Task<Person> GetAsync(long id, CancellationToken cancellationToken)
    {
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
        var storedDomainPerson = await GetAsync(id, cancellationToken).ConfigureAwait(false);
        var days = (await _repository
            .GetDaysAsync(cancellationToken)
            .ConfigureAwait(false)).ToDictionary(x => x.Id);

        foreach (var activityDay in activity.ActivityDay)
        {
            var day = days[activityDay.Day.Id];
            activityDay.UpdateDay(day);
        }

        storedDomainPerson.Activities.Add(activity);
        _ = await _repository
            .UpdateAsync(storedDomainPerson, cancellationToken)
            .ConfigureAwait(false);

        await _repository.SaveAsync(cancellationToken).ConfigureAwait(false);
        return activity;
    }

    public async Task<IReadOnlyCollection<Activity>> GetActivitiesAsync(long id, CancellationToken cancellationToken)
    {
        var criteria = new Criteria<Person, ICollection<Activity>>(
            x => x.Id == id,
            x => x.Activities);

        var paginatedActivity = await _repository.GetAsync(criteria, cancellationToken).ConfigureAwait(false);
        return paginatedActivity.Single().ToList();
    }

    public async Task<Paginated<Person>> GetNearbyPeopleAsync(
        long id,
        double radius,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var address = await GetAddressAsync(id, cancellationToken).ConfigureAwait(false);

        var nearbyPeople = await _geographicProximityService
            .GetAsync(address, radius, pageIndex, pageSize, cancellationToken)
            .ConfigureAwait(false);

        return nearbyPeople;
    }

    public async Task<Activity> GetActivityAsync(long id, long activityId, CancellationToken cancellationToken)
    {
        var criteria = new Criteria<Person, ICollection<Activity>>(
            x => x.Id == id && x.Activities.Any(y => y.Id == activityId),
            x => x.Activities);

        var paginatedActivity = await _repository.GetAsync(criteria, cancellationToken).ConfigureAwait(false);
        var storedActivity = paginatedActivity.Single().Single();

        if (storedActivity is null)
        {
            throw new EntityNotFoundException($"No activity entity found for id {activityId} and person id {id}");
        }

        return storedActivity;
    }
}
