using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DtgdExample.Config
{
    /// <summary>
    /// Подкраска ячеек позиций по роботам, чтоб можно было какая сделка по робота ведется реальная или виртуальная
    /// </summary>
    public class ColorConverterBackGround : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!String.IsNullOrEmpty(System.Convert.ToString(value)))
            {
                var color = (Color)System.Windows.Media.ColorConverter.ConvertFromString(System.Convert.ToString(value));
                return new SolidColorBrush(color);
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
