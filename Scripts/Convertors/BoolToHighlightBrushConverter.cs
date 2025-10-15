using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System;

namespace Calendar.Scripts
{
    /// <summary>
    /// Převádí bool hodnotu na barvu pozadí – světle modrou, pokud je true.
    /// </summary>
    public class BoolToHighlightBrushConverter : IValueConverter
    {
        /// <summary>
        /// Vrací LightBlue, pokud je hodnota true, jinak Transparent.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is bool isSelected && isSelected
                ? new SolidColorBrush(Colors.LightBlue)
                : new SolidColorBrush(Colors.Transparent);

        /// <summary>
        /// Nepodporováno – vždy vyhodí výjimku.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
