using System;
using System.Globalization;
using System.Windows.Data;

namespace ChatApp.Classes
{
    public class FirstCharacterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (((string)value).Length > 0)
                {
                    var s = (string)value;
                    return s.Substring(0, 1).ToUpper();
                }
                else
                {
                    return 'A';
                }
            }
            else
            {
                return 'A';
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
