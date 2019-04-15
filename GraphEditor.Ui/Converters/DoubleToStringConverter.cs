using System;
using System.Globalization;
using System.Windows.Data;

namespace GraphEditor.Ui.Converters
{
    public class DoubleToStringConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double) value).ToString();
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.Parse(value.ToString());
        }
    }
}
