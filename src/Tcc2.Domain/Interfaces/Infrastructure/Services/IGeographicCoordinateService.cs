using Tcc2.Domain.Entities.ValueObjects;

namespace Tcc2.Domain.Interfaces.Infrastructure.Services;

public interface IGeographicCoordinateService
{
    Task<GeographicCoordinate?> GetAsync(Address address, CancellationToken cancellationToken);
}
