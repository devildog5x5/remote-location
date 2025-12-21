using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using IPManagementInterface.Models;

namespace IPManagementInterface.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DeviceStatus status)
            {
                return status switch
                {
                    DeviceStatus.Online => Colors.Green,
                    DeviceStatus.Offline => Colors.Red,
                    _ => Colors.Gray
                };
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
