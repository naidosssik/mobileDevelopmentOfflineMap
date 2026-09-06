using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace OfflineMapApp.Services;

public static class PhotoMetadataService
{
    public static (double Latitude, double Longitude)?
        GetGpsCoordinates(string filePath)
    {
        var directories =
            ImageMetadataReader.ReadMetadata(filePath);

        var gpsDirectory =
            directories
                .OfType<GpsDirectory>()
                .FirstOrDefault();

        if (gpsDirectory == null)
        {
            return null;
        }

        var geoLocation =
            gpsDirectory.GetGeoLocation();

        if (geoLocation == null)
        {
            return null;
        }

        double latitude =
            geoLocation.Latitude;

        double longitude =
            geoLocation.Longitude;

        return (
            latitude,
            longitude
        );
    }
}