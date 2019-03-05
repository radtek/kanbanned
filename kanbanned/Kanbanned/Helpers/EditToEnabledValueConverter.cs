using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Kanbanned.Helpers
{
    public class EditToEnabledValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String uloga = (String)value;
            if (uloga.Equals("VODJA") || uloga.Equals("KREATOR"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {           
            if ((bool)value == true)
            {
                return "KREATOR";
            }
            else
            {
                return "RADNIK";
            }
        }
    }
}
