using BruTile.Predefined;
using BruTile.Web;

using Mapsui;
using Mapsui.Projections;
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

        // Источник тайлов
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

        // Слой карты
        var tileLayer = new TileLayer(tileSource);

        MapView.Map.Layers.Add(tileLayer);

        // -----------------------------
        // Стартовая позиция: Москва
        // -----------------------------

        double longitude = 37.6173;
        double latitude = 55.7558;

		var projected = SphericalMercator.FromLonLat(longitude, latitude);

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