using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Курсач
{
    public class ConverterForBtnInWrap : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            double param = double.Parse(parameter.ToString());
            double actDim = double.Parse(value.ToString());
            return (double)actDim * param;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                if (double.TryParse(parameter.ToString(), out double result))
                {
                    return result;
                }
            }
            return new Exception();
        }
    }
}
