using System;
using System.Globalization;
using System.Windows.Data;

namespace SerialInspector.View
{
    [ValueConversion(typeof(object), typeof(string))]
    public class HexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte)
            {
                return string.Format("0x{0:X2}", value);
            }

            if (value is string)
            {
                return ("0x" + value).PadRight(10, '0');
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(byte))
            {
                return System.Convert.ToByte(value.ToString(), 16);
            }

            if (targetType == typeof(string))
            {
                return value.ToString().Remove(0, 2).PadRight(8, '0');
            }


            throw new NotImplementedException();
        }
    }
}