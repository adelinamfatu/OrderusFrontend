using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AppFrontend.Converters
{
    public class TimeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime && parameter is ListView listView)
            {
                DateTime currentTime = DateTime.Now;

                if (dateTime < currentTime)
                {
                    return Color.SlateGray;
                }
                else if (dateTime > currentTime)
                {
                    return Color.PaleVioletRed;
                }
            }

            return Color.PaleGreen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
