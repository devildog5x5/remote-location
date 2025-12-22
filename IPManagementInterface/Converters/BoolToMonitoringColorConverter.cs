using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace IPManagementInterface.Converters
{
    public class BoolToMonitoringColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isMonitoring)
            {
                return new SolidColorBrush(isMonitoring ? Colors.Red : Colors.Green);
            }
            return new SolidColorBrush(Colors.Green);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
