using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SendMultipleEmails.Converter
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class ReverseBool : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool re = (bool)value;
            return !re;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool re = (bool)value;
            return !re;

            // return DependencyProperty.UnsetValue;
        }

    }
}
