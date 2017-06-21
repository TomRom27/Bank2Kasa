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
                    return new SolidColorBrush(Color.FromRgb(204,153,102)); 
                case ActionToDo.AnnotateInKasa:
                    return new SolidColorBrush(Color.FromRgb(242, 230, 217)); 
                case ActionToDo.RemoveFromImport:
                    return null;//  new SolidColorBrush(Color.FromRgb(172, 115, 57)); 
                case ActionToDo.Add2Kasa:
                    return new SolidColorBrush(Color.FromRgb(223, 191, 159));
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
