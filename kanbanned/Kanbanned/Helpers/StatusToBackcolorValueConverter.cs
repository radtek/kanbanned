using Kanbanned.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Kanbanned.Helpers
{
    public class StatusToBackcolorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ZStatus status = (ZStatus)value;
            if (status == ZStatus.NotStarted)
            {
                return new SolidColorBrush(Colors.Gray);
            }
            else if(status == ZStatus.InProgress)
            {
                return new SolidColorBrush(Colors.Green);
            }
            else if(status == ZStatus.Finished)
            {
                return new SolidColorBrush(Colors.SteelBlue);
            }
            else
            {
                return new SolidColorBrush(Colors.Red);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
