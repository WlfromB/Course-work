using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Курсач
{
    public class WidthToFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is double windowWidth)
            {
                if (parameter is string fontSize)
                {
                    return Math.Round((windowWidth / 1080 * 1.33 * double.Parse(fontSize))).ToString();
                }
                else if (parameter is double fontSize1)
                {
                    return Math.Round((windowWidth / 1080 * 1.33 * fontSize1)).ToString();
                }
                else

                {
                    return (windowWidth / 1080 * 1.33 * 15).ToString();
                }
            }
            return "15";
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new Exception();
        }
    }
}
