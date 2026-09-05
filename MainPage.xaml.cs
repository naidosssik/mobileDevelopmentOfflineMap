using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using OfflineMapApp.Models;

namespace OfflineMapApp;

public partial class MainPage : ContentPage
{
    private readonly List<MapPoint> _points = new();
    private const string YandexApiKey = "2cffb843-ca1a-430e-b39b-07f1d5b570a1";

    public MainPage()
    {
        InitializeComponent();
        InitializeMap();
    }

    private void InitializeMap()
    {
        MapView.Map = new Mapsui.Map();

        var tileUrl =
            "https://tiles.api-maps.yandex.ru/v1/tiles/" +
            "?x={x}" +
            "&y={y}" +
            "&z={z}" +
            "&lang=en_RU" +
            "&l=map" +
            "&projection=web_mercator" +
            $"&apikey={YandexApiKey}";

        var tileSource = new HttpTileSource(
            new GlobalSphericalMercator(),
            tileUrl,
            name: "Yandex Maps"
        );

        var tileLayer = new TileLayer(tileSource);

        MapView.Map.Layers.Add(tileLayer);

        // Москва
        double longitude = 37.6173;
        double latitude = 55.7558;

        var projected =
            SphericalMercator.FromLonLat(longitude, latitude);

        var moscow = new MPoint(
            projected.x,
            projected.y
        );

        MapView.Map.Navigator.CenterOnAndZoomTo(
            moscow,
            MapView.Map.Navigator.Resolutions[10]
        );
    }

    private void ZoomIn_Clicked(object? sender, EventArgs e)
    {
        MapView.Map.Navigator.ZoomIn();
    }

    private void ZoomOut_Clicked(object? sender, EventArgs e)
    {
        MapView.Map.Navigator.ZoomOut();
    }
    private async void AddPoint_Clicked(object? sender, EventArgs e)
    {
        string? name = await DisplayPromptAsync(
            "Добавить точку",
            "Введите название точки:",
            placeholder: "Например: МГУ"
        );

        if (string.IsNullOrWhiteSpace(name))
            return;

        string? latitudeText = await DisplayPromptAsync(
            "Координаты",
            "Введите широту:",
            placeholder: "55.7558",
            keyboard: Keyboard.Numeric
        );

        if (string.IsNullOrWhiteSpace(latitudeText))
            return;

        string? longitudeText = await DisplayPromptAsync(
            "Координаты",
            "Введите долготу:",
            placeholder: "37.6173",
            keyboard: Keyboard.Numeric
        );

        if (string.IsNullOrWhiteSpace(longitudeText))
            return;

        string? description = await DisplayPromptAsync(
            "Описание",
            "Введите описание точки:",
            placeholder: "Можно оставить пустым"
        );

        if (!double.TryParse(
            latitudeText.Replace(',', '.'),
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out double latitude))
        {
            await DisplayAlertAsync(
                "Ошибка",
                "Широта введена неверно.",
                "OK"
            );

            return;
        }

        if (!double.TryParse(
            longitudeText.Replace(',', '.'),
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out double longitude))
        {
            await DisplayAlertAsync(
                "Ошибка",
                "Долгота введена неверно.",
                "OK"
            );

            return;
        }

        if (latitude < -90 || latitude > 90)
        {
            await DisplayAlertAsync(
                "Ошибка",
                "Широта должна быть от -90 до 90.",
                "OK"
            );

            return;
        }

        if (longitude < -180 || longitude > 180)
        {
            await DisplayAlertAsync(
                "Ошибка",
                "Долгота должна быть от -180 до 180.",
                "OK"
            );

            return;
        }

        var point = new MapPoint
        {
            Name = name,
            Description = description ?? string.Empty,
            Latitude = latitude,
            Longitude = longitude,
            Source = "Manual"
        };

        _points.Add(point);

        await DisplayAlertAsync(
            "Готово",
            $"Точка «{point.Name}» добавлена.",
            "OK"
        );
    }
}