using System.Linq;

using BruTile.Predefined;
using BruTile.Web;

using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;

using OfflineMapApp.Models;

namespace OfflineMapApp;

public partial class MainPage : ContentPage
{
    // Вставь сюда свой действующий ключ Yandex Tiles API
    private const string YandexApiKey = "2cffb843-ca1a-430e-b39b-07f1d5b570a1";


    // Список всех пользовательских точек
    private readonly List<MapPoint> _points = new();


    // Отдельный слой Mapsui,
    // на котором будут находиться наши точки
    private readonly MemoryLayer _pointsLayer = new()
    {
        Name = "User Points",
        Style = null
    };


    public MainPage()
    {
        InitializeComponent();

        InitializeMap();
    }

    private void InitializeMap()
    {
        MapView.Map = new Mapsui.Map();



        string tileUrl =
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


        // Сначала добавляем слой карты
        MapView.Map.Layers.Add(tileLayer);


        // Потом слой пользовательских точек, чтобы точки рисовались поверх карты
        MapView.Map.Layers.Add(_pointsLayer);


        double longitude = 37.6173;
        double latitude = 55.7558;


        var projected =
            SphericalMercator.FromLonLat(
                longitude,
                latitude
            );


        var moscow = new MPoint(
            projected.x,
            projected.y
        );


        MapView.Map.Navigator.CenterOnAndZoomTo(
            moscow,
            MapView.Map.Navigator.Resolutions[10]
        );
    }

    private void ZoomIn_Clicked(
        object? sender,
        EventArgs e)
    {
        MapView.Map.Navigator.ZoomIn();
    }


    private void ZoomOut_Clicked(
        object? sender,
        EventArgs e)
    {
        MapView.Map.Navigator.ZoomOut();
    }


    private async void AddPoint_Clicked(
        object? sender,
        EventArgs e)
    {
        var addPointPage = new AddPointPage();


        // Когда AddPointPage создаст MapPoint,
        // он будет передан сюда
        addPointPage.BindingContext =
            new Action<MapPoint>(point =>
            {
                AddPointToMap(point);
            });


        await Navigation.PushModalAsync(
            addPointPage
        );
    }

    private void AddPointToMap(MapPoint point)
    {
        // Сохраняем модель точки
        _points.Add(point);


        // GPS-координаты переводим
        // в координаты Web Mercator
        var projected =
            SphericalMercator.FromLonLat(
                point.Longitude,
                point.Latitude
            );


        var mapPosition = new MPoint(
            projected.x,
            projected.y
        );


        // Создаём объект Mapsui
        var feature =
            new PointFeature(mapPosition);


        // Сохраняем внутри feature данные точки
        feature["Id"] =
            point.Id.ToString();

        feature["Name"] =
            point.Name;

        feature["Description"] =
            point.Description;


        // выбираем стиль в зависимости от настроек пользователя
        feature.Styles.Add(
            CreateMarkerStyle(point)
        );


        // Добавляем feature в слой точек
        _pointsLayer.Features =
            _pointsLayer.Features
                .Append(feature)
                .ToList();


        // Сообщаем Mapsui, что данные слоя изменились
        _pointsLayer.DataHasChanged();


        // После добавления перемещаемся к новой точке
        MapView.Map.Navigator.CenterOnAndZoomTo(
            mapPosition,
            MapView.Map.Navigator.Resolutions[12],
            500
        );
    }

    private IStyle CreateMarkerStyle(
        MapPoint point)
    {
        return point.MarkerShape switch
        {
            "Pin" =>
                CreatePinMarkerStyle(point),

            "Circle" =>
                CreateCircleMarkerStyle(point),

            _ =>
                CreateCircleMarkerStyle(point)
        };
    }

    private string GetMarkerColorHex(
        MapPoint point)
    {
        return point.MarkerColor switch
        {
            "Blue" =>
                "#4285F4",

            "Green" =>
                "#43A047",

            "Purple" =>
                "#9C27B0",

            "Red" =>
                "#E65353",

            _ =>
                "#E65353"
        };
    }

    private string CreateMarkerContentSvg(
        MapPoint point)
    {

        if (point.MarkerContent == "Empty")
        {
            return string.Empty;
        }

        if (point.MarkerContent == "Number")
        {
            string number =
                point.MarkerNumber?.ToString()
                ?? "1";


            return $"""
            <text
                x="32"
                y="39"
                text-anchor="middle"
                font-family="Arial"
                font-size="22"
                font-weight="bold"
                fill="#555555">
                {number}
            </text>
            """;
        }

        return point.MarkerIcon switch
        {
            "Heart" =>
            """
            <path
                d="
                    M32 44
                    C19 35 14 29 14 21
                    C14 15 19 11 24 11
                    C28 11 31 13 32 16
                    C34 13 37 11 41 11
                    C46 11 51 15 51 21
                    C51 29 45 35 32 44Z
                "
                fill="#666666"
            />
            """,


            "Star" =>
            """
            <path
                d="
                    M32 13
                    L37 24
                    L49 25
                    L40 33
                    L43 45
                    L32 39
                    L21 45
                    L24 33
                    L15 25
                    L27 24Z
                "
                fill="#666666"
            />
            """,


            "Photo" =>
            """
            <rect
                x="18"
                y="21"
                width="28"
                height="22"
                rx="3"
                fill="#666666"
            />

            <circle
                cx="32"
                cy="32"
                r="6"
                fill="#FFFFFF"
            />
            """,


            "Place" =>
            """
            <path
                d="
                    M32 16
                    C24 16 18 22 18 30
                    C18 40 32 49 32 49
                    C32 49 46 40 46 30
                    C46 22 40 16 32 16Z
                "
                fill="#666666"
            />

            <circle
                cx="32"
                cy="29"
                r="5"
                fill="#FFFFFF"
            />
            """,


            _ =>
                string.Empty
        };
    }


    private IStyle CreateCircleMarkerStyle(
        MapPoint point)
    {
        string color =
            GetMarkerColorHex(point);


        string content =
            CreateMarkerContentSvg(point);


        string svg =
            $"""
            <svg
                xmlns="http://www.w3.org/2000/svg"
                width="64"
                height="64"
                viewBox="0 0 64 64">

                <!--
                    Цветная внешняя оболочка
                -->

                <circle
                    cx="32"
                    cy="32"
                    r="29"
                    fill="{color}"
                />


                <!--
                    Белая внутренняя часть
                -->

                <circle
                    cx="32"
                    cy="32"
                    r="21"
                    fill="#FFFFFF"
                />


                <!--
                    Empty / Number / Icon
                -->

                {content}

            </svg>
            """;


        return new ImageStyle
        {
            Image =
                $"svg-content://{svg}",

            SymbolScale = 0.75
        };
    }

    private IStyle CreatePinMarkerStyle(
        MapPoint point)
    {
        string color =
            GetMarkerColorHex(point);


        string content =
            CreateMarkerContentSvg(point);


        string svg =
            $"""
            <svg
                xmlns="http://www.w3.org/2000/svg"
                width="64"
                height="80"
                viewBox="0 0 64 80">

                <!--
                    Цветная внешняя форма
                -->

                <path
                    d="
                        M32 4
                        C17 4 8 14 8 29
                        C8 47 32 74 32 74
                        C32 74 56 47 56 29
                        C56 14 47 4 32 4Z
                    "
                    fill="{color}"
                />


                <!--
                    Белый центр
                -->

                <circle
                    cx="32"
                    cy="28"
                    r="17"
                    fill="#FFFFFF"
                />


                <!--
                    Содержимое немного
                    поднимаем вверх,
                    потому что центр Pin
                    находится выше
                -->

                <g transform="translate(0,-4)">
                    {content}
                </g>

            </svg>
            """;


        return new ImageStyle
        {
            Image =
                $"svg-content://{svg}",

            SymbolScale = 0.75,

            RelativeOffset =
                new RelativeOffset(
                    0,
                    0.5
                )
        };
    }
}