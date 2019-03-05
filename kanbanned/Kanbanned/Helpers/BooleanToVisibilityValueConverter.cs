using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Kanbanned.Helpers
{
    public class BooleanToVisibilityValueConverter : IValueConverter
    {
        /// <summary>
        /// Converter boolean vrednosti u Visibility vrednost kako bih bindovao
        /// visibility nekog elementa u zavisnosti od neke vrednosti
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((bool)value == false)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((Visibility)value == Visibility.Visible)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
