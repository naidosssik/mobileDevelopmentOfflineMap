using System.Globalization;
using OfflineMapApp.Models;

namespace OfflineMapApp;

public partial class AddPointPage : ContentPage
{
    public AddPointPage()
    {
        InitializeComponent();
    }

    private async void Save_Clicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        string name = NameEntry.Text?.Trim() ?? string.Empty;
        string latitudeText = LatitudeEntry.Text?.Trim() ?? string.Empty;
        string longitudeText = LongitudeEntry.Text?.Trim() ?? string.Empty;
        string description = DescriptionEditor.Text?.Trim() ?? string.Empty;


        // Проверяем обязательные поля

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


        // Преобразуем широту в число

        if (!TryParseCoordinate(latitudeText, out double latitude))
        {
            ShowError("Широта должна быть числом.");
            return;
        }


        // Преобразуем долготу в число

        if (!TryParseCoordinate(longitudeText, out double longitude))
        {
            ShowError("Долгота должна быть числом.");
            return;
        }


        // Проверяем диапазоны координат

        if (latitude < -90 || latitude > 90)
        {
            ShowError("Широта должна находиться в диапазоне от -90 до 90.");
            return;
        }

        if (longitude < -180 || longitude > 180)
        {
            ShowError("Долгота должна находиться в диапазоне от -180 до 180.");
            return;
        }


        // Создаём объект точки

        var point = new MapPoint
        {
            Name = name,
            Latitude = latitude,
            Longitude = longitude,
            Description = description,
            Source = "Manual"
        };


        // Возвращаем точку на главную страницу

        if (BindingContext is Action<MapPoint> addPointAction)
        {
            addPointAction(point);
        }


        // Возвращаемся назад

        await Navigation.PopModalAsync();
    }


    private async void Cancel_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
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
}