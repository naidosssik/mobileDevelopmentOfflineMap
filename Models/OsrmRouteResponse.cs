using System.Text.Json.Serialization;

namespace OfflineMapApp.Models;

public class OsrmRouteResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("routes")]
    public List<OsrmRoute> Routes { get; set; } = new();
}

public class OsrmRoute
{
    [JsonPropertyName("distance")]
    public double Distance { get; set; }

    [JsonPropertyName("duration")]
    public double Duration { get; set; }

    [JsonPropertyName("geometry")]
    public OsrmGeometry Geometry { get; set; } = new();
}

public class OsrmGeometry
{
    [JsonPropertyName("coordinates")]
    public List<List<double>> Coordinates { get; set; } = new();
}