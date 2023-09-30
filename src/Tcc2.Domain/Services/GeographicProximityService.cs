using Tcc2.Domain.Entities;
using Tcc2.Domain.Entities.ValueObjects;
using Tcc2.Domain.Interfaces.Infrastructure.Repositories;
using Tcc2.Domain.Interfaces.Services;
using Tcc2.Domain.Pagination;

namespace Tcc2.Domain.Services;
public class GeographicProximityService : IGeographicProximityService
{
    private const double RadiusOfTheEarth = 6371; // approximately
    private readonly IPersonRepository _personRepository;

    public GeographicProximityService(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public Task<Paginated<Person>> GetAsync(
        Address originAddress,
        double radiusInKilometers,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var originLatitude = originAddress.GeographicCoordinate!.Latitude;
        var originLongitude = originAddress.GeographicCoordinate!.Longitude;

        // Haversine Formula
        return _personRepository.GetAsync(x => RadiusOfTheEarth * (2 * Math.Asin(
            Math.Sqrt(
                Math.Pow(
                    Math.Sin(Math.PI / 180 * (x.Address.GeographicCoordinate!.Latitude - originLatitude) / 2),
                    2) +
                Math.Pow(
                    Math.Sin(Math.PI / 180 * (x.Address.GeographicCoordinate!.Longitude - originLongitude) / 2),
                    2) *
                Math.Cos(Math.PI / 180 * originLatitude) *
                Math.Cos(Math.PI / 180 * x.Address.GeographicCoordinate!.Latitude)
            )))
            <= radiusInKilometers && x.Id != originAddress.PersonId,
            pageIndex,
            pageSize,
            cancellationToken);
    }
}
