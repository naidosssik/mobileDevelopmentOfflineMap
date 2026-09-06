using System.Linq;

using BruTile.Predefined;
using BruTile.Web;

using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using Microsoft.Maui.Devices.Sensors;   

using OfflineMapApp.Models;
using OfflineMapApp.Services;

namespace OfflineMapApp;

public partial class MainPage : ContentPage
{
    // Вставь сюда свой действующий ключ Yandex Tiles API
    private const string YandexApiKey = "2cffb843-ca1a-430e-b39b-07f1d5b570a1";


    // Список всех пользовательских точек
    private readonly List<MapPoint> _points = new();

    private PointFeature? _currentLocationFeature;
    private Location? _currentLocation;

    // Отдельный слой Mapsui, на котором будут находиться наши точки
    private readonly MemoryLayer _pointsLayer = new()
    {
        Name = "User Points",
        Style = null
    };

    private readonly MemoryLayer _routeLayer = new()
    {
        Name = "Route",
        Style = null
    };

    public MainPage()
    {
        InitializeComponent();

        InitializeMap();
        RefreshPointsList();
        Loaded += MainPage_Loaded;
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

        MapView.Map.Layers.Add(tileLayer);
        MapView.Map.Layers.Add(_routeLayer);
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

    private async void AddPointToMap(MapPoint point)
    {
        AddPointToMapInternal(point);

        await SavePointsAsync();
    }

    private void AddPointToMapInternal(MapPoint point)
    {
        _points.Add(point);

        var projected =
            SphericalMercator.FromLonLat(
                point.Longitude,
                point.Latitude
            );

        var mapPosition =
            new MPoint(
                projected.x,
                projected.y
            );

        var feature =
            new PointFeature(
                mapPosition
            );

        feature["Id"] =
            point.Id.ToString();

        feature["Name"] =
            point.Name;

        feature["Description"] =
            point.Description;

        feature.Styles.Add(
            CreateMarkerStyle(point)
        );

        _pointsLayer.Features =
            _pointsLayer.Features
                .Append(feature)
                .ToList();

        _pointsLayer.DataHasChanged();

        RefreshPointsList();

        MapView.Map.Navigator.CenterOnAndZoomTo(
            mapPosition,
            MapView.Map.Navigator.Resolutions[12],
            500
        );
    }

    private async Task SavePointsAsync()
    {
        try
        {
            await PointStorageService
                .SavePointsAsync(_points);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Ошибка сохранения",
                ex.Message,
                "OK"
            );
        }
    }

    private async Task LoadSavedPointsAsync()
    {
        try
        {
            var savedPoints =
                await PointStorageService
                    .LoadPointsAsync();

            foreach (var point in savedPoints)
            {
                AddPointToMapInternal(point);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Ошибка загрузки",
                ex.Message,
                "OK"
            );
        }
    }

    private void RefreshPointsList()
    {
        PointsCollectionView.ItemsSource = null;
        PointsCollectionView.ItemsSource = _points;

        EmptyPointsLabel.IsVisible =
            _points.Count == 0;

        PointsCollectionView.IsVisible =
            _points.Count > 0;
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

    private void PointsCollectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault()
            is not MapPoint point)
        {
            return;
        }

        MoveToPoint(point);

        PointsCollectionView.SelectedItem = null;
    }

    private void GoToPoint_Clicked(object? sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.CommandParameter is not MapPoint point)
            return;

        MoveToPoint(point);
    }

    private void MoveToPoint(MapPoint point)
    {
        var projected =
            SphericalMercator.FromLonLat(
                point.Longitude,
                point.Latitude
            );

        var mapPosition = new MPoint(
            projected.x,
            projected.y
        );

        MapView.Map.Navigator.CenterOnAndZoomTo(
            mapPosition,
            MapView.Map.Navigator.Resolutions[12],
            500
        );
    }

    private async void DeletePoint_Clicked(object? sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.CommandParameter is not MapPoint point)
            return;

        bool confirmed = await DisplayAlertAsync(
            "Удаление точки",
            $"Удалить точку «{point.Name}»?",
            "Удалить",
            "Отмена"
        );

        if (!confirmed)
            return;

        RemovePoint(point);
    }

    private async void RemovePoint(MapPoint point)
    {
        _points.Remove(point);

        var featureToRemove =
            _pointsLayer.Features
                .FirstOrDefault(
                    feature =>
                        feature["Id"]?.ToString()
                        == point.Id.ToString()
                );

        if (featureToRemove != null)
        {
            _pointsLayer.Features =
                _pointsLayer.Features
                    .Where(
                        feature =>
                            feature != featureToRemove
                    )
                    .ToList();

            _pointsLayer.DataHasChanged();
        }

        RefreshPointsList();

        await SavePointsAsync();
    }

    private async void MyLocation_Clicked(object? sender, EventArgs e)
    {
        try
        {
            GpsStatusLabel.Text =
                "GPS: определяем местоположение...";

            var request =
                new GeolocationRequest(
                    GeolocationAccuracy.Medium,
                    TimeSpan.FromSeconds(10)
                );

            Location? location =
                await Geolocation.Default
                    .GetLocationAsync(request);


            if (location == null)
            {
                GpsStatusLabel.Text =
                    "GPS: местоположение не найдено";

                await DisplayAlertAsync(
                    "GPS",
                    "Не удалось определить местоположение.",
                    "OK"
                );

                return;
            }


            ShowCurrentLocation(location);
        }
        catch (FeatureNotSupportedException)
        {
            GpsStatusLabel.Text =
                "GPS: не поддерживается";

            await DisplayAlertAsync(
                "GPS",
                "Геолокация не поддерживается на этом устройстве.",
                "OK"
            );
        }
        catch (PermissionException)
        {
            GpsStatusLabel.Text =
                "GPS: нет разрешения";

            await DisplayAlertAsync(
                "GPS",
                "Приложению не разрешён доступ к местоположению.",
                "OK"
            );
        }
        catch (Exception ex)
        {
            GpsStatusLabel.Text =
                "GPS: ошибка";

            await DisplayAlertAsync(
                "Ошибка GPS",
                ex.Message,
                "OK"
            );
        }
    }

    private void ShowCurrentLocation(Location location)
    {
        _currentLocation = location;

        double latitude = location.Latitude;
        double longitude = location.Longitude;

        var projected = SphericalMercator.FromLonLat(longitude,latitude);
        var mapPosition = new MPoint(projected.x, projected.y);

        // Если предыдущая GPS-точка уже была, удаляем её
        if (_currentLocationFeature != null)
        {
            _pointsLayer.Features =
                _pointsLayer.Features
                    .Where(feature =>
                        feature != _currentLocationFeature
                    )
                    .ToList();
        }

        // Создаём новую GPS-точку
        _currentLocationFeature =
            new PointFeature(mapPosition);

        // Отдельный стиль для GPS
        _currentLocationFeature.Styles.Add(
            new VectorStyle
            {
                Fill =
                    new Mapsui.Styles.Brush(
                        Mapsui.Styles.Color.Blue
                    ),

                Outline =
                    new Mapsui.Styles.Pen(
                        Mapsui.Styles.Color.White,
                        4
                    )
            }
        );


        _pointsLayer.Features =
            _pointsLayer.Features
                .Append(_currentLocationFeature)
                .ToList();


        _pointsLayer.DataHasChanged();


        // Центрируем карту
        MapView.Map.Navigator.CenterOnAndZoomTo(
            mapPosition,
            MapView.Map.Navigator.Resolutions[13],
            500
        );


        // Показываем координаты
        GpsStatusLabel.Text =
            $"GPS: {latitude:F5}, {longitude:F5}";
    }

    private async Task OpenPhotoAsync()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(
                new PickOptions
                {
                    PickerTitle = "Выберите фотографию",

                    FileTypes = FilePickerFileType.Images
                }
            );

            if (result == null)
            {
                return;
            }

            await ProcessPhotoAsync(result.FullPath);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Ошибка",
                $"Не удалось открыть изображение:\n{ex.Message}",
                "OK"
            );
        }
    }

    private async void OpenPhotoMenu_Clicked(object? sender, EventArgs e)
    {
        await OpenPhotoAsync();
    }

    private async Task ProcessPhotoAsync(string photoPath)
    {
        if (string.IsNullOrWhiteSpace(photoPath))
        {
            await DisplayAlertAsync(
                "Ошибка",
                "Путь к фотографии пустой.",
                "OK"
            );

            return;
        }

        if (!File.Exists(photoPath))
        {
            await DisplayAlertAsync(
                "Ошибка",
                $"Файл не найден:\n{photoPath}",
                "OK"
            );

            return;
        }

        try
        {
            var gps =
                PhotoMetadataService
                    .GetGpsCoordinates(photoPath);

            // Если GPS в EXIF нет
            if (gps == null)
            {
                await DisplayAlertAsync(
                    "GPS не найден",
                    $"Фотография выбрана:\n" +
                    $"{Path.GetFileName(photoPath)}\n\n" +
                    "Но GPS-координаты в EXIF отсутствуют.",
                    "OK"
                );

                return;
            }

            double latitude =
                gps.Value.Latitude;

            double longitude =
                gps.Value.Longitude;

            // Временно показываем, что GPS реально прочитан
            await DisplayAlertAsync(
                "EXIF GPS найден",
                $"Широта: {latitude:F6}\n" +
                $"Долгота: {longitude:F6}",
                "OK"
            );

            // Создаём точку
            var point = new MapPoint
            {
                Name =
                    Path.GetFileNameWithoutExtension(photoPath),

                Latitude = latitude,

                Longitude = longitude,

                Description =
                    "Точка создана из GPS фотографии",

                Source = "Photo",

                PhotoPath = photoPath,

                MarkerColor = "Blue",

                MarkerShape = "Pin",

                MarkerContent = "Icon",

                MarkerIcon = "Photo"
            };

            // ВАЖНО:
            // именно эта строка добавляет точку
            AddPointToMap(point);

            await DisplayAlertAsync(
                "Точка добавлена",
                $"Точка «{point.Name}» добавлена на карту.",
                "OK"
            );
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Ошибка обработки фото",
                ex.ToString(),
                "OK"
            );
        }
    }

    private async void MainPage_Loaded(object? sender, EventArgs e)
    {
        Loaded -= MainPage_Loaded;

        await LoadSavedPointsAsync();

        if (string.IsNullOrWhiteSpace(
            App.StartupPhotoPath))
        {
            return;
        }

        string photoPath =
            App.StartupPhotoPath;

        App.StartupPhotoPath = null;

        if (!File.Exists(photoPath))
        {
            await DisplayAlertAsync(
                "Ошибка",
                $"Файл не найден:\n{photoPath}",
                "OK"
            );

            return;
        }

        await ProcessPhotoAsync(photoPath);
    }
}