using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Bank2Kasa.Converters
{
    /// <summary>
    /// Converter used in XAML for the conversion of a boolean to <see cref="Visibility" /> enumeration.
    /// If value is <c>true</c>, it is converted to <see cref="Visibility.Visible" />, otherwise it
    /// is converted to <see cref="Visibility.Collapsed" />.
    /// </summary>
    /// <example language="XML">
    /// <Window.Resources>
    ///     <converters:BooleanToVisibility x:Key="BooleanToVisibility"/>
    /// </Window.Resources>
    /// <!-- TextBox.Visibility is set to Visible if binding evaluates to true -->
    /// <TextBox IsEnabled="{Binding IfTrueVisible, Converter={StaticResource BooleanToVisibility}}" />}}
    /// </example>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityHidden : IValueConverter
    {
        #region Operations
        /// <summary>
        /// Executes the conversion.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                throw new ArgumentException("Not of type Boolean.", "value");
            }

            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// Converts a converted value back to its original value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility))
            {
                throw new ArgumentException("Not of type Visibility.", "value");
            }

            if ((Visibility)value == Visibility.Visible)
            {
                return true;
            }

            return false;
        }
        #endregion Operations
    }
}
