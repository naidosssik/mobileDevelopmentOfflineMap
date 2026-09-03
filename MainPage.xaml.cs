using Mapsui.Tiling;

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

        MapView.Map.Layers.Add(
            OpenStreetMap.CreateTileLayer()
        );
    }
}