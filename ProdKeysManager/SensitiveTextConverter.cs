using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ProdKeysManager
{
    public class SensitiveTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && targetType == typeof(string))
            {
                if (Properties.Settings.Default.EnableSensitiveMode)
                {
                    if (str.Length > 8)
                    {
                        return str.Substring(0, 4) + new string('*', str.Length - 8) + str.Substring(str.Length - 4);
                    }
                    else
                    {
                        return new string('*', str.Length);
                    }
                }
                else
                {
                    return str;
                }
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
