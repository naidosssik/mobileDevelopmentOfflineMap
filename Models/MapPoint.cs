namespace OfflineMapApp.Models;

public class MapPoint
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string CoordinatesText => $"{Latitude:F5}, {Longitude:F5}";

    public string Source { get; set; } = "Manual";

    public string? PhotoPath { get; set; }

    // Цвет метки
    public string MarkerColor { get; set; } = "Red";

    // Форма: Circle или Pin
    public string MarkerShape { get; set; } = "Circle";

    // Содержимое: Empty, Number или Icon
    public string MarkerContent { get; set; } = "Empty";

    // Используется только при MarkerContent = Icon
    public string MarkerIcon { get; set; } = "Heart";

    // Используется только при MarkerContent = Number
    public int? MarkerNumber { get; set; }
}