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
        MapView.Map = new Mapsui.Map();

        var tileSource = new HttpTileSource(
            new GlobalSphericalMercator(),
            "https://tile.openstreetmap.org/{z}/{x}/{y}.png",
            name: "OpenStreetMap",
            configureHttpRequestMessage: request =>
            {
                request.Headers.TryAddWithoutValidation(
                    "User-Agent",
                    "OfflineMapApp/1.0"
                );
            }
        );

        var tileLayer = new TileLayer(tileSource);

        MapView.Map.Layers.Add(tileLayer);
    }
}