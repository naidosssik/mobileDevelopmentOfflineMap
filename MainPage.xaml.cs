using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;

namespace OfflineMapApp;

public partial class MainPage : ContentPage
{
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
}