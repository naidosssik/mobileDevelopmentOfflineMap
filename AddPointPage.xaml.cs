using System.Globalization;
using OfflineMapApp.Models;

namespace OfflineMapApp;

public partial class AddPointPage : ContentPage
{
    private string _selectedColor = "Red";
    private string _selectedType = "Icon";

    public AddPointPage()
    {
        InitializeComponent();

        UpdateColorSelection();
        UpdateTypeSelection();
    }


    private async void Save_Clicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        string name =
            NameEntry.Text?.Trim() ?? string.Empty;

        string latitudeText =
            LatitudeEntry.Text?.Trim() ?? string.Empty;

        string longitudeText =
            LongitudeEntry.Text?.Trim() ?? string.Empty;

        string description =
            DescriptionEditor.Text?.Trim() ?? string.Empty;


        // Обязательные поля

        if (string.IsNullOrWhiteSpace(name))
        {
            ShowError("Введите название точки.");
            return;
        }

        if (string.IsNullOrWhiteSpace(latitudeText))
        {
            ShowError("Введите широту.");
            return;
        }

        if (string.IsNullOrWhiteSpace(longitudeText))
        {
            ShowError("Введите долготу.");
            return;
        }


        // Проверка координат

        if (!TryParseCoordinate(latitudeText, out double latitude))
        {
            ShowError("Широта должна быть числом.");
            return;
        }

        if (!TryParseCoordinate(longitudeText, out double longitude))
        {
            ShowError("Долгота должна быть числом.");
            return;
        }


        if (latitude < -90 || latitude > 90)
        {
            ShowError(
                "Широта должна находиться в диапазоне от -90 до 90."
            );

            return;
        }

        if (longitude < -180 || longitude > 180)
        {
            ShowError(
                "Долгота должна находиться в диапазоне от -180 до 180."
            );

            return;
        }


        // Иконка

        string markerIcon =
            IconPicker.SelectedItem?.ToString() ?? "Heart";


        // Номер

        int? markerNumber = null;

        if (_selectedType == "Number")
        {
            if (string.IsNullOrWhiteSpace(NumberEntry.Text))
            {
                ShowError("Введите номер точки.");
                return;
            }

            if (!int.TryParse(
                NumberEntry.Text,
                out int number))
            {
                ShowError("Номер точки должен быть целым числом.");
                return;
            }

            markerNumber = number;
        }


        // Создание точки

        var point = new MapPoint
        {
            Name = name,

            Latitude = latitude,
            Longitude = longitude,

            Description = description,

            Source = "Manual",

            MarkerColor = _selectedColor,
            MarkerType = _selectedType,
            MarkerIcon = markerIcon,
            MarkerNumber = markerNumber
        };


        // Передаём точку обратно на MainPage

        if (BindingContext is Action<MapPoint> addPointAction)
        {
            addPointAction(point);
        }


        await Navigation.PopModalAsync();
    }


    private async void Cancel_Clicked(
        object? sender,
        EventArgs e)
    {
        await Navigation.PopModalAsync();
    }


    // -----------------------------
    // Цвет точки
    // -----------------------------

    private void RedColor_Clicked(
        object? sender,
        EventArgs e)
    {
        _selectedColor = "Red";

        UpdateColorSelection();
    }


    private void BlueColor_Clicked(
        object? sender,
        EventArgs e)
    {
        _selectedColor = "Blue";

        UpdateColorSelection();
    }


    private void GreenColor_Clicked(
        object? sender,
        EventArgs e)
    {
        _selectedColor = "Green";

        UpdateColorSelection();
    }


    private void PurpleColor_Clicked(
        object? sender,
        EventArgs e)
    {
        _selectedColor = "Purple";

        UpdateColorSelection();
    }


    private void UpdateColorSelection()
    {
        RedColorButton.BorderWidth =
            _selectedColor == "Red" ? 3 : 0;

        BlueColorButton.BorderWidth =
            _selectedColor == "Blue" ? 3 : 0;

        GreenColorButton.BorderWidth =
            _selectedColor == "Green" ? 3 : 0;

        PurpleColorButton.BorderWidth =
            _selectedColor == "Purple" ? 3 : 0;


        RedColorButton.BorderColor = Colors.Black;
        BlueColorButton.BorderColor = Colors.Black;
        GreenColorButton.BorderColor = Colors.Black;
        PurpleColorButton.BorderColor = Colors.Black;
    }


    // -----------------------------
    // Тип точки
    // -----------------------------

    private void PinType_Clicked(
        object? sender,
        EventArgs e)
    {
        SelectMarkerType("Pin");
    }


    private void CircleType_Clicked(
        object? sender,
        EventArgs e)
    {
        SelectMarkerType("Circle");
    }


    private void NumberType_Clicked(
        object? sender,
        EventArgs e)
    {
        SelectMarkerType("Number");
    }


    private void IconType_Clicked(
        object? sender,
        EventArgs e)
    {
        SelectMarkerType("Icon");
    }


    private void SelectMarkerType(string type)
    {
        _selectedType = type;

        IconSettings.IsVisible =
            type == "Icon";

        NumberSettings.IsVisible =
            type == "Number";

        UpdateTypeSelection();
    }


    private void UpdateTypeSelection()
    {
        PinTypeButton.BorderWidth =
            _selectedType == "Pin" ? 2 : 0;

        CircleTypeButton.BorderWidth =
            _selectedType == "Circle" ? 2 : 0;

        NumberTypeButton.BorderWidth =
            _selectedType == "Number" ? 2 : 0;

        IconTypeButton.BorderWidth =
            _selectedType == "Icon" ? 2 : 0;


        PinTypeButton.BorderColor = Colors.Black;
        CircleTypeButton.BorderColor = Colors.Black;
        NumberTypeButton.BorderColor = Colors.Black;
        IconTypeButton.BorderColor = Colors.Black;
    }


    // -----------------------------
    // Ошибки
    // -----------------------------

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }


    // -----------------------------
    // Координаты
    // -----------------------------

    private bool TryParseCoordinate(
        string text,
        out double coordinate)
    {
        text = text.Replace(',', '.');

        return double.TryParse(
            text,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out coordinate
        );
    }
}