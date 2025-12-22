using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace IPManagementInterface.Converters
{
    public class TagsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<string> tags && tags.Any())
            {
                return string.Join(", ", tags);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string tagsString && !string.IsNullOrWhiteSpace(tagsString))
            {
                return tagsString.Split(',')
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();
            }
            return new List<string>();
        }
    }
}
