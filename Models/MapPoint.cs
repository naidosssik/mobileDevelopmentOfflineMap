namespace OfflineMapApp.Models;

public class MapPoint
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Source { get; set; } = "Manual";

    public string? PhotoPath { get; set; }


    // Оформление точки

    public string MarkerColor { get; set; } = "Red";

    public string MarkerType { get; set; } = "Icon";

    public string MarkerIcon { get; set; } = "Heart";

    public int? MarkerNumber { get; set; }
}