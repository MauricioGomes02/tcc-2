using Tcc2.Domain.Entities;
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

        return _repository.GetAsync(criteria, cancellationToken);
    }

    public async Task<Person> GetAsync(long id, CancellationToken cancellationToken)
    {
        var person = await _repository.GetAsync(id, cancellationToken).ConfigureAwait(false);

        if (person is null)
        {
            throw new EntityNotFoundException($"No person entity found for id {id}");
        }

        return person;
    }

    public async Task<Paginated<Person>> GetNearbyPeopleAsync(
        long id,
        double radius,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var person = await GetAsync(id, cancellationToken).ConfigureAwait(false);
        var address = person.Address;

        var nearbyPeople = await _geographicProximityService
            .GetAsync(address, radius, pageIndex, pageSize, cancellationToken)
            .ConfigureAwait(false);

        return nearbyPeople;
    }
}
