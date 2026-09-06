using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using OfflineMapApp.Models;

namespace OfflineMapApp;

public partial class MainPage : ContentPage
{
    private readonly List<MapPoint> _points = new();

    private readonly MemoryLayer _pointsLayer = new()
    {
        Name = "User Points",
        Style = null
    };

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
        MapView.Map.Layers.Add(_pointsLayer);

        // координаты Москвы, чтобы по умолчанию карта открывалась в этом городе
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
        var addPointPage = new AddPointPage();

        addPointPage.BindingContext = new Action<MapPoint>(point =>
        {
            AddPointToMap(point);
        });

        await    Navigation.PushModalAsync(addPointPage);
    }

    private void AddPointToMap(MapPoint point)
    {
        _points.Add(point);

        var projected = SphericalMercator.FromLonLat(
            point.Longitude,
            point.Latitude
        );

        var mapPosition = new MPoint(
            projected.x,
            projected.y
        );

        var feature = new PointFeature(mapPosition);

        feature["Id"] = point.Id.ToString();
        feature["Name"] = point.Name;
        feature["Description"] = point.Description;

        Mapsui.Styles.Color markerColor = point.MarkerColor switch
        {
            "Blue" => Mapsui.Styles.Color.Blue,
            "Green" => Mapsui.Styles.Color.Green,
            "Purple" => Mapsui.Styles.Color.Purple,
            "Red" => Mapsui.Styles.Color.Red,

            _ => Mapsui.Styles.Color.Red
        };

        feature.Styles.Add(
            new VectorStyle
            {
                Fill = new Mapsui.Styles.Brush(markerColor),

                Outline = new Mapsui.Styles.Pen(
                    Mapsui.Styles.Color.White,
                    4
                )
            }
        );

        _pointsLayer.Features = _pointsLayer.Features
            .Append(feature)
            .ToList();

        _pointsLayer.DataHasChanged();

        MapView.Map.Navigator.CenterOnAndZoomTo(
            mapPosition,
            MapView.Map.Navigator.Resolutions[12],
            500
        );
    }
}