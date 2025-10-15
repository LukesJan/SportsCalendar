using System;
using System.Globalization;
using System.Windows.Data;

namespace Calendar.Scripts
{
    /// <summary>
    /// Převádí TimeSpan na řetězec ve formátu "hh:mm".
    /// </summary>
    public class TimeConverter : IValueConverter
    {
        /// <summary>
        /// Konvertuje TimeSpan na string "hh:mm".
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is TimeSpan time ? time.ToString(@"hh\:mm") : value;

        /// <summary>
        /// Konverze zpět není implementována.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
