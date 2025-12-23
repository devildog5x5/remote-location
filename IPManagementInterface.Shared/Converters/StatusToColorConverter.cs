using System.Globalization;

namespace IPManagementInterface.Shared.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Models.DeviceStatus status)
        {
            return status switch
            {
                Models.DeviceStatus.Online => Color.FromArgb("#4CAF50"),
                Models.DeviceStatus.Offline => Color.FromArgb("#F44336"),
                _ => Color.FromArgb("#757575")
            };
        }
        return Color.FromArgb("#757575");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
