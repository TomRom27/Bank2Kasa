using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

using WUKasa;

namespace Bank2Kasa.Converters
{
    [ValueConversion(typeof(ActionToDo), typeof(SolidColorBrush))]
    public class ActionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((ActionToDo)value)
            {
                case ActionToDo.Add2KasaAndRemoveFromImport:
                    return new SolidColorBrush(Color.FromRgb(26,201,26)); // green
                case ActionToDo.AnnotateInKasa:
                    return new SolidColorBrush(Color.FromRgb(240, 248, 255)); // very light blue
                case ActionToDo.RemoveFromImport:
                    return new SolidColorBrush(Color.FromRgb(245, 205, 24)); // dark yellow
                case ActionToDo.Add2Kasa:
                    return new SolidColorBrush(Color.FromRgb(245, 255, 24)); // bright syellow
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
