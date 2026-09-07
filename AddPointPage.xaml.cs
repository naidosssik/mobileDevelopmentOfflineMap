using System.Globalization;
using OfflineMapApp.Models;

namespace OfflineMapApp;

public partial class AddPointPage : ContentPage
{
    private string _selectedColor = "Red";
    private string _selectedShape = "Circle";
    private string _selectedContent = "Empty";
    private MapPoint? _editingPoint;

    public AddPointPage()
    {
        InitializeComponent();

        UpdateColorSelection();
        UpdateShapeSelection();
        UpdateContentSelection();
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


        if (!TryParseCoordinate(
            latitudeText,
            out double latitude))
        {
            ShowError("Широта должна быть числом.");
            return;
        }

        if (!TryParseCoordinate(
            longitudeText,
            out double longitude))
        {
            ShowError("Долгота должна быть числом.");
            return;
        }

        // Проверка диапазонов

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


        int? markerNumber = null;

        if (_selectedContent == "Number")
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
                ShowError(
                    "Номер точки должен быть целым числом."
                );

                return;
            }

            if (number < 1)
            {
                ShowError(
                    "Номер точки должен быть больше 0."
                );

                return;
            }

            markerNumber = number;
        }


        string markerIcon =
            IconPicker.SelectedItem?.ToString() ?? "Heart";

        MapPoint point;

        if (_editingPoint == null)
        {
            point = new MapPoint();
        }
        else
        {
            point = _editingPoint;
        }

        point.Name = name;
        point.Latitude = latitude;
        point.Longitude = longitude;
        point.Description = description;
        point.Source = _editingPoint?.Source ?? "Manual";

        point.MarkerColor = _selectedColor;
        point.MarkerShape = _selectedShape;
        point.MarkerContent = _selectedContent;
        point.MarkerIcon = markerIcon;
        point.MarkerNumber = markerNumber;

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

    private void PinShape_Clicked(
        object? sender,
        EventArgs e)
    {
        _selectedShape = "Pin";

        UpdateShapeSelection();
    }


    private void CircleShape_Clicked(
        object? sender,
        EventArgs e)
    {
        _selectedShape = "Circle";

        UpdateShapeSelection();
    }


    private void UpdateShapeSelection()
    {
        PinShapeButton.BorderWidth =
            _selectedShape == "Pin" ? 2 : 0;

        CircleShapeButton.BorderWidth =
            _selectedShape == "Circle" ? 2 : 0;


        PinShapeButton.BorderColor = Colors.Black;
        CircleShapeButton.BorderColor = Colors.Black;
    }

    private void EmptyContent_Clicked(
        object? sender,
        EventArgs e)
    {
        SelectContent("Empty");
    }


    private void NumberContent_Clicked(
        object? sender,
        EventArgs e)
    {
        SelectContent("Number");
    }


    private void IconContent_Clicked(
        object? sender,
        EventArgs e)
    {
        SelectContent("Icon");
    }


    private void SelectContent(string content)
    {
        _selectedContent = content;


        // Если выбран номер — показываем поле ввода номера
        NumberSettings.IsVisible =
            content == "Number";


        // Если выбрана иконка — показываем Picker
        IconSettings.IsVisible =
            content == "Icon";


        UpdateContentSelection();
    }


    private void UpdateContentSelection()
    {
        EmptyContentButton.BorderWidth =
            _selectedContent == "Empty" ? 2 : 0;

        NumberContentButton.BorderWidth =
            _selectedContent == "Number" ? 2 : 0;

        IconContentButton.BorderWidth =
            _selectedContent == "Icon" ? 2 : 0;


        EmptyContentButton.BorderColor = Colors.Black;
        NumberContentButton.BorderColor = Colors.Black;
        IconContentButton.BorderColor = Colors.Black;


        NumberSettings.IsVisible =
            _selectedContent == "Number";

        IconSettings.IsVisible =
            _selectedContent == "Icon";
    }


    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }

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

    public AddPointPage(MapPoint point)
    {
        InitializeComponent();

        _editingPoint = point;

        NameEntry.Text = point.Name;
        LatitudeEntry.Text = point.Latitude.ToString();
        LongitudeEntry.Text = point.Longitude.ToString();
        DescriptionEditor.Text = point.Description;

        _selectedColor = point.MarkerColor;
        _selectedShape = point.MarkerShape;     
        _selectedContent = point.MarkerContent;

        NumberEntry.Text =
            point.MarkerNumber?.ToString() ?? string.Empty;

        IconPicker.SelectedItem =
            point.MarkerIcon;

        UpdateColorSelection();
        UpdateShapeSelection();
        UpdateContentSelection();
    }
}