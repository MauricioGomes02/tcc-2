namespace Tcc2.Domain.Entities.ValueObjects;

public class GeographicCoordinate
{
    public GeographicCoordinate(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
}
