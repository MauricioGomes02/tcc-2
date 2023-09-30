using Newtonsoft.Json;

namespace Tcc2.Infrastructure.Models.Outputs;

public class GeographicCoordinateOutput
{
    [JsonProperty("lat")]
    public double Latitude { get; set; }
    [JsonProperty("lon")]
    public double Longitude { get; set; }
}
