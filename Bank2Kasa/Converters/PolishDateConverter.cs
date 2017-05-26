using System;
using System.Globalization;
using System.Windows.Data;

namespace Bank2Kasa.Converters
{
    public class PolishDateConverter : IValueConverter
    {
        public const string DateFormat = "dd.MM.yyyy";

        #region Public methods
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is DateTime))
            {
                throw new ArgumentException("Not of type DateTime.", "value");
            }
            return ((DateTime)value).ToString(DateFormat);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is String))
            {
                throw new ArgumentException("Not of type String.", "value");
            }
            return DateTime.ParseExact((string)value, DateFormat, new System.Globalization.CultureInfo("pl-PL"));
        }
        #endregion Public methods

        #region Private methods
        #endregion Private methods
    }

}
