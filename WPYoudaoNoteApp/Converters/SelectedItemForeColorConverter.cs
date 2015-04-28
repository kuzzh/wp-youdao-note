using System;
using System.Windows.Data;
using System.Windows.Media;

namespace WPYoudaoNoteApp.Converters
{
    public class SelectedItemForeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var backColor = (Color)value;
            return new SolidColorBrush(backColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
