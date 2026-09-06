using System.Text.Json;
using OfflineMapApp.Models;

namespace OfflineMapApp.Services;

public static class PointStorageService
{
    private const string FileName = "points.json";

    private static string FilePath =>
        Path.Combine(
            FileSystem.AppDataDirectory,
            FileName
        );

    public static async Task SavePointsAsync(
        List<MapPoint> points)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json =
            JsonSerializer.Serialize(
                points,
                options
            );

        await File.WriteAllTextAsync(
            FilePath,
            json
        );
    }

    public static async Task<List<MapPoint>>
        LoadPointsAsync()
    {
        if (!File.Exists(FilePath))
        {
            return new List<MapPoint>();
        }

        string json =
            await File.ReadAllTextAsync(
                FilePath
            );

        var points =
            JsonSerializer.Deserialize<List<MapPoint>>(
                json
            );

        return points
            ?? new List<MapPoint>();
    }
}