using System;
using System.Globalization;
using System.Windows.Data;

namespace Jeff.Converter
{
    class GetActualPosition : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double retval = 0d;
            double val = (double)value;
            double param = double.Parse(parameter.ToString());

            retval = val + param;

            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
