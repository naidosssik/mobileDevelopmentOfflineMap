using System.Text;
using System.Text.Json;

using OfflineMapApp.Models;

namespace OfflineMapApp.Services;

public static class RouteService
{
    private static readonly HttpClient HttpClient = new();

    public static async Task<OsrmRoute?> GetRouteAsync(
        double startLatitude,
        double startLongitude,
        double endLatitude,
        double endLongitude)
    {
        string url =
            "https://valhalla1.openstreetmap.de/route";

        var requestBody = new
        {
            locations = new[]
            {
                new
                {
                    lat = startLatitude,
                    lon = startLongitude
                },

                new
                {
                    lat = endLatitude,
                    lon = endLongitude
                }
            },

            costing = "auto",

            units = "kilometers"
        };

        string jsonBody =
            JsonSerializer.Serialize(requestBody);

        using var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                url
            );

        request.Content =
            new StringContent(
                jsonBody,
                Encoding.UTF8,
                "application/json"
            );

        // Для публичного demo-сервера рекомендуется
        // указывать идентификатор клиента
        request.Headers.TryAddWithoutValidation(
            "X-Client-Id",
            "OfflineMapApp-student-project"
        );

        using var response =
            await HttpClient.SendAsync(request);

        string responseJson =
            await response.Content
                .ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Ошибка Valhalla: " +
                $"{(int)response.StatusCode} " +
                $"{response.ReasonPhrase}\n\n" +
                responseJson
            );
        }

        return ParseRoute(responseJson);
    }


    private static OsrmRoute? ParseRoute(
        string json)
    {
        using var document =
            JsonDocument.Parse(json);

        JsonElement root =
            document.RootElement;

        if (!root.TryGetProperty(
            "trip",
            out JsonElement trip))
        {
            return null;
        }


        // =====================================================
        // DISTANCE + DURATION
        // =====================================================

        double distanceMeters = 0;
        double durationSeconds = 0;


        if (trip.TryGetProperty(
            "summary",
            out JsonElement summary))
        {
            // Valhalla возвращает length в километрах,
            // потому что мы запросили units = kilometers
            if (summary.TryGetProperty(
                "length",
                out JsonElement lengthElement))
            {
                double distanceKm =
                    lengthElement.GetDouble();

                distanceMeters =
                    distanceKm * 1000.0;
            }


            if (summary.TryGetProperty(
                "time",
                out JsonElement timeElement))
            {
                durationSeconds =
                    timeElement.GetDouble();
            }
        }


        // =====================================================
        // ROUTE GEOMETRY
        // =====================================================

        if (!trip.TryGetProperty(
            "legs",
            out JsonElement legs))
        {
            return null;
        }

        var allCoordinates =
            new List<List<double>>();


        foreach (JsonElement leg in legs.EnumerateArray())
        {
            if (!leg.TryGetProperty(
                "shape",
                out JsonElement shapeElement))
            {
                continue;
            }

            string? encodedShape =
                shapeElement.GetString();

            if (string.IsNullOrWhiteSpace(
                encodedShape))
            {
                continue;
            }


            var decoded =
                DecodePolyline6(encodedShape);


            foreach (var coordinate in decoded)
            {
                // В твоём DrawRoute ожидается:
                // [longitude, latitude]

                allCoordinates.Add(
                    new List<double>
                    {
                        coordinate.Longitude,
                        coordinate.Latitude
                    }
                );
            }
        }


        if (allCoordinates.Count < 2)
        {
            return null;
        }


        return new OsrmRoute
        {
            Distance = distanceMeters,

            Duration = durationSeconds,

            Geometry =
                new OsrmGeometry
                {
                    Coordinates =
                        allCoordinates
                }
        };
    }


    // =========================================================
    // POLYLINE6 DECODER
    // =========================================================

    private static List<RouteCoordinate>
        DecodePolyline6(
            string encoded)
    {
        var coordinates =
            new List<RouteCoordinate>();

        int index = 0;

        long latitude = 0;
        long longitude = 0;


        while (index < encoded.Length)
        {
            latitude +=
                DecodeNextValue(
                    encoded,
                    ref index
                );

            longitude +=
                DecodeNextValue(
                    encoded,
                    ref index
                );


            double decodedLatitude =
                latitude / 1_000_000.0;

            double decodedLongitude =
                longitude / 1_000_000.0;


            coordinates.Add(
                new RouteCoordinate
                {
                    Latitude =
                        decodedLatitude,

                    Longitude =
                        decodedLongitude
                }
            );
        }


        return coordinates;
    }


    private static long DecodeNextValue(
        string encoded,
        ref int index)
    {
        long result = 0;

        int shift = 0;

        int value;


        do
        {
            value =
                encoded[index++] - 63;

            result |=
                (long)(value & 0x1F)
                << shift;

            shift += 5;

        } while (value >= 0x20);


        return (result & 1) != 0
            ? ~(result >> 1)
            : result >> 1;
    }


    private class RouteCoordinate
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}