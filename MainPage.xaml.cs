using BruTile.Predefined;
using BruTile.Web;
using Mapsui.Tiling.Layers;

namespace OfflineMapApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        InitializeMap();
    }

    private void InitializeMap()
    {
        // Создаём карту
        MapView.Map = new Mapsui.Map();

        // Создаём источник тайлов
        var tileSource = new HttpTileSource(
            new GlobalSphericalMercator(),
            "https://{s}.tile.openstreetmap.fr/osmfr/{z}/{x}/{y}.png",
            serverNodes: new[] { "a", "b", "c" },
            name: "OpenStreetMap France",
            configureHttpRequestMessage: request =>
            {
                request.Headers.TryAddWithoutValidation(
                    "User-Agent",
                    "OfflineMapApp/1.0 student-project"
                );
            }
        );

        // Создаём слой карты
        var tileLayer = new TileLayer(tileSource);

        // Добавляем слой на карту
        MapView.Map.Layers.Add(tileLayer);
    }
}